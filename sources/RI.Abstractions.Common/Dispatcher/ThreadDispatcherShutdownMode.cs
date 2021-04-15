using System;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Describes the current shutdown mode of a thread dispatcher.
    /// </summary>
    [Serializable]
    public enum ThreadDispatcherShutdownMode
    {
        /// <summary>
        ///     The dispatcher is not running or currently not being shut down.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The dispatcher is being shut down and all already pending delegates are discarded.
        /// </summary>
        DiscardPending = 1,

        /// <summary>
        ///     The dispatcher is being shut down and all already pending delegates are processed before the shutdown completes.
        /// </summary>
        FinishPending = 2,

        /// <summary>
        ///     The dispatcher is being shut down and, already pending delegates are processed before the shutdown completes, and new delegates can still be enqueued.
        /// </summary>
        AllowNew = 3,
    }
}
