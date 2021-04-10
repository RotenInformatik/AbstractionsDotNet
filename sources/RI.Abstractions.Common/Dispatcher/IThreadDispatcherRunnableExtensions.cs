using System;
using System.Threading.Tasks;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IThreadDispatcherRunnable" /> type.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public static class IThreadDispatcherRunnableExtensions
    {
        /// <summary>
        /// Manually enqueues the delegate immediately (without affecting the state of this object) to the associated thread dispatchers queue and does not wait for its execution.
        /// </summary>
        /// <param name="runnable">The thread dispatcher executable object.</param>
        /// <returns>
        ///     The dispatcher operation object which can be used to track and control the execution of the enqueued delegate.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The already specified execution context, priority, options, delegate, and parameters are used.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="runnable" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
        public static IThreadDispatcherOperation PostManually (this IThreadDispatcherRunnable runnable)
        {
            if (runnable == null)
            {
                throw new ArgumentNullException(nameof(runnable));
            }

            return runnable.Dispatcher.Post(runnable.ExecutionContext, runnable.Priority, runnable.Options, runnable.Action, runnable.GetParameters());
        }

        /// <summary>
        /// Manually enqueues the delegate immediately (without affecting the state of this object) to the associated thread dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="runnable">The thread dispatcher executable object.</param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The already specified execution context, priority, options, delegate, and parameters are used.
        /// </para>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="runnable" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
        public static object SendManually(this IThreadDispatcherRunnable runnable)
        {
            if (runnable == null)
            {
                throw new ArgumentNullException(nameof(runnable));
            }

            return runnable.Dispatcher.Send(runnable.ExecutionContext, runnable.Priority, runnable.Options, runnable.Action, runnable.GetParameters());
        }

        /// <summary>
        /// Manually enqueues the delegate immediately (without affecting the state of this object) to the associated thread dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="runnable">The thread dispatcher executable object.</param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The already specified execution context, priority, options, delegate, and parameters are used.
        /// </para>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="runnable" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
        public static Task<object> SendManuallyAsync(this IThreadDispatcherRunnable runnable)
        {
            if (runnable == null)
            {
                throw new ArgumentNullException(nameof(runnable));
            }

            return runnable.Dispatcher.SendAsync(runnable.ExecutionContext, runnable.Priority, runnable.Options, runnable.Action, runnable.GetParameters());
        }

        /// <summary>
        /// Executes the delegate immediately with completely circumventing the associated dispatcher.
        /// </summary>
        /// <param name="runnable">The thread dispatcher executable object.</param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The already specified delegate and parameters are used.
        /// </para>
        /// <para>
        /// The already specified execution context, priority, and options are ignored.
        /// </para>
        ///     <para>
        ///         Blocks until the delegate was executed.
        ///     </para>
        ///     <note type="note">
        ///         The delegate is executed on the thread which calls <see cref="ExecuteImmediately"/>.
        ///     </note>
        ///     <note type="note">
        ///         Any exception can be thrown, depending of the code behind the delegate.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="runnable" /> is null. </exception>
        public static object ExecuteImmediately (this IThreadDispatcherRunnable runnable)
        {
            if (runnable == null)
            {
                throw new ArgumentNullException(nameof(runnable));
            }

            return runnable.Action.DynamicInvoke(runnable.GetParameters());
        }
    }
}
