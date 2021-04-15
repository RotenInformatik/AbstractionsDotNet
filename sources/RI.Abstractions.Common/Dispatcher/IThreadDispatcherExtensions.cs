using System;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IThreadDispatcher" /> type.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public static class IThreadDispatcherExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />) and default options (<see cref="IThreadDispatcher.DefaultOptions" />).
        ///     </para>
        ///     <para>
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread.
        ///     </para>
        /// </remarks>

        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
        public static IThreadDispatcherOperation Post (this IThreadDispatcher dispatcher, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.Post(null, -1, ThreadDispatcherOptions.Default, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <para>
        ///         The delegate is enqueued with the default options (<see cref="IThreadDispatcher.DefaultOptions" />).
        ///     </para>
        ///     <para>
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
        public static IThreadDispatcherOperation Post(this IThreadDispatcher dispatcher, int priority, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.Post(null, priority, ThreadDispatcherOptions.Default, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />).
        ///     </para>
        ///     <para>
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
        public static IThreadDispatcherOperation Post(this IThreadDispatcher dispatcher, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.Post(null, -1, options, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delagate. </param>
        /// <returns>
        ///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <para>
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
        public static IThreadDispatcherOperation Post(this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.Post(null, priority, options, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        ///     <remarks>
        ///     <para>
        ///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />) and default options (<see cref="IThreadDispatcher.DefaultOptions" />).
        ///     </para>
        ///     </remarks>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread and can be therefore be cascaded.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
        public static object Send(this IThreadDispatcher dispatcher, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.Send(null, -1, ThreadDispatcherOptions.Default, Timeout.InfiniteTimeSpan, CancellationToken.None, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        ///     <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <para>
        ///         The delegate is enqueued with the default options (<see cref="IThreadDispatcher.DefaultOptions" />).
        ///     </para>
        ///     </remarks>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread and can be therefore be cascaded.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        public static object Send(this IThreadDispatcher dispatcher, int priority, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.Send(null, priority, ThreadDispatcherOptions.Default, Timeout.InfiniteTimeSpan, CancellationToken.None, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        ///     <remarks>
        ///     <para>
        ///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />).
        ///     </para>
        ///     </remarks>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread and can be therefore be cascaded.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        public static object Send(this IThreadDispatcher dispatcher, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.Send(null, -1, options, Timeout.InfiniteTimeSpan, CancellationToken.None, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        ///     <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     </remarks>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread and can be therefore be cascaded.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        public static object Send(this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.Send(null, priority, options, Timeout.InfiniteTimeSpan, CancellationToken.None, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />) and default options (<see cref="IThreadDispatcher.DefaultOptions" />).
        ///     </para>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread and can be therefore be cascaded.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        public static Task<object> SendAsync(this IThreadDispatcher dispatcher, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.SendAsync(null, -1, ThreadDispatcherOptions.Default, Timeout.InfiniteTimeSpan, CancellationToken.None, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <para>
        ///         The delegate is enqueued with the default options (<see cref="IThreadDispatcher.DefaultOptions" />).
        ///     </para>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread and can be therefore be cascaded.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        public static Task<object> SendAsync(this IThreadDispatcher dispatcher, int priority, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.SendAsync(null, priority, ThreadDispatcherOptions.Default, Timeout.InfiniteTimeSpan, CancellationToken.None, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />).
        ///     </para>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread and can be therefore be cascaded.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        public static Task<object> SendAsync(this IThreadDispatcher dispatcher, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.SendAsync(null, -1, options, Timeout.InfiniteTimeSpan, CancellationToken.None, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The return value of the executed delegate or null if the delegate has no return value.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <para>
        ///         Blocks until the scheduled delegate and all previously enqueued delegates of higher or same priority were processed.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread and can be therefore be cascaded.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        public static Task<object> SendAsync(this IThreadDispatcher dispatcher, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return dispatcher.SendAsync(null, priority, options, Timeout.InfiniteTimeSpan, CancellationToken.None, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue for delayed or repeated execution.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="mode"> The mode under which the associated timer operates. </param>
        /// <param name="milliseconds"> The delay of the execution in milliseconds until the first execution. </param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher timer created for the delayed or repeated execution.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />) and default options (<see cref="IThreadDispatcher.DefaultOptions" />).
        ///     </para>
        ///     <para>
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is zero or negative is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or being shut down. </exception>
        public static IThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, ThreadDispatcherTimerMode mode, int milliseconds, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return dispatcher.PostDelayed(mode, milliseconds, null, -1, ThreadDispatcherOptions.Default, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue for delayed or repeated execution.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="mode"> The mode under which the associated timer operates. </param>
        /// <param name="milliseconds"> The delay of the execution in milliseconds until the first execution. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher timer created for the delayed or repeated execution.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <para>
        ///         The delegate is enqueued with the default options (<see cref="IThreadDispatcher.DefaultOptions" />).
        ///     </para>
        ///     <para>
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is zero or negative or <paramref name="priority" />  is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or being shut down. </exception>
        public static IThreadDispatcherTimer PostDelayed(this IThreadDispatcher dispatcher, ThreadDispatcherTimerMode mode, int milliseconds, int priority, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return dispatcher.PostDelayed(mode, milliseconds, null, priority, ThreadDispatcherOptions.Default, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue for delayed or repeated execution.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="mode"> The mode under which the associated timer operates. </param>
        /// <param name="milliseconds"> The delay of the execution in milliseconds until the first execution. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher timer created for the delayed or repeated execution.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />).
        ///     </para>
        ///     <para>
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is zero or negative or is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or being shut down. </exception>
        public static IThreadDispatcherTimer PostDelayed(this IThreadDispatcher dispatcher, ThreadDispatcherTimerMode mode, int milliseconds, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return dispatcher.PostDelayed(mode, milliseconds, null, -1, options, action, parameters);
        }

        /// <summary>
        ///     Enqueues a delegate to the dispatchers queue for delayed or repeated execution.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="mode"> The mode under which the associated timer operates. </param>
        /// <param name="milliseconds"> The delay of the execution in milliseconds until the first execution. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher timer created for the delayed or repeated execution.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <para>
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </para>
        ///     <para>
        ///         Callable from the dispatcher thread.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is zero or negative or <paramref name="priority" />  is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or being shut down. </exception>
        public static IThreadDispatcherTimer PostDelayed(this IThreadDispatcher dispatcher, ThreadDispatcherTimerMode mode, int milliseconds, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return dispatcher.PostDelayed(mode, milliseconds, null, priority, options, action, parameters);
        }

        /// <inheritdoc cref="CreateAwaiter"/>
        public static ThreadDispatcherAwaiter GetAwaiter(this IThreadDispatcher dispatcher) => dispatcher.CreateAwaiter();

        /// <summary>
        ///     Creates an awaiter for a dispatcher.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher to use in the awaiter. </param>
        /// <returns>
        ///     The created awaiter or null if the dispatcher is not running.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public static ThreadDispatcherAwaiter CreateAwaiter (this IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            lock (dispatcher.SyncRoot)
            {
                if (!dispatcher.IsRunning)
                {
                    return null;
                }

                return new ThreadDispatcherAwaiter(dispatcher);
            }
        }

        /// <summary>
        ///     Creates a synchronization context for a dispatcher.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <returns>
        ///     The created synchronization context or null if the dispatcher is not running.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public static SynchronizationContext CreateSynchronizationContext(this IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            lock (dispatcher.SyncRoot)
            {
                if (!dispatcher.IsRunning)
                {
                    return null;
                }

                return new ThreadDispatcherSynchronizationContext(dispatcher);
            }
        }

        /// <summary>
        ///     Creates a task scheduler for a dispatcher.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <returns>
        ///     The created task scheduler or null if the dispatcher is not running.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public static TaskScheduler CreateTaskScheduler(this IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            lock (dispatcher.SyncRoot)
            {
                if (!dispatcher.IsRunning)
                {
                    return null;
                }

                return new ThreadDispatcherTaskScheduler(dispatcher);
            }
        }

        /// <summary>
        ///     Creates a task factory for a dispatcher.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <returns>
        ///     The created task factory or null if the dispatcher is not running.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public static TaskFactory CreateTaskFactory(this IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            lock (dispatcher.SyncRoot)
            {
                if (!dispatcher.IsRunning)
                {
                    return null;
                }

                return new ThreadDispatcherTaskFactory(dispatcher);
            }
        }

        /// <summary>
        ///     Determines under which options the current code is executed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <returns>
        ///     The options of the currently executed code or the default options of the dispatcher if the current code is not executed by the dispatcher.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
        public static ThreadDispatcherOptions? GetCurrentOptionsOrDefault (this IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            lock (dispatcher.SyncRoot)
            {
                return dispatcher.GetCurrentOptions() ?? dispatcher.DefaultOptions;
            }
        }

        /// <summary>
        ///     Determines under which options the current code is executed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="defaultOptions"> The default options to return if the current code is not executed by the dispatcher. </param>
        /// <returns>
        ///     The options of the currently executed code or <paramref name="defaultOptions" /> if the current code is not executed by the dispatcher.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
        public static ThreadDispatcherOptions? GetCurrentOptionsOrDefault (this IThreadDispatcher dispatcher, ThreadDispatcherOptions? defaultOptions)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            lock (dispatcher.SyncRoot)
            {
                return dispatcher.GetCurrentOptions() ?? defaultOptions;
            }
        }

        /// <summary>
        ///     Determines under which priority the current code is executed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <returns>
        ///     The priority of the currently executed code or the default priority of the dispatcher if the current code is not executed by the dispatcher.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
        public static int? GetCurrentPriorityOrDefault (this IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            lock (dispatcher.SyncRoot)
            {
                return dispatcher.GetCurrentPriority() ?? dispatcher.DefaultPriority;
            }
        }

        /// <summary>
        ///     Determines under which priority the current code is executed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="defaultPriority"> The default priority to return if the current code is not executed by the dispatcher. </param>
        /// <returns>
        ///     The priority of the currently executed code or <paramref name="defaultPriority" /> if the current code is not executed by the dispatcher.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="defaultPriority" /> is less than zero. </exception>
        public static int? GetCurrentPriorityOrDefault (this IThreadDispatcher dispatcher, int? defaultPriority)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (defaultPriority < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(defaultPriority));
            }

            lock (dispatcher.SyncRoot)
            {
                return dispatcher.GetCurrentPriority() ?? defaultPriority;
            }
        }

        /// <summary>
        ///     Waits until all queued operations have been processed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running. </exception>
        public static void DoProcessing(this IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            dispatcher.DoProcessing(0);
        }



        /// <summary>
        ///     Waits until all queued operations have been processed.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <returns>
        ///     The task which can be used to await the completion of the processing.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running. </exception>
        public static Task DoProcessingAsync(this IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return dispatcher.DoProcessingAsync(0);
        }

        #endregion
    }
}
