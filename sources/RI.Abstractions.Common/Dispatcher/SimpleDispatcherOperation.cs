using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Abstractions.Dispatcher
{
    internal sealed class SimpleDispatcherOperation : IThreadDispatcherOperation
    {
        #region Instance Constructor/Destructor

        internal SimpleDispatcherOperation(SimpleDispatcher dispatcher, ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (priority < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.SyncRoot = new object();

            this.ExecutionContext = executionContext?.Clone() ?? ThreadDispatcherExecutionContext.Capture(this.Options);

            this.Dispatcher = dispatcher;
            this.Priority = priority;
            this.Options = options;
            this.Action = action;
            this.Parameters = parameters;

            this.Stage = 0;
            this.Task = null;

            this.State = ThreadDispatcherOperationState.Waiting;
            this.Result = null;
            this.Exception = null;

            this.OperationDoneEvent = new ManualResetEvent(false);
            this.OperationDoneTask = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            this.Dispatcher.AddKeepAlive(this);

            this.WatchdogEvents = 0;
            this.WatchdogTime = null;
            this.Dispatched = DateTime.UtcNow;
            this.FirstExecution = null;
            this.LastExecution = null;
        }

        ~SimpleDispatcherOperation()
        {
            this.Dispatcher?.RemoveKeepAlive(this);

            this.OperationDoneEvent?.Close();

            this.ExecutionContext?.Dispose();
        }

        #endregion



        private static object GlobalSyncRoot { get; } = new object();

        private static Dictionary<Type, MethodInfo> ResultGetterMethods { get; } = new Dictionary<Type, MethodInfo>();

        private static MethodInfo GetResultGetterMethod (Type type)
        {
            lock (SimpleDispatcherOperation.GlobalSyncRoot)
            {
                if (SimpleDispatcherOperation.ResultGetterMethods.ContainsKey(type))
                {
                    return SimpleDispatcherOperation.ResultGetterMethods[type];
                }

                PropertyInfo resultProperty = type.GetProperty(nameof(Task<object>.Result), BindingFlags.Instance | BindingFlags.Public);
                MethodInfo resultGetter = resultProperty?.GetGetMethod(false);
                SimpleDispatcherOperation.ResultGetterMethods.Add(type, resultGetter);
                return resultGetter;
            }
        }




        #region Instance Fields

        private Exception _exception;
        private object _result;
        private ThreadDispatcherOperationState _state;

        private int _watchdogEvents;

        private TimeSpan? _watchdogTime;

        private DateTime _dispatched;

        private DateTime? _firstExecution;

        private DateTime? _lastExecution;

        #endregion




        #region Instance Properties/Indexer

        public Delegate Action { get; }

        public Exception Exception
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._exception;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._exception = value;
                }
            }
        }

        public bool IsDone
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return (this.State != ThreadDispatcherOperationState.Waiting) && (this.State != ThreadDispatcherOperationState.Executing);
                }
            }
        }

        public object Result
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._result;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._result = value;
                }
            }
        }

        public ThreadDispatcherOperationState State
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._state;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._state = value;
                }
            }
        }

        public int WatchdogEvents
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._watchdogEvents;
                }
            }
            internal set
            {
                lock (this.SyncRoot)
                {
                    this._watchdogEvents = value;
                }
            }
        }

        public TimeSpan? WatchdogTime
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._watchdogTime;
                }
            }
            internal set
            {
                lock (this.SyncRoot)
                {
                    this._watchdogTime = value;
                }
            }
        }

        public DateTime Dispatched
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._dispatched;
                }
            }
            internal set
            {
                lock (this.SyncRoot)
                {
                    this._dispatched = value;
                }
            }
        }

        public DateTime? FirstExecution
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._firstExecution;
                }
            }
            internal set
            {
                lock (this.SyncRoot)
                {
                    this._firstExecution = value;
                }
            }
        }

        public DateTime? LastExecution
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._lastExecution;
                }
            }
            internal set
            {
                lock (this.SyncRoot)
                {
                    this._lastExecution = value;
                }
            }
        }

        public SimpleDispatcher Dispatcher { get; }

        IThreadDispatcher IThreadDispatcherRunnable.Dispatcher => this.Dispatcher;

        public ThreadDispatcherExecutionContext ExecutionContext { get; }

        public ThreadDispatcherOptions Options { get; }

        private object[] Parameters { get; }

        public object[] GetParameters () => this.Parameters.ToArray();

        public int Priority { get; }

        private ManualResetEvent OperationDoneEvent { get;}

        private TaskCompletionSource<object> OperationDoneTask { get; }

        private int Stage { get; set; }

        private Task Task { get; set; }

        #endregion




        #region Instance Methods

        public bool Cancel ()
        {
            return this.CancelInternal(false);
        }

        public bool Wait (TimeSpan timeout, CancellationToken cancellationToken)
        {
            if ((timeout.Ticks < 0) && (timeout != Timeout.InfiniteTimeSpan))
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            lock (this.SyncRoot)
            {
                this.Dispatcher.VerifyNotFromDispatcher(nameof(this.Wait));

                if (this.IsDone)
                {
                    return true;
                }
            }

            bool result = WaitHandle.WaitAny(new[] {cancellationToken.WaitHandle, this.OperationDoneEvent}, timeout) == 1;
            return result;
        }

        public Task<bool> WaitAsync (TimeSpan timeout, CancellationToken cancellationToken)
        {
            if ((timeout.Ticks < 0) && (timeout != Timeout.InfiniteTimeSpan))
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }



            lock (this.SyncRoot)
            {
                this.Dispatcher.VerifyNotFromDispatcher(nameof(this.WaitAsync));

                if (this.IsDone)
                {
                    return Task.FromResult(true);
                }
            }

            Task operationTask = this.OperationDoneTask.Task;
            Task timeoutTask = Task.Delay(timeout, cancellationToken);

            Task<Task> completed = Task.WhenAny(operationTask, timeoutTask);
            Task<bool> final = completed.ContinueWith(_ => object.ReferenceEquals(completed, operationTask), CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, this.Dispatcher.TaskScheduler);
            return final;
        }

        internal bool CancelHard ()
        {
            return this.CancelInternal(true);
        }

        internal void Execute ()
        {
            lock (this.SyncRoot)
            {
                if ((this.State != ThreadDispatcherOperationState.Waiting) && (this.Stage == 0))
                {
                    return;
                }

                if ((this.State != ThreadDispatcherOperationState.Executing) && (this.Stage != 0))
                {
                    return;
                }

                this.State = ThreadDispatcherOperationState.Executing;
            }

            bool finished = false;
            object result = null;
            Exception exception;
            bool canceled = false;

            try
            {
                finished = this.ExecuteCore(out result, out exception, out canceled);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            lock (this.SyncRoot)
            {
                if (canceled)
                {
                    this.Exception = null;
                    this.Result = null;
                    this.State = ThreadDispatcherOperationState.Canceled;

                    this.Dispatcher.RemoveKeepAlive(this);

                    this.OperationDoneEvent.Set();
                    this.OperationDoneTask.TrySetCanceled();
                }
                else if (exception != null)
                {
                    this.Exception = exception;
                    this.Result = null;
                    this.State = ThreadDispatcherOperationState.Exception;

                    this.Dispatcher.RemoveKeepAlive(this);

                    this.OperationDoneEvent.Set();
                    this.OperationDoneTask.TrySetException(this.Exception);
                }
                else if (finished)
                {
                    this.Exception = null;
                    this.Result = result;
                    this.State = ThreadDispatcherOperationState.Finished;

                    this.Dispatcher.RemoveKeepAlive(this);

                    this.OperationDoneEvent.Set();
                    this.OperationDoneTask.TrySetResult(this.Result);
                }
            }
        }

        private bool CancelInternal (bool hard)
        {
            lock (this.SyncRoot)
            {
                if (hard)
                {
                    if ((this.State != ThreadDispatcherOperationState.Waiting) && (this.State != ThreadDispatcherOperationState.Executing))
                    {
                        return false;
                    }
                }
                else
                {
                    if (this.State != ThreadDispatcherOperationState.Waiting)
                    {
                        return false;
                    }
                }

                this.Exception = null;
                this.Result = null;
                this.State = this.State == ThreadDispatcherOperationState.Executing ? ThreadDispatcherOperationState.Aborted : ThreadDispatcherOperationState.Canceled;

                this.Dispatcher.RemoveKeepAlive(this);

                this.OperationDoneEvent.Set();
                this.OperationDoneTask.TrySetCanceled();

                return true;
            }
        }

        private void EvaluateTask (out object result, out Exception exception, out bool canceled)
        {
            result = null;
            exception = this.Task.Exception;
            canceled = this.Task.IsCanceled;

            if (this.Task.Status == TaskStatus.RanToCompletion)
            {
                Type taskType = this.Task.GetType();
                MethodInfo resultGetter = GetResultGetterMethod(taskType);
                result = resultGetter?.Invoke(this.Task, null);
            }
        }

        private bool ExecuteCore (out object result, out Exception exception, out bool canceled)
        {
            if (this.Stage == 0)
            {
                Type returnType = this.Action.Method.ReturnType;
                bool isTask = typeof(Task).IsAssignableFrom(returnType);

                if (isTask)
                {
                    this.Task = (Task)this.ExecuteCoreAction();

                    if (this.Task.IsCompleted)
                    {
                        this.EvaluateTask(out result, out exception, out canceled);
                        return true;
                    }
                    else
                    {
                        this.Stage = 1;

                        this.Task.ContinueWith(_ => { this.Dispatcher.EnqueueOperation(this); }, CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, this.Dispatcher.TaskScheduler);

                        result = null;
                        exception = null;
                        canceled = false;
                        return false;
                    }
                }
                else
                {
                    result = this.ExecuteCoreAction();
                    exception = null;
                    canceled = false;
                    return true;
                }
            }

            if (this.Stage == 1)
            {
                this.EvaluateTask(out result, out exception, out canceled);
                return true;
            }

            throw new InvalidOperationException("Invalid stage: " + this.Stage);
        }

        private object ExecuteCoreAction ()
        {
            DateTime now = DateTime.UtcNow;
            this.LastExecution = now;

            if (!this.FirstExecution.HasValue)
            {
                this.FirstExecution = now;
            }

            if ((this.ExecutionContext != null) && (this.Options != ThreadDispatcherOptions.None))
            {
                return this.ExecutionContext.Run(this.Options, this.Action, this.Parameters);
            }
            else
            {
                return this.Action.DynamicInvoke(this.Parameters);
            }
        }

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        public object SyncRoot { get; }

        #endregion
    }
}
