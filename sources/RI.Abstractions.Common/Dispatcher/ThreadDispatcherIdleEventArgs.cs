using System;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Event arguments for the <see cref="IThreadDispatcher.Idle" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class ThreadDispatcherIdleEventArgs : EventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadDispatcherExceptionEventArgs" />.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher which went idle. </param>
        public ThreadDispatcherIdleEventArgs(IThreadDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the dispatcher which went idle.
        /// </summary>
        /// <value>
        ///     The dispatcher which went idle.
        /// </value>
        public IThreadDispatcher Dispatcher { get; }

        #endregion
    }
}
