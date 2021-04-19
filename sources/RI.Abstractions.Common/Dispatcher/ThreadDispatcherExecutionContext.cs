using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Represents the execution context under which a delegate is executed by a thread dispatcher.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class ThreadDispatcherExecutionContext : ICloneable, IDisposable
    {
        private bool _isDisposed;




        #region Static Methods

        /// <summary>
        ///     Captures the current execution context to later use it to execute a delegate.
        /// </summary>
        /// <param name="options"> The used execution options. Cannot be <see cref="ThreadDispatcherOptions.Default"/>.</param>
        /// <returns>
        ///     The captured execution context.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="options"/> is <see cref="ThreadDispatcherOptions.Default"/>.</exception>
        public static ThreadDispatcherExecutionContext Capture (ThreadDispatcherOptions options)
        {
            if (options == ThreadDispatcherOptions.Default)
            {
                throw new ArgumentException($"Thread dispatcher options cannot be set to {ThreadDispatcherOptions.Default} when using {nameof(ThreadDispatcherExecutionContext)}", nameof(options));
            }

            ThreadDispatcherExecutionContext executionContext = new ThreadDispatcherExecutionContext();

            executionContext.CaptureExecutionContext = (options & ThreadDispatcherOptions.CaptureExecutionContext) == ThreadDispatcherOptions.CaptureExecutionContext;
            executionContext.CaptureSynchronizationContext = (options & ThreadDispatcherOptions.CaptureSynchronizationContext) == ThreadDispatcherOptions.CaptureSynchronizationContext;
            executionContext.CaptureCurrentCulture = (options & ThreadDispatcherOptions.CaptureCurrentCulture) == ThreadDispatcherOptions.CaptureCurrentCulture;
            executionContext.CaptureCurrentUICulture = (options & ThreadDispatcherOptions.CaptureCurrentUICulture) == ThreadDispatcherOptions.CaptureCurrentUICulture;


            executionContext.ExecutionContext = executionContext.CaptureExecutionContext ? ExecutionContext.Capture() : null;
            executionContext.SynchronizationContext = executionContext.CaptureSynchronizationContext ? SynchronizationContext.Current : null;
            executionContext.CurrentCulture = executionContext.CaptureCurrentCulture ? CultureInfo.CurrentCulture : null;
            executionContext.CurrentUICulture = executionContext.CaptureCurrentUICulture ? CultureInfo.CurrentUICulture : null;

            return executionContext;
        }

        #endregion




        #region Instance Constructor/Destructor

        private ThreadDispatcherExecutionContext ()
        {
            this.SyncRoot = new object();
            this.IsDisposed = false;

            this.CaptureExecutionContext = false;
            this.CaptureSynchronizationContext = false;
            this.CaptureCurrentCulture = false;
            this.CaptureCurrentUICulture = false;

            this.ExecutionContext = null;
            this.SynchronizationContext = null;
            this.CurrentCulture = null;
            this.CurrentUICulture = null;

            this.Options = ThreadDispatcherOptions.None;
            this.Action = null;
            this.Parameters = null;
            this.Result = null;
        }

        /// <summary>
        ///     Garbage collects this instance of <see cref="ThreadDispatcherExecutionContext" />.
        /// </summary>
        ~ThreadDispatcherExecutionContext ()
        {
            this.Dispose();
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        /// Gets whether this execution context is already disposed.
        /// </summary>
        /// <value>
        /// true if this execution context is already disposed and can no longer be used, false otherwise.
        /// </value>
        public bool IsDisposed
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._isDisposed;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._isDisposed = value;
                }
            }
        }

        private Delegate Action { get; set; }
        private CultureInfo CurrentCulture { get; set; }
        private CultureInfo CurrentUICulture { get; set; }
        private ExecutionContext ExecutionContext { get; set; }
        private bool CaptureCurrentCulture { get; set; }
        private bool CaptureCurrentUICulture { get; set; }
        private bool CaptureExecutionContext { get; set; }
        private bool CaptureSynchronizationContext { get; set; }
        private ThreadDispatcherOptions Options { get; set; }
        private object[] Parameters { get; set; }
        private object Result { get; set; }
        private SynchronizationContext SynchronizationContext { get; set; }

        #endregion




        #region Instance Methods

        /// <summary>
        /// Executes a delegate in the captured execution context.
        /// </summary>
        /// <param name="options"> The used execution options. Cannot be <see cref="ThreadDispatcherOptions.Default"/>.</param>
        /// <param name="action"> The delegate. Cannot be null.</param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        /// The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is <see cref="ThreadDispatcherOptions.Default"/>.</exception>
        /// <exception cref="ObjectDisposedException">This instance has already been disposed and can no longer be used.</exception>
        public object Run (ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (options == ThreadDispatcherOptions.Default)
            {
                throw new ArgumentException($"Thread dispatcher options cannot be set to {ThreadDispatcherOptions.Default} when using {nameof(ThreadDispatcherExecutionContext)}", nameof(options));
            }

            ThreadDispatcherExecutionContext clone;

            lock (this.SyncRoot)
            {
                this.VerifyNotDisposed();

                clone = this.Clone();
            }

            using (clone)
            {
                return clone.RunInternal(options, action, parameters);
            }
        }

        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        private void RunCore ()
        {
            bool captureCurrentCulture = ((this.Options & ThreadDispatcherOptions.CaptureCurrentCulture) == ThreadDispatcherOptions.CaptureCurrentCulture) && this.CaptureCurrentCulture;
            bool captureCurrentUICulture = ((this.Options & ThreadDispatcherOptions.CaptureCurrentUICulture) == ThreadDispatcherOptions.CaptureCurrentUICulture) && this.CaptureCurrentUICulture;
            bool captureSynchronizationContext = ((this.Options & ThreadDispatcherOptions.CaptureSynchronizationContext) == ThreadDispatcherOptions.CaptureSynchronizationContext) && this.CaptureSynchronizationContext;

            CultureInfo currentCultureBackup = captureCurrentCulture ? CultureInfo.CurrentCulture : null;
            CultureInfo currentUICultureBackup = captureCurrentUICulture ? CultureInfo.CurrentUICulture : null;
            SynchronizationContext synchronizationContextBackup = captureSynchronizationContext ? SynchronizationContext.Current : null;

            try
            {
                if (captureCurrentCulture)
                {
                    CultureInfo.CurrentCulture = this.CurrentCulture;
                }

                if (captureCurrentUICulture)
                {
                    CultureInfo.CurrentUICulture = this.CurrentUICulture;
                }

                if (captureSynchronizationContext)
                {
                    SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);
                }

                this.Result = this.Action.DynamicInvoke(this.Parameters);
            }
            finally
            {
                try
                {
                    if (captureSynchronizationContext)
                    {
                        SynchronizationContext.SetSynchronizationContext(synchronizationContextBackup);
                    }
                }
                catch
                {
                }

                try
                {
                    if (captureCurrentUICulture)
                    {
                        CultureInfo.CurrentUICulture = currentUICultureBackup;
                    }
                }
                catch
                {
                }

                try
                {
                    if (captureCurrentCulture)
                    {
                        CultureInfo.CurrentCulture = currentCultureBackup;
                    }
                }
                catch
                {
                }
            }
        }

        private object RunInternal (ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            this.Options = options;
            this.Action = action;
            this.Parameters = parameters;
            this.Result = null;

            bool captureExecutionContext = ((options & ThreadDispatcherOptions.CaptureExecutionContext) == ThreadDispatcherOptions.CaptureExecutionContext) && this.CaptureExecutionContext;
            if (captureExecutionContext)
            {
                ExecutionContext.Run(this.ExecutionContext, state => { ((ThreadDispatcherExecutionContext)state).RunCore(); }, this);
            }
            else
            {
                this.RunCore();
            }

            return this.Result;
        }

        private void VerifyNotDisposed ()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(ThreadDispatcherExecutionContext));
            }
        }

        #endregion




        #region Interface: ICloneable<ThreadDispatcherExecutionContext>

        /// <inheritdoc cref="IDisposable.Dispose" />
        /// <exception cref="ObjectDisposedException">This instance has already been disposed and can no longer be used.</exception>
        public ThreadDispatcherExecutionContext Clone ()
        {
            lock (this.SyncRoot)
            {
                this.VerifyNotDisposed();

                ThreadDispatcherExecutionContext clone = new ThreadDispatcherExecutionContext();

                clone.CaptureExecutionContext = this.CaptureExecutionContext;
                clone.CaptureSynchronizationContext = this.CaptureSynchronizationContext;
                clone.CaptureCurrentCulture = this.CaptureCurrentCulture;
                clone.CaptureCurrentUICulture = this.CaptureCurrentUICulture;

                clone.ExecutionContext = this.ExecutionContext?.CreateCopy();
                clone.SynchronizationContext = this.SynchronizationContext;
                clone.CurrentCulture = (CultureInfo)this.CurrentCulture?.Clone() ?? this.CurrentCulture;
                clone.CurrentUICulture = (CultureInfo)this.CurrentUICulture?.Clone() ?? this.CurrentUICulture;

                return clone;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ObjectDisposedException">This instance has already been disposed and can no longer be used.</exception>
        object ICloneable.Clone ()
        {
            return this.Clone();
        }

        #endregion




        #region Interface: IDisposable

        /// <inheritdoc />
        public void Dispose ()
        {
            lock (this.SyncRoot)
            {
                this.IsDisposed = true;

                this.ExecutionContext?.Dispose();
                this.ExecutionContext = null;

                this.CaptureExecutionContext = false;
                this.CaptureSynchronizationContext = false;
                this.CaptureCurrentCulture = false;
                this.CaptureCurrentUICulture = false;

                this.SynchronizationContext = null;
                this.CurrentCulture = null;
                this.CurrentUICulture = null;

                this.Options = ThreadDispatcherOptions.None;
                this.Action = null;
                this.Parameters = null;
                this.Result = null;
            }
        }

        #endregion




        #region Interface: ISynchronizable



        /// <summary>
        ///     Gets the object which can be used for thread synchronization when accessing <see cref="ThreadDispatcherExecutionContext"/>.
        /// </summary>
        /// <value>
        ///     The object which can be used for thread synchronization.
        /// </value>
        public object SyncRoot { get; }

        #endregion
    }
}
