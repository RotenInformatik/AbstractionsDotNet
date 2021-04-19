using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Standalone thread-bound dispatcher abstraction.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A thread dispatcher provides a prioritized queue for delegates which is processed on a single specified thread.
    ///     </para>
    ///     <para>
    ///         The delegates are executed in the order of their priority and then in the order they were added to the queue.
    ///         When all delegates are executed, or the queue is empty respectively, the thread dispatcher waits for new delegates to process.
    ///     </para>
    ///     <para>
    ///         A watchdog can be used to ensure that the execution of a delegate does not block the dispatcher indefinitely.
    ///         The watchdog is active whenever <see cref="WatchdogTimeout" /> is not null.
    ///         The watchdog runs in a separate thread and raises the <see cref="Watchdog" /> event if the execution of a delegate takes longer than the specified timeout.
    ///     </para>
    ///     <para>
    ///         Whether <see cref="ExecutionContext" />, <see cref="SynchronizationContext"/>, and/or <see cref="CultureInfo" /> is captured from the dispatching thread onto the delegate execution thread (and used for executing a delegate), depends on the used <see cref="ThreadDispatcherOptions" />.
    ///     </para>
    /// </remarks>
    public interface IThreadDispatcher : ISynchronizeInvoke, IDisposable
    {
        /// <summary>
        ///     Gets or sets whether exceptions, thrown when executing delegates, are catched, continuing dispatcher operations if so.
        /// </summary>
        /// <value>
        ///     true if exceptions are catched, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value must be false.
        ///     </note>
        ///     <note type="implement">
        ///         The <see cref="Exception" /> event must be raised regardless of the value of <see cref="CatchExceptions" />.
        ///     </note>
        /// </remarks>
        bool CatchExceptions { get; set; }

        /// <summary>
        ///     Gets or sets the default options.
        /// </summary>
        /// <value>
        ///     The default options.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value must be <see cref="ThreadDispatcherOptions.None" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is <see cref="ThreadDispatcherOptions.Default"/>. </exception>
        ThreadDispatcherOptions DefaultOptions { get; set; }

        /// <summary>
        ///     Gets or sets the default priority.
        /// </summary>
        /// <value>
        ///     The default priority.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value should be <c> int.MaxValue / 2 </c>.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is less than zero. </exception>
        int DefaultPriority { get; set; }

        /// <summary>
        ///     Gets whether the dispatcher is running.
        /// </summary>
        /// <value>
        ///     true if the dispatcher is running, false otherwise.
        /// </value>
        bool IsRunning { get; }

        /// <summary>
        ///     Gets whether the dispatcher is shutting down.
        /// </summary>
        /// <value>
        ///     true if the dispatcher is shutting down, false otherwise.
        ///     false is also returned if the dispatcher is not running.
        /// </value>
        bool IsShuttingDown { get; }

        /// <summary>
        ///     Gets the active shutdown mode.
        /// </summary>
        /// <value>
        ///     The active shutdown mode or <see cref="ThreadDispatcherShutdownMode.None" /> if the dispatcher is not shutting down.
        ///     <see cref="ThreadDispatcherShutdownMode.None" /> is also returned if the dispatcher is not running.
        /// </value>
        ThreadDispatcherShutdownMode ShutdownMode { get; }

        /// <summary>
        ///     Gets or sets the watchdog timeout.
        /// </summary>
        /// <value>
        ///     The watchdog timeout or null if no watchdog is used.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException"> The value is negative. </exception>
        TimeSpan? WatchdogTimeout { get; set; }

        /// <summary>
        ///     Gets the object which can be used for thread synchronization when accessing <see cref="IThreadDispatcher"/>.
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
        ///     Raised when an exception occurred during execution of a delegate.
        /// </summary>
        /// <remarks>
        ///     <note type="implement">
        ///         The <see cref="Exception" /> event must raised from the thread <see cref="IThreadDispatcher" /> runs in.
        ///     </note>
        ///     <note type="implement">
        ///         The <see cref="Exception" /> event must be raised regardless of the value of <see cref="CatchExceptions" />.
        ///     </note>
        /// </remarks>
        event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

        /// <summary>
        ///     Raised when a watchdog was not reset within the timeout period.
        /// </summary>
        /// <remarks>
        ///     <note type="implement">
        ///         The <see cref="Watchdog"/> event must be raised from a separate watchdog thread.
        ///     </note>
        /// </remarks>
        event EventHandler<ThreadDispatcherWatchdogEventArgs> Watchdog;

        /// <summary>
        ///     Raised when the dispatcher becomes idle (the delegate queue is empty).
        /// </summary>
        /// <remarks>
        ///     <note type="implement">
        ///         The <see cref="Idle" /> event must raised from the thread <see cref="IThreadDispatcher" /> runs in.
        ///     </note>
        /// </remarks>
        event EventHandler<ThreadDispatcherIdleEventArgs> Idle;

        /// <summary>
        ///     Processes the delegate queue and waits for new delegates until <see cref="Shutdown" /> is called.
        /// </summary>
        /// <exception cref="InvalidOperationException"> The dispatcher is already running. </exception>
        /// <exception cref="ThreadDispatcherException"> The execution of a delegate has thrown an exception and <see cref="CatchExceptions" /> is false. </exception>
        void Run();

        /// <summary>
        ///     Adds an object to the list of objects which are kept alive at least as long as this thread dispatcher is running.
        /// </summary>
        /// <param name="obj"> The object to add to the keep-alive list. </param>
        /// <returns>
        ///     true if the thread dispatcher is running and the object was added to the keep-alive list, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The keep-alive list simply stores a reference to the specified object, preventing the garbage collector from collecting the object.
        ///         Therefore, the keep-alive list is useful for keeping objects which are needed later, e.g. during the processing of a delegate, but are not required anywhere else.
        ///         An example for this is a timer which needs to live as long as the timer is running, e.g. a one-shot timer where the timer instance itself (apart from its timer event) is not required anymore after it was created.
        ///     </para>
        ///     <note type="implement">
        ///         After the thread dispatcher is shut down, the keep-alive list must be cleared.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
        bool AddKeepAlive (object obj);

        /// <summary>
        ///     Removes an object from the list of objects which are kept alive at least as long as this thread dispatcher is running.
        /// </summary>
        /// <param name="obj"> The object to remove from the keep-alive list. </param>
        /// <returns>
        ///     true if the thread dispatcher is running and the object was removed from the keep-alive list, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="AddKeepAlive" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
        bool RemoveKeepAlive (object obj);

        /// <summary>
        ///     Stops processing the delegate queue but does not wait for its shutdown.
        /// </summary>
        /// <param name="shutdownMode"> Specifies the used shutdown mode. </param>
        /// <exception cref="ArgumentException"><paramref name="shutdownMode"/> is <see cref="ThreadDispatcherShutdownMode.None"/>.</exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or it is already being shut down. </exception>
        void BeginShutdown (ThreadDispatcherShutdownMode shutdownMode);

        /// <summary>
        ///     Stops processing the delegate queue and waits for its shutdown.
        /// </summary>
        /// <param name="shutdownMode"> Specifies the used shutdown mode. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="Shutdown" /> cannot be called from inside the dispatcher thread.
        ///         Use <see cref="BeginShutdown" /> from inside the dispatcher thread instead.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="shutdownMode"/> is <see cref="ThreadDispatcherShutdownMode.None"/>.</exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running, is already being shut down, or the method was called from the dispatcher thread itself. </exception>
        void Shutdown(ThreadDispatcherShutdownMode shutdownMode);

        /// <summary>
        ///     Stops processing the delegate queue and waits for its shutdown
        /// </summary>
        /// <param name="shutdownMode"> Specifies the used shutdown mode. </param>
        /// <returns>
        ///     The task which can be used to await the completion of the shutdown.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="ShutdownAsync" /> cannot be called from inside the dispatcher thread.
        ///         Use <see cref="BeginShutdown" /> from inside the dispatcher thread instead.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="shutdownMode"/> is <see cref="ThreadDispatcherShutdownMode.None"/>.</exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running, is already being shut down, or the method was called from the dispatcher thread itself. </exception>
        Task ShutdownAsync(ThreadDispatcherShutdownMode shutdownMode);

        /// <summary>
        ///     Waits until all queued operations of a specified priority have been processed.
        /// </summary>
        /// <param name="priority"> The priority. </param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running. </exception>
        void DoProcessing (int priority);
        
        /// <summary>
        ///     Waits until all queued operations of a specified priority have been processed.
        /// </summary>
        /// <param name="priority"> The priority. </param>
        /// <returns>
        ///     The task which can be used to await the completion of the processing.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running. </exception>
        Task DoProcessingAsync (int priority);

        /// <summary>
        ///     Determines under which options the current code is executed.
        /// </summary>
        /// <returns>
        ///     The options of the currently executed code or null if the current code is not executed by the dispatcher.
        ///     null is returned if the dispatcher is not running.
        /// </returns>
        ThreadDispatcherOptions? GetCurrentOptions ();

        /// <summary>
        ///     Determines under which priority the current code is executed.
        /// </summary>
        /// <returns>
        ///     The priority of the currently executed code or null if the current code is not executed by the dispatcher.
        ///     null is returned if the dispatcher is not running.
        /// </returns>
        int? GetCurrentPriority ();

        /// <summary>
        ///     Determines whether the caller of this function is executed inside the dispatchers thread.
        /// </summary>
        /// <returns>
        ///     true if the caller of this function is executed inside the dispatchers thread, false otherwise.
        ///     false is returned if the dispatcher is not running.
        /// </returns>
        bool IsInThread ();

        /// <summary>
        ///     Waits until the thread dispatcher is shut down.
        /// </summary>
        /// <remarks>
        /// <note type="implement">
        /// This method blocks indefinitely until the thread dispatcher has been shut down.
        /// </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The dispatcher is not being shut down.</exception>
        void WaitForShutdown();

        /// <summary>
        ///     Waits until the thread dispatcher is shut down.
        /// </summary>
        /// <remarks>
        /// <note type="implement">
        /// This method waits indefinitely until the thread dispatcher has been shut down.
        /// </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The dispatcher is not being shut down.</exception>
        Task WaitForShutdownAsync();

        /// <summary>
        ///     Enqueues a delegate to the thread dispatchers queue and does not wait for its execution.
        /// </summary>
        /// <param name="executionContext"> The context under which the delegate is executed. Can be null to use the calling threads context. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher operation object which can be used to track and control the execution of the enqueued delegate.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <note type="implement">
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </note>
        ///     <note type="implement">
        ///         Must be callable from the dispatcher thread.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
        IThreadDispatcherOperation Post(ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters);

        /// <summary>
        ///     Enqueues a delegate to the thread dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="executionContext"> The context under which the delegate is executed. Can be null to use the calling threads context. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="timeout">The timeout used to wait for the delegate to finish execution. Can be <see cref="Timeout.InfiniteTimeSpan"/> to wait indefinitely.</param>
        /// <param name="ct">The cancellation token used to cancel the wait for the delegate to finish execution. Can be <see cref="CancellationToken.None"/> if cancellation is not used.</param>
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
        ///     <note type="implement">
        ///         Must be callable from the dispatcher thread and can be therefore be cascaded.
        ///     </note>
        ///     <note type="implement">
        ///         <paramref name="timeout"/> and <paramref name="ct"/> are not used to cancel the execution of the delegate.
        ///         They are solely used to wait for its execution to finish.
        ///         The processing of the operation continues unchanged.
        ///         Use <see cref="IThreadDispatcherOperation.Cancel"/> to cancel the processing of an operation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero or timeout is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        object Send (ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, TimeSpan timeout, CancellationToken ct, Delegate action, params object[] parameters);

        /// <summary>
        ///     Enqueues a delegate to the thread dispatchers queue and waits for its execution to be completed.
        /// </summary>
        /// <param name="executionContext"> The context under which the delegate is executed. Can be null to use the calling threads context. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="timeout">The timeout used to wait for the delegate to finish execution. Can be <see cref="Timeout.InfiniteTimeSpan"/> to wait indefinitely.</param>
        /// <param name="ct">The cancellation token used to cancel the wait for the delegate to finish execution. Can be <see cref="CancellationToken.None"/> if cancellation is not used.</param>
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
        ///     <note type="implement">
        ///         Must be callable from the dispatcher thread and can be therefore be cascaded.
        ///     </note>
        ///     <note type="implement">
        ///         <paramref name="timeout"/> and <paramref name="ct"/> are not used to cancel the execution of the delegate.
        ///         They are solely used to wait for its execution to finish.
        ///         The processing of the operation continues unchanged.
        ///         Use <see cref="IThreadDispatcherOperation.Cancel"/> to cancel the processing of an operation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero or timeout is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
        /// <exception cref="TimeoutException">The execution of the delegate was cancelled by timeout or a cancellation token.</exception>
        /// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
        /// <exception cref="OperationCanceledException"> The execution of the delegate was canceled or aborted. </exception>
        Task<object> SendAsync (ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, TimeSpan timeout, CancellationToken ct, Delegate action, params object[] parameters);

        /// <summary>
        ///     Creates a new thread dispatcher timer.
        /// </summary>
        /// <param name="mode"> The mode under which the created timer operates. </param>
        /// <param name="milliseconds"> The delay of the execution in milliseconds until the first execution. </param>
        /// <param name="executionContext"> The context under which the delegate is executed. Can be null to use the calling threads context. </param>
        /// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
        /// <param name="options"> The used execution options. Can be <see cref="ThreadDispatcherOptions.Default"/> to use the default options.</param>
        /// <param name="action"> The delegate. </param>
        /// <param name="parameters"> Optional parameters of the delegate. </param>
        /// <returns>
        ///     The dispatcher timer object which can be used to track and control the execution of the enqueued delegate.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
        ///     </para>
        ///     <note type="implement">
        ///         A delegate must be enqueuable before the dispatcher is run.
        ///     </note>
        ///     <note type="implement">
        ///         Must be callable from the dispatcher thread.
        ///     </note>
        ///     <note type="implement">
        ///         The returned timer must not be started.
        /// <see cref="IThreadDispatcherTimer.Start"/> has to be called in order to start the timer.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is zero or negative or <paramref name="priority" />  is less than zero. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The dispatcher is not running or being shut down. </exception>
        IThreadDispatcherTimer PostDelayed (ThreadDispatcherTimerMode mode, int milliseconds, ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters);
    }
}
