using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     A standalone implementation of a thread-bound dispatcher.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A <see cref="ThreadDispatcher" /> provides a queue for delegates, filled through <c>Send</c> and <c>Post</c> methods, which is processed on the thread where <see cref="Run" /> is called (<see cref="Run" /> blocks while executing the queue until <see cref="Shutdown" />, <see cref="ShutdownAsync"/>, or <see cref="BeginShutdown"/> is called).
    ///     </para>
    ///     <para>
    ///         The delegates are executed in the order they are added to the queue.
    ///         When all delegates are executed, or the queue is empty respectively, <see cref="ThreadDispatcher" /> waits for new delegates to process.
    ///     </para>
    ///     <para>
    ///         During <see cref="Run" />, the current <see cref="SynchronizationContext" /> is replaced by an instance of <see cref="ThreadDispatcherSynchronizationContext" /> and restored afterwards.
    ///     </para>
    ///     <para>
    ///         A watchdog can be used to ensure that the execution of a delegate does not block the dispatcher undetected.
    ///         The watchdog is active whenever <see cref="WatchdogTimeout" /> is not null.
    ///         The watchdog runs in a separate thread and raises the <see cref="Watchdog" /> event if the execution of a delegate takes longer than the specified timeout.
    ///     </para>
    ///     <note type="important">
    ///         Whether <see cref="ExecutionContext" />, <see cref="SynchronizationContext"/>, and/or <see cref="CultureInfo" /> is captured and used for executing a delegate, depends on the used <see cref="ThreadDispatcherOptions" />.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Allow dispatching during shutdown?
    public sealed class SimpleDispatcher : IThreadDispatcher
    {
        #region Constants

        /// <summary>
        ///     The default value for <see cref="DefaultOptions" /> if it is not explicitly set.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default value is <see cref="ThreadDispatcherOptions.None" />.
        ///     </para>
        /// </remarks>
        public const ThreadDispatcherOptions DefaultOptionsValue = ThreadDispatcherOptions.None;

        /// <summary>
        ///     The default value for <see cref="DefaultPriority" /> if it is not explicitly set.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default value is <c> int.MaxValue / 2 </c>.
        ///     </para>
        /// </remarks>
        public const int DefaultPriorityValue = int.MaxValue / 2;

        private const int WatchdogCheckInterval = 20;

        private const int WatchdogThreadTimeoutMilliseconds = 1000;

        private const int EventCloseDelayMilliseconds = 20;

        private const int SignalClearDelayMilliseconds = 20;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SimpleDispatcher" />.
        /// </summary>
        public SimpleDispatcher()
        {
            this.SyncRoot = new object();

            this.CatchExceptions = false;
            this.DefaultPriority = SimpleDispatcher.DefaultPriorityValue;
            this.DefaultOptions = SimpleDispatcher.DefaultOptionsValue;
            this.WatchdogTimeout = null;

            this.ShutdownMode = ThreadDispatcherShutdownMode.None;

            /*R -> R*/ this.Thread = null;
            /*R -> (CH & D) + (CH & R)*/ this.Queue = null;
            /*R -> R*/ this.Posted = null;
            /*R -> R*/ this.IdleSignals = null;
            /*R -> R*/ this.CurrentPriority = null;
            /*R -> R*/ this.CurrentOptions = null;
            /*R -> (CH & D) + (CH & R)*/ this.CurrentOperation = null;
            /*R -> (D) + (R)*/ this.KeepAlives = null;
            /*R -> R*/ this.WatchdogLoop = null;

            /*C -> (CH & D) + (CH & R)*/ this.PreRunQueue = new PriorityQueue<SimpleDispatcherOperation>();

            /*C -> (F) + (R) */ this.FinishedEvent = new ManualResetEvent(false);
            /*C -> (F) + (R) */ this.FinishedSignals = new List<TaskCompletionSource<object>>();

            // (F) -> (D) -> (BS) & (CH)
            // (R) -> (CH)
        }

        /// <summary>
        ///     Garbage collects this instance of <see cref="SimpleDispatcher" />.
        /// </summary>
        ~SimpleDispatcher()
        {
            ((IDisposable)this).Dispose();

            this.FinishedSignals?.ForEach(x => x.TrySetResult(null));
            System.Threading.Thread.Sleep(SimpleDispatcher.SignalClearDelayMilliseconds);
            this.FinishedSignals?.Clear();

            this.FinishedEvent?.Set();
            System.Threading.Thread.Sleep(SimpleDispatcher.EventCloseDelayMilliseconds);
            this.FinishedEvent?.Close();
        }

        #endregion




        #region Instance Fields

        private bool _catchExceptions;
        private ThreadDispatcherOptions _defaultOptions;
        private int _defaultPriority;
        private ThreadDispatcherShutdownMode _shutdownMode;
        private TimeSpan? _watchdogTimeout;

        #endregion




        #region Instance Properties/Indexer

        private Stack<IThreadDispatcherOperation> CurrentOperation { get; set; }

        private Stack<ThreadDispatcherOptions> CurrentOptions { get; set; }

        private Stack<int> CurrentPriority { get; set; }

        private ManualResetEvent FinishedEvent { get; set; }

        private List<TaskCompletionSource<object>> FinishedSignals { get; set; }

        private List<TaskCompletionSource<object>> IdleSignals { get; set; }

        private HashSet<object> KeepAlives { get; set; }

        private ManualResetEvent Posted { get; set; }

        private PriorityQueue<IThreadDispatcherOperation> PreRunQueue { get; set; }

        private PriorityQueue<IThreadDispatcherOperation> Queue { get; set; }

        private Thread Thread { get; set; }

        private WatchdogThread WatchdogLoop { get; set; }

        private ThreadDispatcherTaskScheduler TaskScheduler { get; set; }

        private ThreadDispatcherSynchronizationContext SynchronizationContext { get; set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Processes the delegate queue and waits for new delegates until <see cref="Shutdown" /> is called.
        /// </summary>
        /// <exception cref="InvalidOperationException"> The dispatcher is already running. </exception>
        /// <exception cref="ThreadDispatcherException"> The execution of a delegate has thrown an exception and <see cref="CatchExceptions" /> is false. </exception>
        public void Run ()
        {
            SynchronizationContext synchronizationContextBackup = System.Threading.SynchronizationContext.Current;

            try
            {
                lock (this.SyncRoot)
                {
                    this.VerifyNotRunning();

                    synchronizationContextBackup = System.Threading.SynchronizationContext.Current;

                    this.Thread = Thread.CurrentThread;
                    this.Queue = new PriorityQueue<IThreadDispatcherOperation>();
                    this.Posted = new ManualResetEvent(this.PreRunQueue.Count > 0);
                    this.IdleSignals = new List<TaskCompletionSource<object>>();
                    this.CurrentPriority = new Stack<int>();
                    this.CurrentOptions = new Stack<ThreadDispatcherOptions>();
                    this.CurrentOperation = new Stack<IThreadDispatcherOperation>();
                    this.KeepAlives = new HashSet<object>();

                    this.ShutdownMode = ThreadDispatcherShutdownMode.None;
                    this.PreRunQueue.MoveTo(this.Queue);

                    System.Threading.SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);

                    this.FinishedEvent.Reset();
                    this.FinishedSignals.Clear();

                    this.WatchdogLoop = new WatchdogThread(this);
                    this.WatchdogLoop.Timeout = SimpleDispatcher.WatchdogThreadTimeoutMilliseconds;
                    this.WatchdogLoop.Start();
                }

                this.ExecuteFrame(null);
            }
            finally
            {
                lock (this.SyncRoot)
                {
                    this.WatchdogLoop?.Stop();
                    this.WatchdogLoop = null;

                    this.CancelHard(false);

                    this.IdleSignals?.ForEach(x => x.TrySetResult(null));

                    System.Threading.SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);

                    this.PreRunQueue?.Clear();
                    this.ShutdownMode = ThreadDispatcherShutdownMode.None;

                    this.KeepAlives?.Clear();
                    this.CurrentOperation?.Clear();
                    this.CurrentPriority?.Clear();
                    this.CurrentOptions?.Clear();
                    this.IdleSignals?.Clear();
                    this.Posted?.Close();
                    this.Queue?.Clear();

                    this.KeepAlives = null;
                    this.CurrentOperation = null;
                    this.CurrentPriority = null;
                    this.CurrentOptions = null;
                    this.IdleSignals = null;
                    this.Posted = null;
                    this.Queue = null;
                    this.Thread = null;

                    this.FinishedEvent?.Set();

                    this.FinishedSignals?.ForEach(x => x.TrySetResult(null));
                    System.Threading.Thread.Sleep(SimpleDispatcher.SignalClearDelayMilliseconds);
                    this.FinishedSignals?.Clear();
                }
            }
        }

        private void CancelHard (bool includePreRunQueue)
        {
            this.CurrentOperation?.ForEach(x => x.CancelHard());
            this.Queue?.ForEach(x => x.CancelHard());

            if (includePreRunQueue)
            {
                this.PreRunQueue?.ForEach(x => x.CancelHard());
            }
        }

        private void EnqueueOperation (IThreadDispatcherOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            lock (this.SyncRoot)
            {
                this.VerifyRunning();

                this.AddKeepAlive(operation);

                this.Queue.Enqueue(operation, operation.Priority);
                this.Posted.Set();
            }
        }

        private void ExecuteFrame (IThreadDispatcherOperation returnTrigger)
        {
            while (true)
            {
                this.Posted.WaitOne();

                lock (this.SyncRoot)
                {
                    this.Posted.Reset();
                }

                while (true)
                {
                    SimpleDispatcherOperation operation = null;

                    lock (this.SyncRoot)
                    {
                        if (this.ShutdownMode == ThreadDispatcherShutdownMode.DiscardPending)
                        {
                            foreach (IThreadDispatcherOperation operationToCancel in this.Queue)
                            {
                                operationToCancel.Cancel();
                            }

                            this.Queue.Clear();
                            this.SignalIdle();
                            return;
                        }

                        if ((this.ShutdownMode == ThreadDispatcherShutdownMode.FinishPending) && (this.Queue.Count == 0))
                        {
                            this.SignalIdle();
                            return;
                        }

                        if (this.Queue.Count > 0)
                        {
                            operation = this.Queue.Dequeue();
                            this.CurrentOperation.Push(operation);
                            this.CurrentPriority.Push(operation.Priority);
                            this.CurrentOptions.Push(operation.Options);
                        }
                        else
                        {
                            this.SignalIdle();
                        }
                    }

                    if (operation == null)
                    {
                        break;
                    }

                    //TODO: We should measure the runtime here
                    //TODO: Also measure active time

                    lock (this.SyncRoot)
                    {
                        this.WatchdogLoop.StartSurveillance(operation);
                    }

                    try
                    {
                        operation.Execute();
                    }
                    finally
                    {
                        lock (this.SyncRoot)
                        {
                            this.WatchdogLoop.StopSurveillance(operation);
                        }
                    }

                    bool catchExceptions;

                    lock (this.SyncRoot)
                    {
                        this.CurrentOptions.Pop();
                        this.CurrentPriority.Pop();
                        this.CurrentOperation.Pop();

                        catchExceptions = this.CatchExceptions;
                    }

                    if (operation.Exception != null)
                    {
                        //TODO: Add operation to event args
                        this.OnException(operation.Exception, catchExceptions);

                        if (!catchExceptions)
                        {
                            //TODO: Add operation to event args
                            throw new ThreadDispatcherException(operation.Exception);
                        }
                    }

                    if (object.ReferenceEquals(operation, returnTrigger))
                    {
                        lock (this.SyncRoot)
                        {
                            if (this.Queue.Count == 0)
                            {
                                this.SignalIdle();
                            }
                        }
                        return;
                    }
                }
            }
        }

        private void OnException (Exception exception, bool canContinue)
        {
            this.Exception?.Invoke(this, new ThreadDispatcherExceptionEventArgs(exception, canContinue));
        }

        private void OnWatchdog (TimeSpan timeout, IThreadDispatcherOperation currentOperation)
        {
            this.Watchdog?.Invoke(this, new ThreadDispatcherWatchdogEventArgs(timeout, currentOperation));
        }

        private void SignalIdle ()
        {
            //TODO: Add IdleEvent
            //TODO: Add Idle event
            this.IdleSignals?.ForEach(x => x.TrySetResult(null));
            this.IdleSignals?.Clear();
        }

        private void VerifyNotFromDispatcher (string methodName)
        {
            if (this.IsInThread())
            {
                throw new InvalidOperationException(methodName + " cannot be called from inside the dispatcher thread.");
            }
        }

        private void VerifyNotRunning ()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException(nameof(SimpleDispatcher) + " is already running.");
            }
        }

        private void VerifyNotShuttingDown ()
        {
            if (this.ShutdownMode != ThreadDispatcherShutdownMode.None)
            {
                throw new InvalidOperationException(nameof(SimpleDispatcher) + " is already shutting down.");
            }
        }

        private void VerifyRunning ()
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException(nameof(SimpleDispatcher) + " is not running.");
            }
        }

        #endregion




        #region Interface: IThreadDispatcher

        /// <inheritdoc />
        public bool CatchExceptions
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._catchExceptions;
                }
            }
            set
            {
                lock (this.SyncRoot)
                {
                    this._catchExceptions = value;
                }
            }
        }

        /// <inheritdoc />
        public ThreadDispatcherOptions DefaultOptions
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._defaultOptions;
                }
            }
            set
            {
                if (value == ThreadDispatcherOptions.Default)
                {
                    throw new ArgumentException($"Invalid default thread dispatcher option: {nameof(ThreadDispatcherOptions.Default)}.", nameof(value));
                }

                lock (this.SyncRoot)
                {
                    this._defaultOptions = value;
                }
            }
        }

        /// <inheritdoc />
        public int DefaultPriority
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._defaultPriority;
                }
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                lock (this.SyncRoot)
                {
                    this._defaultPriority = value;
                }
            }
        }

        /// <inheritdoc />
        bool ISynchronizeInvoke.InvokeRequired => !this.IsInThread();

        /// <inheritdoc />
        public bool IsRunning
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Thread != null;
                }
            }
        }

        /// <inheritdoc />
        public bool IsShuttingDown
        {
            get
            {
                lock (this.SyncRoot)
                {
                    if (this.Thread == null)
                    {
                        return false;
                    }

                    return this.ShutdownMode != ThreadDispatcherShutdownMode.None;
                }
            }
        }

        /// <inheritdoc />
        public ThreadDispatcherShutdownMode ShutdownMode
        {
            get
            {
                lock (this.SyncRoot)
                {
                    if (this.Thread == null)
                    {
                        return ThreadDispatcherShutdownMode.None;
                    }

                    return this._shutdownMode;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._shutdownMode = value;
                }
            }
        }

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public TimeSpan? WatchdogTimeout
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._watchdogTimeout;
                }
            }
            set
            {
                if (value.GetValueOrDefault(TimeSpan.Zero).Ticks <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                lock (this.SyncRoot)
                {
                    this._watchdogTimeout = value;
                }
            }
        }

        /// <inheritdoc />
        public event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

        /// <inheritdoc />
        public event EventHandler<ThreadDispatcherWatchdogEventArgs> Watchdog;

        /// <inheritdoc />
        public bool AddKeepAlive (object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            lock (this.SyncRoot)
            {
                if (this.KeepAlives == null)
                {
                    return false;
                }

                this.KeepAlives.Add(obj);
                return true;
            }
        }

        /// <inheritdoc />
        IAsyncResult ISynchronizeInvoke.BeginInvoke (Delegate method, object[] args)
        {
            return this.SendAsync(method, args);
        }

        /// <inheritdoc />
        public void BeginShutdown (bool finishPendingDelegates)
        {
            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown();

                this.ShutdownMode = finishPendingDelegates ? ThreadDispatcherShutdownMode.FinishPending : ThreadDispatcherShutdownMode.DiscardPending;
                this.Posted.Set();
            }
        }

        /// <inheritdoc />
        void IDisposable.Dispose ()
        {
            lock (this.SyncRoot)
            {
                if (this.IsRunning && (!this.IsShuttingDown))
                {
                    this.BeginShutdown(false);
                }

                this.CancelHard(true);

                this.KeepAlives?.Clear();
                this.Queue?.Clear();
                this.PreRunQueue?.Clear();
            }
        }

        /// <inheritdoc />
        public void DoProcessing (int priority)
        {
            if (priority < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            bool isInThread;
            IThreadDispatcherOperation operation = null;

            while (true)
            {
                lock (this.SyncRoot)
                {
                    if (operation == null)
                    {
                        this.VerifyRunning();
                    }
                    else if (!this.IsRunning)
                    {
                        return;
                    }

                    if ((this.Queue.Count == 0) && (this.CurrentOperation.Count == 0))
                    {
                        return;
                    }

                    if (this.Queue.HighestPriority < priority)
                    {
                        return;
                    }

                    isInThread = this.IsInThread();
                    operation = this.Post(null, priority, ThreadDispatcherOptions.None, new Action(() => { }));
                }

                if (isInThread)
                {
                    this.ExecuteFrame(operation);
                }
                else
                {
                    operation.Wait();
                }
            }
        }

        /// <inheritdoc />
        public Task DoProcessingAsync (int priority)
        {
            if (priority < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            lock (this.SyncRoot)
            {
                this.VerifyRunning();

                Task waitTask = this.SendAsync(null, priority, ThreadDispatcherOptions.None, new Func<Task>(async () =>
                {
                    while (true)
                    {
                        IThreadDispatcherOperation operation;

                        lock (this.SyncRoot)
                        {
                            if (!this.IsRunning)
                            {
                                return;
                            }

                            if ((this.Queue.Count == 0) && (this.CurrentOperation.Count == 0))
                            {
                                return;
                            }

                            if (this.Queue.HighestPriority < priority)
                            {
                                return;
                            }

                            operation = this.Post(priority, ThreadDispatcherOptions.None, new Action(() => { }));
                        }

                        await operation.WaitAsync();
                    }
                }));

                return waitTask;
            }
        }

        /// <inheritdoc />
        object ISynchronizeInvoke.EndInvoke (IAsyncResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            Task<object> task = result as Task<object>;

            if (task == null)
            {
                throw new ArgumentException($"Async result is not Task<object>; it is {result.GetType().Name}", nameof(result));
            }

            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                //TODO: What is this ?!?!
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        /// <inheritdoc />
        public ThreadDispatcherOptions? GetCurrentOptions ()
        {
            lock (this.SyncRoot)
            {
                if (!this.IsInThread())
                {
                    return null;
                }

                if (this.CurrentOptions == null)
                {
                    return null;
                }

                if (this.CurrentOptions.Count == 0)
                {
                    return null;
                }

                return this.CurrentOptions.Peek();
            }
        }

        /// <inheritdoc />
        public int? GetCurrentPriority ()
        {
            lock (this.SyncRoot)
            {
                if (!this.IsInThread())
                {
                    return null;
                }

                if (this.CurrentPriority == null)
                {
                    return null;
                }

                if (this.CurrentPriority.Count == 0)
                {
                    return null;
                }

                return this.CurrentPriority.Peek();
            }
        }

        /// <inheritdoc />
        object ISynchronizeInvoke.Invoke (Delegate method, object[] args) => this.Send(method, args);

        /// <inheritdoc />
        public bool IsInThread ()
        {
            lock (this.SyncRoot)
            {
                if (this.Thread == null)
                {
                    return false;
                }

                return this.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;
            }
        }

        /// <inheritdoc />
        public IThreadDispatcherOperation Post (ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (priority < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            lock (this.SyncRoot)
            {
                //TODO: We should allow it, with a separate shutdown mode
                this.VerifyNotShuttingDown();

                priority = (priority == -1) ? this.DefaultPriority : priority;
                options = (options == ThreadDispatcherOptions.Default) ? this.DefaultOptions : options;
                parameters ??= new object[0];

                SimpleDispatcherOperation operation = new SimpleDispatcherOperation(this, executionContext, priority, options, action, parameters);

                if (this.IsRunning)
                {
                    this.EnqueueOperation(operation);
                }
                else
                {
                    this.PreRunQueue.Enqueue(operation, priority);
                }

                return operation;
            }
        }

        /// <inheritdoc />
        public bool RemoveKeepAlive (object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            lock (this.SyncRoot)
            {
                if (this.KeepAlives == null)
                {
                    return false;
                }

                return this.KeepAlives.Remove(obj);
            }
        }

        /// <inheritdoc />
        public object Send (ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (priority < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            bool result;
            bool isInThread;
            IThreadDispatcherOperation operation;

            lock (this.SyncRoot)
            {
                //TODO: We should allow it, with a separate shutdown mode
                this.VerifyRunning();
                this.VerifyNotShuttingDown();

                parameters ??= new object[0];
                isInThread = this.IsInThread();
                operation = this.Post(executionContext, priority, options, action, parameters);
            }

            if (isInThread)
            {
                //TODO: Use passed cancellation token and timeout (add to interface)
                this.ExecuteFrame(operation);
                result = true;
            }
            else
            {
                //TODO: Use passed cancellation token and timeout (add to interface)
                result = operation.Wait(Timeout.Infinite, CancellationToken.None);
            }

            if ((!result) && ((operation.State != ThreadDispatcherOperationState.Canceled) && (operation.State != ThreadDispatcherOperationState.Aborted)))
            {
                throw new TimeoutException();
            }

            if (operation.Exception != null)
            {
                throw new ThreadDispatcherException(operation.Exception);
            }

            if ((operation.State == ThreadDispatcherOperationState.Canceled) || (operation.State == ThreadDispatcherOperationState.Aborted))
            {
                throw new OperationCanceledException();
            }

            return operation.Result;
        }

        /// <inheritdoc />
        public Task<object> SendAsync (ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (priority < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            IThreadDispatcherOperation operation;

            lock (this.SyncRoot)
            {
                //TODO: We should allow it, with a separate shutdown mode
                this.VerifyRunning();
                this.VerifyNotShuttingDown();

                parameters ??= new object[0];
                operation = this.Post(executionContext, priority, options, action, parameters);
            }

            //TODO: Use passed cancellation token and timeout (add to interface)
            Task<bool> waitTask = operation.WaitAsync(Timeout.Infinite, CancellationToken.None);
            Task<object> resultTask = waitTask.ContinueWith(x =>
            {
                if((!x.Result) && ((operation.State != ThreadDispatcherOperationState.Canceled) && (operation.State != ThreadDispatcherOperationState.Aborted)))
                {
                    throw new TimeoutException();
                }

                if (operation.Exception != null)
                {
                    throw new ThreadDispatcherException(operation.Exception);
                }

                if ((operation.State == ThreadDispatcherOperationState.Canceled) || (operation.State == ThreadDispatcherOperationState.Aborted))
                {
                    throw new OperationCanceledException();
                }

                return operation.Result;
            }, CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, this.Scheduler);

            return resultTask;
        }

        /// <inheritdoc />
        public IThreadDispatcherTimer PostDelayed (ThreadDispatcherTimerMode mode, int milliseconds, ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (milliseconds < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }

            if (priority < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown();

                priority = (priority == -1) ? this.DefaultPriority : priority;
                options = (options == ThreadDispatcherOptions.Default) ? this.DefaultOptions : options;
                parameters ??= new object[0];

                SimpleDispatcherTimer timer = new SimpleDispatcherTimer(this, mode, milliseconds, executionContext, priority, options, action, parameters);

                return timer;
            }
        }

        /// <inheritdoc />
        public void Shutdown (bool finishPendingDelegates)
        {
            ManualResetEvent finishedEvent;

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown();
                this.VerifyNotFromDispatcher(nameof(this.Shutdown));

                finishedEvent = this.FinishedEvent;

                this.BeginShutdown(finishPendingDelegates);
            }

            finishedEvent.WaitOne();
        }

        /// <inheritdoc />
        public Task ShutdownAsync (bool finishPendingDelegates)
        {
            Task finishTask;

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown();
                this.VerifyNotFromDispatcher(nameof(this.ShutdownAsync));

                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                this.FinishedSignals.Add(tcs);
                finishTask = tcs.Task;

                this.BeginShutdown(finishPendingDelegates);
            }

            return finishTask;
        }

        #endregion




        #region Type: WatchdogThread

        private sealed class WatchdogThread : HeavyThread
        {
            #region Instance Constructor/Destructor

            public WatchdogThread (SimpleDispatcher dispatcher)
            {
                if (dispatcher == null)
                {
                    throw new ArgumentNullException(nameof(dispatcher));
                }

                this.Dispatcher = dispatcher;

                this.Operations = new Stack<WatchdogThreadItem>();
            }

            #endregion




            #region Instance Properties/Indexer

            public ThreadDispatcher Dispatcher { get; }

            private Stack<WatchdogThreadItem> Operations { get; }

            #endregion




            #region Instance Methods

            public void StartSurveillance (SimpleDispatcherOperation operation)
            {
                if (operation == null)
                {
                    throw new ArgumentNullException(nameof(operation));
                }

                lock (this.SyncRoot)
                {
                    this.Operations.Push(new WatchdogThreadItem(operation));
                }
            }

            public void StopSurveillance (SimpleDispatcherOperation operation)
            {
                if (operation == null)
                {
                    throw new ArgumentNullException(nameof(operation));
                }

                lock (this.SyncRoot)
                {
                    if ((this.Operations.Count == 0) || (!object.ReferenceEquals(operation, this.Operations.Peek().Operation)))
                    {
                        throw new ThreadDispatcherException("Watchdog operation surveillance stack is out of sync.");
                    }

                    this.Operations.Pop();
                }
            }

            #endregion




            #region Overrides

            protected override void Dispose (bool disposing)
            {
                this.Operations?.Clear();

                base.Dispose(disposing);
            }

            protected override void OnException (Exception exception, bool canContinue)
            {
                base.OnException(exception, canContinue);

                this.Dispatcher.Post<Exception>(int.MaxValue, ThreadDispatcherOptions.None, x => { throw new HeavyThreadException(x); }, exception);
            }

            protected override void OnRun ()
            {
                base.OnRun();

                while (!this.StopRequested)
                {
                    Thread.Sleep(SimpleDispatcher.WatchdogCheckInterval);

                    WatchdogThreadItem item;
                    TimeSpan timeout;

                    lock (this.SyncRoot)
                    {
                        if ((this.Operations.Count == 0) || (!this.Dispatcher.WatchdogTimeout.HasValue))
                        {
                            continue;
                        }

                        item = this.Operations.Peek();
                        timeout = this.Dispatcher.WatchdogTimeout.Value;
                    }

                    DateTime now = DateTime.UtcNow;
                    TimeSpan runTimeThisLoop = now.Subtract(item.LastCheck);
                    item.LastCheck = now;

                    SimpleDispatcherOperation operation = item.Operation;
                    bool hasTimeout = false;

                    lock (operation.SyncRoot)
                    {
                        //TODO: I don't think that the runtime is measured correctly...
                        double runTime = operation.RunTimeMillisecondsInternal + runTimeThisLoop.TotalMilliseconds;
                        double watchdogTime = operation.WatchdogTimeMillisecondsInternal + runTimeThisLoop.TotalMilliseconds;

                        operation.RunTimeMillisecondsInternal = runTime;
                        operation.WatchdogTimeMillisecondsInternal = watchdogTime;

                        if (watchdogTime > timeout.TotalMilliseconds)
                        {
                            hasTimeout = true;
                            operation.WatchdogTimeMillisecondsInternal = 0.0;
                            operation.WatchdogEventsInternal += 1;
                        }
                    }

                    if (hasTimeout)
                    {
                        this.Dispatcher.OnWatchdog(timeout, operation);
                    }
                }
            }

            protected override void OnStarting()
            {
                base.OnStarting();

                this.Thread.CurrentCulture = CultureInfo.InvariantCulture;
                this.Thread.CurrentUICulture = CultureInfo.InvariantCulture;
                this.Thread.Priority = ThreadPriority.Highest;
                this.Thread.IsBackground = false;
            }

            #endregion
        }

        #endregion




        #region Type: WatchdogThreadItem

        private sealed class WatchdogThreadItem
        {
            #region Instance Constructor/Destructor

            public WatchdogThreadItem (SimpleDispatcherOperation operation)
            {
                if (operation == null)
                {
                    throw new ArgumentNullException(nameof(operation));
                }

                this.Operation = operation;

                this.LastCheck = DateTime.UtcNow;
            }

            #endregion




            #region Instance Properties/Indexer

            public DateTime LastCheck { get; set; }

            public SimpleDispatcherOperation Operation { get; }

            #endregion
        }

        #endregion
    }
}
