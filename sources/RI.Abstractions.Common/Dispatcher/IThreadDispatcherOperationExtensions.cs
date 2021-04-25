using System;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IThreadDispatcherOperation" /> type.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public static class IThreadDispatcherOperationExtensions
    {
        /// <summary>
        ///     Gets whether the thread dispatcher operation has finished processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <returns>
        ///     true if <see cref="IThreadDispatcherOperation.State" /> is anything else than <see cref="ThreadDispatcherOperationState.Waiting" /> or <see cref="ThreadDispatcherOperationState.Executing" />, false if <see cref="IThreadDispatcherOperation.State" /> is <see cref="ThreadDispatcherOperationState.Waiting" /> or <see cref="ThreadDispatcherOperationState.Executing" />.
        /// </returns>
        public static bool IsDone (this IThreadDispatcherOperation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            lock (op.SyncRoot)
            {
                return (op.State != ThreadDispatcherOperationState.Waiting) && (op.State != ThreadDispatcherOperationState.Waiting);
            }
        }

        /// <summary>
        ///     Waits indefinitely for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        public static void Wait(this IThreadDispatcherOperation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            op.Wait(Timeout.InfiniteTimeSpan, CancellationToken.None);
        }

        /// <summary>
        ///     Waits indefinitely for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing, false if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public static bool Wait(this IThreadDispatcherOperation op, CancellationToken cancellationToken)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.Wait(Timeout.InfiniteTimeSpan, cancellationToken);
        }

        /// <summary>
        ///     Waits a specified amount of time for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <param name="timeout"> The maximum time to wait for the thread dispatcher operation to finish processing before the method returns. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing within the specified timeout, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        public static bool Wait(this IThreadDispatcherOperation op, TimeSpan timeout)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.Wait(timeout, CancellationToken.None);
        }

        /// <summary>
        ///     Waits a specified amount of time for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <param name="milliseconds"> The maximum time in milliseconds to wait for the thread dispatcher operation to finish processing before the method returns. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing within the specified timeout, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        public static bool Wait(this IThreadDispatcherOperation op, int milliseconds)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.Wait(TimeSpan.FromMilliseconds(milliseconds), CancellationToken.None);
        }

        /// <summary>
        ///     Waits a specified amount of time for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <param name="milliseconds"> The maximum time in milliseconds to wait for the thread dispatcher operation to finish processing before the method returns. </param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing within the specified timeout, false otherwise or if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public static bool Wait(this IThreadDispatcherOperation op, int milliseconds, CancellationToken cancellationToken)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.Wait(TimeSpan.FromMilliseconds(milliseconds), cancellationToken);
        }

        /// <summary>
        ///     Waits indefinitely for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <returns>
        ///     The task which can be used to await the finish of the processing.
        /// </returns>
        public static Task WaitAsync(this IThreadDispatcherOperation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.WaitAsync(Timeout.InfiniteTimeSpan, CancellationToken.None);
        }

        /// <summary>
        ///     Waits indefinitely for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing, false if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public static Task<bool> WaitAsync(this IThreadDispatcherOperation op, CancellationToken cancellationToken)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.WaitAsync(Timeout.InfiniteTimeSpan, cancellationToken);
        }

        /// <summary>
        ///     Waits a specified amount of time for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <param name="timeout"> The maximum time to wait for the thread dispatcher operation to finish processing before the method returns. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing within the specified timeout, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        public static Task<bool> WaitAsync(this IThreadDispatcherOperation op, TimeSpan timeout)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.WaitAsync(timeout, CancellationToken.None);
        }

        /// <summary>
        ///     Waits a specified amount of time for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <param name="milliseconds"> The maximum time in milliseconds to wait for the thread dispatcher operation to finish processing before the method returns. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing within the specified timeout, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        public static Task<bool> WaitAsync(this IThreadDispatcherOperation op, int milliseconds)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.WaitAsync(TimeSpan.FromMilliseconds(milliseconds), CancellationToken.None);
        }

        /// <summary>
        ///     Waits a specified amount of time for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="op">The thread dispatcher.</param>
        /// <param name="milliseconds"> The maximum time in milliseconds to wait for the thread dispatcher operation to finish processing before the method returns. </param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing within the specified timeout, false otherwise or if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public static Task<bool> WaitAsync(this IThreadDispatcherOperation op, int milliseconds, CancellationToken cancellationToken)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            return op.WaitAsync(TimeSpan.FromMilliseconds(milliseconds), cancellationToken);
        }
    }
}
