using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Utilities;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Simple thread dispatcher abstraction implementation with basic thread dispatching functionality.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A <see cref="SimpleDispatcher" /> provides a queue for delegates which is processed on the thread where <see cref="Run" /> is called (<see cref="Run" /> blocks while executing the queue until <see cref="Shutdown" />, <see cref="ShutdownAsync"/>, or <see cref="BeginShutdown"/> is called).
    ///     </para>
    ///     <para>
    ///         The delegates are executed in the order they are added to the queue.
    ///         When all delegates are executed, or the queue is empty respectively, <see cref="SimpleDispatcher" /> waits for new delegates to process.
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
        ///         The default value is zero (lowest priority).
        ///     </para>
        /// </remarks>
        public const int DefaultPriorityValue = 0;

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

            this.SynchronizationContext = null;
            this.TaskScheduler = null;
            this.Thread = null;
            this.Queue = null;
            this.Posted = null;
            this.IdleSignals = null;
            this.CurrentPriority = null;
            this.CurrentOptions = null;
            this.CurrentOperation = null;
            this.KeepAlives = new HashSet<object>();
            this.WatchdogLoop = null;

            this.PreRunQueue = new PriorityQueue<SimpleDispatcherOperation>();

            this.FinishedEvent = new ManualResetEvent(false);
            this.FinishedSignals = new List<TaskCompletionSource<object>>();

            this.IdleEvent = new ManualResetEvent(false);
            this.IdleSignals = new List<TaskCompletionSource<object>>();
        }

        /// <summary>
        ///     Garbage collects this instance of <see cref="SimpleDispatcher" />.
        /// </summary>
        ~SimpleDispatcher()
        {
            ((IDisposable)this).Dispose();

            this.IdleSignals?.ForEach(x => x.TrySetResult(null));
            System.Threading.Thread.Sleep(SimpleDispatcher.SignalClearDelayMilliseconds);
            this.IdleSignals?.Clear();

            this.IdleEvent?.Set();
            System.Threading.Thread.Sleep(SimpleDispatcher.EventCloseDelayMilliseconds);
            this.IdleEvent?.Close();

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

        private Stack<SimpleDispatcherOperation> CurrentOperation { get; set; }

        private Stack<ThreadDispatcherOptions> CurrentOptions { get; set; }

        private Stack<int> CurrentPriority { get; set; }

        private ManualResetEvent FinishedEvent { get; }

        private ManualResetEvent IdleEvent { get; }

        private List<TaskCompletionSource<object>> FinishedSignals { get; }

        private List<TaskCompletionSource<object>> IdleSignals { get; }

        private HashSet<object> KeepAlives { get; set; }

        private ManualResetEvent Posted { get; set; }

        private PriorityQueue<SimpleDispatcherOperation> PreRunQueue { get; set; }

        private PriorityQueue<SimpleDispatcherOperation> Queue { get; set; }

        private Thread Thread { get; set; }

        private WatchdogThread WatchdogLoop { get; set; }

        internal ThreadDispatcherTaskScheduler TaskScheduler { get; set; }

        private ThreadDispatcherSynchronizationContext SynchronizationContext { get; set; }

        #endregion




        #region Instance Methods

        /// <inheritdoc />
        public void Run ()
        {
            SynchronizationContext synchronizationContextBackup = System.Threading.SynchronizationContext.Current;

            try
            {
                lock (this.SyncRoot)
                {
                    this.VerifyNotRunning();

                    synchronizationContextBackup = System.Threading.SynchronizationContext.Current;

                    this.SynchronizationContext = new ThreadDispatcherSynchronizationContext(this);
                    this.TaskScheduler = new ThreadDispatcherTaskScheduler(this);

                    this.Thread = Thread.CurrentThread;
                    this.Queue = new PriorityQueue<SimpleDispatcherOperation>();
                    this.Posted = new ManualResetEvent(this.PreRunQueue.Count > 0);
                    this.CurrentPriority = new Stack<int>();
                    this.CurrentOptions = new Stack<ThreadDispatcherOptions>();
                    this.CurrentOperation = new Stack<SimpleDispatcherOperation>();

                    this.ShutdownMode = ThreadDispatcherShutdownMode.None;
                    this.PreRunQueue.MoveTo(this.Queue);

                    System.Threading.SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);

                    this.FinishedEvent.Reset();
                    this.FinishedSignals.Clear();

                    this.IdleEvent.Reset();
                    this.IdleSignals.Clear();

                    this.WatchdogLoop = new WatchdogThread(this, SimpleDispatcher.WatchdogThreadTimeoutMilliseconds);
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

                    System.Threading.SynchronizationContext.SetSynchronizationContext(synchronizationContextBackup);

                    this.KeepAlives?.Clear();
                    this.PreRunQueue?.Clear();
                    this.ShutdownMode = ThreadDispatcherShutdownMode.None;

                    this.CurrentOperation?.Clear();
                    this.CurrentPriority?.Clear();
                    this.CurrentOptions?.Clear();
                    this.IdleSignals?.Clear();
                    this.Posted?.Close();
                    this.Queue?.Clear();

                    this.CurrentOperation = null;
                    this.CurrentPriority = null;
                    this.CurrentOptions = null;
                    this.Posted = null;
                    this.Queue = null;
                    this.Thread = null;

                    this.IdleEvent?.Set();
                    this.IdleSignals?.ForEach(x => x.TrySetResult(null));
                    System.Threading.Thread.Sleep(SimpleDispatcher.SignalClearDelayMilliseconds);
                    this.IdleSignals?.Clear();

                    this.FinishedEvent?.Set();
                    this.FinishedSignals?.ForEach(x => x.TrySetResult(null));
                    System.Threading.Thread.Sleep(SimpleDispatcher.SignalClearDelayMilliseconds);
                    this.FinishedSignals?.Clear();

                    this.SynchronizationContext = null;
                    this.TaskScheduler = null;
                }
            }
        }

        private void CancelHard (bool includePreRunQueue)
        {
            if (this.CurrentOperation != null)
            {
                foreach (SimpleDispatcherOperation op in this.CurrentOperation)
                {
                    op.CancelHard();
                }
            }

            if (this.Queue != null)
            {
                foreach (SimpleDispatcherOperation op in this.Queue)
                {
                    op.CancelHard();
                }
            }

            if (includePreRunQueue)
            {
                if (this.PreRunQueue != null)
                {
                    foreach (SimpleDispatcherOperation op in this.PreRunQueue)
                    {
                        op.CancelHard();
                    }
                }
            }
        }

        internal void EnqueueOperation (SimpleDispatcherOperation operation)
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

                    bool exit = false;
                    bool signalIdle = false;

                    lock (this.SyncRoot)
                    {
                        if (this.ShutdownMode == ThreadDispatcherShutdownMode.DiscardPending)
                        {
                            foreach (IThreadDispatcherOperation operationToCancel in this.Queue)
                            {
                                operationToCancel.Cancel();
                            }

                            this.Queue.Clear();

                            signalIdle = true;
                            exit = true;
                        }
                        else if ((this.ShutdownMode == ThreadDispatcherShutdownMode.FinishPending) && (this.Queue.Count == 0))
                        {
                            signalIdle = true;
                            exit = true;
                        }
                        else if ((this.ShutdownMode == ThreadDispatcherShutdownMode.AllowNew) && (this.Queue.Count == 0))
                        {
                            signalIdle = true;
                            exit = true;
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
                            signalIdle = true;
                        }
                    }

                    if (signalIdle)
                    {
                        signalIdle = false;
                        this.SignalIdle();
                    }

                    if (exit)
                    {
                        return;
                    }

                    if (operation == null)
                    {
                        break;
                    }

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
                        this.OnException(operation.Exception, catchExceptions, operation);

                        if (!catchExceptions)
                        {
                            throw new ThreadDispatcherException(operation.Exception, operation);
                        }
                    }

                    if (object.ReferenceEquals(operation, returnTrigger))
                    {
                        lock (this.SyncRoot)
                        {
                            if (this.Queue.Count == 0)
                            {
                                signalIdle = true;
                            }
                        }

                        if (signalIdle)
                        {
                            this.SignalIdle();
                        }

                        return;
                    }
                }
            }
        }

        private void OnException (Exception exception, bool canContinue, IThreadDispatcherOperation currentOperation)
        {
            this.Exception?.Invoke(this, new ThreadDispatcherExceptionEventArgs(exception, canContinue, currentOperation));
        }

        private void OnWatchdog (TimeSpan timeout, IThreadDispatcherOperation currentOperation)
        {
            this.Watchdog?.Invoke(this, new ThreadDispatcherWatchdogEventArgs(timeout, currentOperation));
        }

        private void OnIdle ()
        {
            this.Idle?.Invoke(this, new ThreadDispatcherIdleEventArgs(this));
        }

        private void SignalIdle ()
        {
            this.IdleEvent?.Set();

            this.IdleSignals?.ForEach(x => x.TrySetResult(null));
            System.Threading.Thread.Sleep(SimpleDispatcher.SignalClearDelayMilliseconds);
            this.IdleSignals?.Clear();

            this.OnIdle();
        }

        internal void VerifyNotFromDispatcher (string methodName)
        {
            if (this.IsInThread())
            {
                throw new InvalidOperationException(methodName + " cannot be called from inside the dispatcher thread.");
            }
        }

        internal void VerifyNotRunning ()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException(nameof(SimpleDispatcher) + " is already running.");
            }
        }

        internal void VerifyNotShuttingDown (ThreadDispatcherShutdownMode requestedMode)
        {
            if (this.ShutdownMode == ThreadDispatcherShutdownMode.None)
            {
                return;
            }

            bool throwException;

            if (requestedMode == ThreadDispatcherShutdownMode.None)
            {
                throwException = true;
            }
            else if (requestedMode == ThreadDispatcherShutdownMode.AllowNew)
            {
                throwException = false;
            }
            else
            {
                throwException = true;
            }

            if (throwException)
            {
                throw new InvalidOperationException(nameof(SimpleDispatcher) + " is already shutting down.");
            }
        }

        internal void VerifyShuttingDown()
        {
            if (this.ShutdownMode == ThreadDispatcherShutdownMode.None)
            {
                throw new InvalidOperationException(nameof(SimpleDispatcher) + " is not shutting down.");
            }
        }

        internal void VerifyRunning ()
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
                if (value.HasValue)
                {
                    if (value.Value.Ticks < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }
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
        public event EventHandler<ThreadDispatcherIdleEventArgs> Idle;

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
        public void BeginShutdown (ThreadDispatcherShutdownMode shutdownMode)
        {
            if (shutdownMode == ThreadDispatcherShutdownMode.None)
            {
                throw new ArgumentException($"The shutdown mode cannot be {nameof(ThreadDispatcherShutdownMode.None)}.", nameof(shutdownMode));
            }

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown(ThreadDispatcherShutdownMode.None);

                this.ShutdownMode = shutdownMode;
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
                    this.BeginShutdown(ThreadDispatcherShutdownMode.DiscardPending);
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

            lock (this.SyncRoot)
            {
                this.VerifyRunning();

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

        /// <inheritdoc />
        public Task DoProcessingAsync (int priority)
        {
            if (priority < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotFromDispatcher(nameof(this.ShutdownAsync));

                if ((this.Queue.Count == 0) && (this.CurrentOperation.Count == 0))
                {
                    return Task.CompletedTask;
                }

                if (this.Queue.HighestPriority < priority)
                {
                    return Task.CompletedTask;
                }

                this.Post(null, priority, ThreadDispatcherOptions.None, new Action(() =>
                {
                    tcs.SetResult(null);
                }));
            }

            return tcs.Task;
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

            task.Wait();
            return task.Result;
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
        public void WaitForShutdown ()
        {
            Task task;
            lock (this.SyncRoot)
            {
                this.VerifyNotFromDispatcher(nameof(WaitForShutdown));
                task = this.WaitForShutdownAsync();
            }
            
            task.Wait();
        }

        /// <inheritdoc />
        public Task WaitForShutdownAsync ()
        {
            TaskCompletionSource<object> tcs = null;

            lock (this.SyncRoot)
            {
                this.VerifyNotFromDispatcher(nameof(WaitForShutdownAsync));

                if (!this.IsRunning)
                {
                    return Task.CompletedTask;
                }

                if (!this.IsShuttingDown)
                {
                    this.VerifyShuttingDown();
                }

                tcs = new TaskCompletionSource<object>();
                this.FinishedSignals.Add(tcs);
            }

            return tcs.Task;
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
                this.VerifyNotShuttingDown(this.ShutdownMode);

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
        public object Send (ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, TimeSpan timeout, CancellationToken ct, Delegate action, params object[] parameters)
        {
            if (priority < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            if ((timeout.Ticks < 0) && (timeout != Timeout.InfiniteTimeSpan))
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
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
                this.VerifyRunning();
                this.VerifyNotShuttingDown(this.ShutdownMode);

                parameters ??= new object[0];
                isInThread = this.IsInThread();
                operation = this.Post(executionContext, priority, options, action, parameters);
            }

            if (isInThread)
            {
                this.ExecuteFrame(operation);
                result = true;
            }
            else
            {
                result = operation.Wait(timeout, ct);
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
        public Task<object> SendAsync (ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, TimeSpan timeout, CancellationToken ct, Delegate action, params object[] parameters)
        {
            if (priority < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            if ((timeout.Ticks < 0) && (timeout != Timeout.InfiniteTimeSpan))
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            IThreadDispatcherOperation operation;

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown(this.ShutdownMode);
                this.VerifyNotFromDispatcher(nameof(this.SendAsync));

                parameters ??= new object[0];
                operation = this.Post(executionContext, priority, options, action, parameters);
            }

            Task<bool> waitTask = operation.WaitAsync(timeout, ct);
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
            }, CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, this.TaskScheduler);

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
                this.VerifyNotShuttingDown(ThreadDispatcherShutdownMode.None);

                priority = (priority == -1) ? this.DefaultPriority : priority;
                options = (options == ThreadDispatcherOptions.Default) ? this.DefaultOptions : options;
                parameters ??= new object[0];

                SimpleDispatcherTimer timer = new SimpleDispatcherTimer(this, mode, TimeSpan.FromMilliseconds(milliseconds), executionContext, priority, options, action, parameters);

                return timer;
            }
        }

        /// <inheritdoc />
        public void Shutdown (ThreadDispatcherShutdownMode shutdownMode)
        {
            ManualResetEvent finishedEvent;

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown(ThreadDispatcherShutdownMode.None);
                this.VerifyNotFromDispatcher(nameof(this.Shutdown));

                finishedEvent = this.FinishedEvent;

                this.BeginShutdown(shutdownMode);
            }

            finishedEvent.WaitOne();
        }

        /// <inheritdoc />
        public Task ShutdownAsync (ThreadDispatcherShutdownMode shutdownMode)
        {
            Task finishTask;

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown(ThreadDispatcherShutdownMode.None);
                this.VerifyNotFromDispatcher(nameof(this.ShutdownAsync));

                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                this.FinishedSignals.Add(tcs);
                finishTask = tcs.Task;

                this.BeginShutdown(shutdownMode);
            }

            return finishTask;
        }

        #endregion




        #region Type: WatchdogThread

        private sealed class WatchdogThread : IDisposable
        {
            #region Instance Constructor/Destructor

            public WatchdogThread (SimpleDispatcher dispatcher, int timeoutMilliseconds)
            {
                if (dispatcher == null)
                {
                    throw new ArgumentNullException(nameof(dispatcher));
                }

                if (timeoutMilliseconds < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));
                }

                this.SyncRoot = new object();

                this.Dispatcher = dispatcher;
                this.TimeoutMilliseconds = timeoutMilliseconds;

                this.Operations = new Stack<WatchdogThreadItem>();
                this.Thread = null;
                this.StopRequested = false;
            }

            #endregion




            #region Instance Properties/Indexer

            public SimpleDispatcher Dispatcher { get; }

            private Stack<WatchdogThreadItem> Operations { get; }

            public object SyncRoot { get; }

            public int TimeoutMilliseconds { get; }

            private Thread Thread { get; set; }

            private bool StopRequested { get; set; }

            #endregion




            #region Instance Methods

            public void Start ()
            {
                lock (this.SyncRoot)
                {
                    if (this.Thread != null)
                    {
                        return;
                    }

                    this.StopRequested = false;

                    this.Thread = new Thread(() =>
                    {
                        while (!this.StopRequested)
                        {
                            Thread.Sleep(SimpleDispatcher.WatchdogCheckInterval);

                            WatchdogThreadItem item;
                            TimeSpan? timeout = this.Dispatcher.WatchdogTimeout;

                            lock (this.SyncRoot)
                            {
                                if ((this.Operations.Count == 0) || (!timeout.HasValue))
                                {
                                    continue;
                                }

                                item = this.Operations.Peek();
                            }

                            DateTime now = DateTime.UtcNow;
                            TimeSpan watchdogTime = now.Subtract(item.WatchdogStart);

                            SimpleDispatcherOperation operation = item.Operation;
                            bool hasTimeout = false;

                            lock (operation.SyncRoot)
                            {
                                if (operation.WatchdogEvents > 0)
                                {
                                    operation.WatchdogTime = watchdogTime;
                                }

                                if (watchdogTime > timeout.Value)
                                {
                                    hasTimeout = true;
                                    operation.WatchdogEvents += 1;
                                    operation.WatchdogTime = TimeSpan.Zero;
                                    item.WatchdogStart = now;
                                }
                            }

                            if (hasTimeout)
                            {
                                this.Dispatcher.OnWatchdog(timeout.Value, operation);
                            }
                        }
                    });

                    this.Thread.CurrentCulture = CultureInfo.InvariantCulture;
                    this.Thread.CurrentUICulture = CultureInfo.InvariantCulture;
                    this.Thread.Priority = ThreadPriority.Highest;
                    this.Thread.IsBackground = false;
                    this.Thread.Name = nameof(WatchdogThread);
                    
                    this.Thread.Start();
                }
            }

            public void Stop ()
            {
                lock (this.SyncRoot)
                {
                    this.StopRequested = true;

                    this.Thread?.Join(SimpleDispatcher.WatchdogThreadTimeoutMilliseconds);
                    this.Thread = null;
                }
            }

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
            
            void IDisposable.Dispose () => this.Stop();
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

                DateTime now = DateTime.UtcNow;
                this.OperationStart = now;
                this.WatchdogStart = now;
            }

            #endregion




            #region Instance Properties/Indexer

            public DateTime OperationStart { get; }

            public DateTime WatchdogStart { get; set; }

            public SimpleDispatcherOperation Operation { get; }

            #endregion
        }

        #endregion
    }
}
