using System;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     An operation (delegate) scheduled to be executed by a thread dispatcher either delayed (one-shot) or in repeated intervals (continuous).
    /// </summary>
    /// <remarks>
    ///     <note type="implement">
    ///         <see cref="IThreadDispatcherTimer" /> enqueues a delegate to the specified dispatchers queue (using <see cref="IThreadDispatcher.Post(ThreadDispatcherExecutionContext,int,ThreadDispatcherOptions,Delegate,object[])" />) in a specified interval (never before).
    ///     </note>
    ///     <note type="implement">
    ///         The interval is awaited before the timer is executed for the first time. Afterwards, the delegate is posted to the dispatcher in the specified interval.
    ///     </note>
    ///     <note type="implement">
    ///         If a previous interval has not yet finished executing the delegate, it is not dispatched for execution until the next interval.
    ///     </note>
    ///     <note type="implement">
    ///         The timer is initially stopped and needs to be started explicitly using <see cref="Start" />.
    ///     </note>
    ///     <note type="implement">
    ///         The associated thread dispatcher does not need to be started.
    ///         However, the timer is stopped on an interval if the dispatcher is not running.
    ///     </note>
    /// </remarks>
    public interface IThreadDispatcherTimer : IThreadDispatcherRunnable
    {
        /// <summary>
        ///     Gets the number of times the delegate was executed.
        /// </summary>
        /// <value>
        ///     The number of times the delegate was executed.
        /// </value>
        /// <remarks>
        /// <note type="implement">
        ///         <see cref="ExecutionCount" /> must be reset to zero whenever the timer is (re-)started.
        ///     </note>
        /// </remarks>
        long ExecutionCount { get; }

        /// <summary>
        ///     Gets the used interval.
        /// </summary>
        /// <value>
        ///     The used interval.
        /// </value>
        TimeSpan Interval { get; }

        /// <summary>
        ///     Gets whether the timer is currently running.
        /// </summary>
        /// <value>
        ///     true if the timer is running, false otherwise.
        /// </value>
        bool IsRunning { get; }

        /// <summary>
        ///     Gets the number of times the delegate was not executed because the previous execution was not yet finished.
        /// </summary>
        /// <value>
        ///     The number of times the delegate was not executed because the previous execution was not yet finished
        /// </value>
        /// <remarks>
        /// <note type="implement">
        ///         <see cref="MissCount" /> must be reset to zero whenever the timer is (re-)started.
        ///     </note>
        /// </remarks>
        long MissCount { get; }

        /// <summary>
        ///     Gets the timer mode.
        /// </summary>
        /// <value>
        ///     The timer mode.
        /// </value>
        ThreadDispatcherTimerMode Mode { get; }

        /// <summary>
        ///     Starts the timer.
        /// </summary>
        /// <param name="interval">The new interval of the timer.</param>
        /// <returns>
        /// true if the timer was not started before and had to be started, false if the timer was already running.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// If the timer is already started, nothing should happen.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is zero or negative.</exception>
        bool Start (TimeSpan interval);

        /// <summary>
        ///     Stops the timer.
        /// </summary>
        /// <returns>
        /// true if the timer was started before and had to be stopped, false if the timer was not running.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// If the timer is already stopped, nothing should happen.
        /// </note>
        /// </remarks>
        bool Stop ();
    }
}
