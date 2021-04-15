using System;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Event arguments for the <see cref="IThreadDispatcher.Exception" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class ThreadDispatcherExceptionEventArgs : EventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadDispatcherExceptionEventArgs" />.
        /// </summary>
        /// <param name="exception"> The exception. </param>
        /// <param name="canContinue"> Indicates whether the thread is able to continue or not after the exception was handled. </param>
        /// <param name="currentOperation"> The current operation which threw the exception. </param>
        public ThreadDispatcherExceptionEventArgs (Exception exception, bool canContinue, IThreadDispatcherOperation currentOperation)
        {
            this.Exception = exception;
            this.CanContinue = canContinue;
            this.CurrentOperation = currentOperation;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets whether the thread is able to continue or not after the exception was handled.
        /// </summary>
        /// <value>
        ///     true if the thread is able to continue, false otherwise.
        /// </value>
        public bool CanContinue { get; }

        /// <summary>
        ///     Gets the exception.
        /// </summary>
        /// <value>
        ///     The exception.
        /// </value>
        public Exception Exception { get; }

        /// ///
        /// <summary>
        ///     Gets the current operation which threw the exception.
        /// </summary>
        /// <value>
        ///     The current operation which threw the exception.
        /// </value>
        public IThreadDispatcherOperation CurrentOperation { get; }

        #endregion
    }
}
