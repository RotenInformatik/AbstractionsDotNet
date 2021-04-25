using System;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    /// An object which can be executed by a thread dispatcher.
    /// </summary>
    public interface IThreadDispatcherRunnable
    {
        /// <summary>
        ///     Gets the delegate executed by this thread dispatcher timer.
        /// </summary>
        /// <value>
        ///     The delegate executed by this thread dispatcher timer.
        /// </value>
        /// <remarks>
        /// <note type="implement">
        /// This property must never be null.
        /// </note>
        /// </remarks>
        Delegate Action { get; }

        /// <summary>
        ///     Gets the used dispatcher.
        /// </summary>
        /// <value>
        ///     The used dispatcher.
        /// </value>
        /// <remarks>
        /// <note type="implement">
        /// This property must never be null.
        /// </note>
        /// </remarks>
        IThreadDispatcher Dispatcher { get; }

        /// <summary>
        ///     Gets the used execution options.
        /// </summary>
        /// <value>
        ///     The used execution options.
        /// </value>
        ThreadDispatcherOptions Options { get; }

        /// <summary>
        ///     Gets the optional parameters of the delegate.
        /// </summary>
        /// <return>
        ///     The optional parameters of the delegate. If no parameters are used, an empty array is returned.
        /// </return>
        /// <remarks>
        /// <note type="implement">
        /// This method must never return null.
        /// </note>
        /// <note type="implement">
        /// This method must return a copy of the parameters array as the parameters must remain immutable.
        /// </note>
        /// </remarks>
        object[] GetParameters();

        /// <summary>
        ///     Gets the used execution priority.
        /// </summary>
        /// <value>
        ///     The used execution priority.
        /// </value>
        public int Priority { get; }

        /// <summary>
        ///     Gets the object which can be used for thread synchronization when accessing <see cref="IThreadDispatcherTimer"/>.
        /// </summary>
        /// <value>
        ///     The object which can be used for thread synchronization.
        /// </value>
        /// <remarks>
        /// <note type="implement">
        /// This property must never be null.
        /// </note>
        /// </remarks>
        object SyncRoot { get; }

        /// <summary>
        ///     Gets the used execution context.
        /// </summary>
        /// <value>
        ///     The used execution context.
        /// </value>
        /// <remarks>
        /// <note type="implement">
        /// This property must never be null.
        /// </note>
        /// </remarks>
        ThreadDispatcherExecutionContext ExecutionContext { get; }
    }
}
