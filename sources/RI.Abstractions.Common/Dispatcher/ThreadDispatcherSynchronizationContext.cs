using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     A <see cref="SynchronizationContext" /> which uses an <see cref="IThreadDispatcher" /> for execution.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Option to specify priority and options?
    public sealed class ThreadDispatcherSynchronizationContext : SynchronizationContext
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadDispatcherSynchronizationContext" />.
        /// </summary>
        /// <param name="dispatcher"> The used dispatcher. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public ThreadDispatcherSynchronizationContext (IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            this.SyncRoot = new object();
            this.Dispatcher = dispatcher;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used dispatcher.
        /// </summary>
        /// <value>
        ///     The used dispatcher.
        /// </value>
        public IThreadDispatcher Dispatcher { get; }

        /// <summary>
        ///     Gets the object which can be used for thread synchronization when accessing <see cref="IThreadDispatcherOperation"/>.
        /// </summary>
        /// <value>
        ///     The object which can be used for thread synchronization.
        /// </value>
        /// <remarks>
        /// <note type="implement">
        /// This property must never be null.
        /// </note>
        /// </remarks>
        public object SyncRoot { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override SynchronizationContext CreateCopy ()
        {
            return new ThreadDispatcherSynchronizationContext(this.Dispatcher);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public override void Post (SendOrPostCallback d, object state)
        {
            lock (this.SyncRoot)
            {
                this.Dispatcher.Post(null, this.Dispatcher.GetCurrentPriorityOrDefault(this.Dispatcher.DefaultPriority).Value, this.Dispatcher.GetCurrentOptionsOrDefault(this.Dispatcher.DefaultOptions).Value, new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
            }
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public override void Send (SendOrPostCallback d, object state)
        {
            lock (this.SyncRoot)
            {
                this.Dispatcher.Send(null, this.Dispatcher.GetCurrentPriorityOrDefault(this.Dispatcher.DefaultPriority).Value, this.Dispatcher.GetCurrentOptionsOrDefault(this.Dispatcher.DefaultOptions).Value, new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
            }
        }

        #endregion
    }
}
