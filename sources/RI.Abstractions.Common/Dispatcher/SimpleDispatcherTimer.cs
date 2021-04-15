using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;




namespace RI.Abstractions.Dispatcher
{
    internal sealed class SimpleDispatcherTimer : IThreadDispatcherTimer, IDisposable
    {
        #region Instance Constructor/Destructor

        public SimpleDispatcherTimer(SimpleDispatcher dispatcher, ThreadDispatcherTimerMode mode, TimeSpan interval, ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (interval.Ticks < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
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

            this.Dispatcher = dispatcher;
            this.Mode = mode;
            this.Priority = priority;
            this.Options = options;
            this.Interval = interval;
            this.Action = action;
            this.Parameters = parameters;

            this.ExecutionContext = executionContext?.Clone() ?? ThreadDispatcherExecutionContext.Capture(this.Options);

            this.Timer = null;
            this.PreviousOperation = null;

            this.Dispatcher.AddKeepAlive(this);
        }

        ~SimpleDispatcherTimer()
        {
            this.Dispose(false);
        }

        #endregion




        #region Instance Fields

        private long _executionCount;

        private long _missCount;

        private TimeSpan _interval;

        #endregion




        #region Instance Properties/Indexer

        public Delegate Action { get; }

        public IThreadDispatcher Dispatcher { get; }

        public long ExecutionCount
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._executionCount;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._executionCount = value;
                }
            }
        }

        public TimeSpan Interval
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._interval;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._interval = value;
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Timer != null;
                }
            }
        }
        
        public long MissCount
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._missCount;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._missCount = value;
                }
            }
        }

        public ThreadDispatcherTimerMode Mode { get; }

        public ThreadDispatcherOptions Options { get; }

        private object[] Parameters { get; }

        public object[] GetParameters () => this.Parameters.ToArray();

        public int Priority { get; }

        public ThreadDispatcherExecutionContext ExecutionContext { get; }

        private IThreadDispatcherOperation PreviousOperation { get; set; }

        private Timer Timer { get; set; }

        #endregion




        #region Instance Methods

        public bool Start (TimeSpan interval)
        {
            lock (this.SyncRoot)
            {
                if (this.Timer != null)
                {
                    return false;
                }

                GC.ReRegisterForFinalize(this);

                this.ExecutionCount = 0;
                this.MissCount = 0;
                this.Interval = interval;

                this.Dispatcher.RemoveKeepAlive(this);
                this.Dispatcher.AddKeepAlive(this);

                this.Timer = new Timer(x =>
                {
                    SimpleDispatcherTimer self = (SimpleDispatcherTimer)x;
                    lock (self.SyncRoot)
                    {
                        if (self.Timer == null)
                        {
                            return;
                        }

                        if (self.PreviousOperation != null)
                        {
                            if (!self.PreviousOperation.IsDone())
                            {
                                self.MissCount++;
                                return;
                            }
                        }

                        bool isRunning;

                        lock (self.Dispatcher.SyncRoot)
                        {
                            isRunning = self.Dispatcher.IsRunning;
                            
                            if (isRunning)
                            {
                                self.PreviousOperation = self.Dispatcher.Post(self.ExecutionContext, self.Priority, self.Options, self.Action, self.Parameters);
                                self.ExecutionCount++;
                            }
                        }

                        if ((self.Mode == ThreadDispatcherTimerMode.OneShot) || (!isRunning))
                        {
                            self.Stop();
                        }
                    }
                }, this, (int)this.Interval.TotalMilliseconds, (int)(this.Mode == ThreadDispatcherTimerMode.OneShot ? -1 : this.Interval.TotalMilliseconds));

                return true;
            }
        }

        public bool Stop ()
        {
            lock (this.SyncRoot)
            {
                bool isRunning = this.IsRunning;
                this.Dispose(true);
                GC.SuppressFinalize(this);
                return isRunning;
            }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        private void Dispose (bool disposing)
        {
            lock (this.SyncRoot)
            {
                this.Dispatcher?.RemoveKeepAlive(this);

                this.Timer?.Dispose();
                this.Timer = null;

                this.ExecutionContext?.Dispose();
            }
        }

        #endregion




        #region Interface: IDisposable

        void IDisposable.Dispose ()
        {
            this.Stop();
        }

        #endregion




        #region Interface: ISynchronizable

        public object SyncRoot { get; }

        #endregion
    }
}
