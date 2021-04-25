using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Implements an awaiter which moves execution to a specified <see cref="IThreadDispatcher" />.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class ThreadDispatcherAwaiter : ICriticalNotifyCompletion
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadDispatcherAwaiter" />.
        /// </summary>
        /// <param name="dispatcher"> The used <see cref="IThreadDispatcher" />. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public ThreadDispatcherAwaiter (IThreadDispatcher dispatcher)
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
        ///     Gets the used <see cref="IThreadDispatcher" />.
        /// </summary>
        /// <value>
        ///     The used <see cref="IThreadDispatcher" />.
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

        /// <summary>
        ///     Gets the awaiter as required by the compiler.
        /// </summary>
        /// <returns>
        ///     The awaiter, which is nothing else than this instance itself.
        /// </returns>
        public ThreadDispatcherAwaiter GetAwaiter() => this;




        #region Overrides

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public void OnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            this.Dispatcher.Post(null, this.Dispatcher.GetCurrentPriorityOrDefault(this.Dispatcher.DefaultPriority).Value, this.Dispatcher.GetCurrentOptionsOrDefault(this.Dispatcher.DefaultOptions).Value, continuation);
        }

        /// <summary>
        ///     Gets the result of the awaiter when completed.
        /// </summary>
        public void GetResult()
        {
        }

        /// <summary>
        ///     Gets whether the awaiter is already completed.
        /// </summary>
        /// <value>
        ///     true if the awaiter is already completed, false otherwise.
        /// </value>
        public bool IsCompleted => false;

        /// <inheritdoc cref="OnCompleted" />
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => this.OnCompleted(continuation);

        #endregion
    }
}
