﻿using System;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    /// An operation (delegate) scheduled to be executed by a thread dispatcher.
    /// </summary>
    public interface IThreadDispatcherOperation : IThreadDispatcherRunnable
    {
        /// <summary>
        ///     Gets the exception which occurred during execution of the delegate associated with this thread dispatcher operation.
        /// </summary>
        /// <value>
        ///     The exception which occurred during execution or null if no exception was thrown or the operation was not yet processed.
        /// </value>
        Exception Exception { get; }

        /// <summary>
        ///     Gets the value returned by the delegate associated with this thread dispatcher operation.
        /// </summary>
        /// <value>
        ///     The value returned by the delegate associated with this thread dispatcher operation or null if the delegate has no return value or the operation was not yet processed.
        /// </value>
        object Result { get; }

        /// <summary>
        ///     Gets the total time in milliseconds this thread dispatcher operation was executing within the thread dispatcher.
        /// </summary>
        /// <value>
        ///     The total time in milliseconds this thread dispatcher operation was executing within the thread dispatcher.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The executing time does only include the time the operation was using thread dispatcher resources, but does not include time which was spent in another thread (e.g. when the operation is a task which uses other threads).
        ///     </para>
        /// </remarks>
        int RunTimeMilliseconds { get; }

        /// <summary>
        ///     Gets the current state of the thread dispatcher operation.
        /// </summary>
        /// <value>
        ///     The current state of the thread dispatcher operation.
        /// </value>
        ThreadDispatcherOperationState State { get; }

        /// <summary>
        ///     Gets the number of watchdog events for this operation.
        /// </summary>
        /// <value>
        ///     The number of watchdog events for this operation.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         When a watchdog event occurs, this counter is already incremented, for example the first watchdog events has this property set to 1.
        ///     </para>
        /// </remarks>
        int WatchdogEvents { get; }

        /// <summary>
        ///     Gets the time in milliseconds this thread dispatcher operation was executing within the thread dispatcher since its last watchdog event.
        /// </summary>
        /// <value>
        ///     The time in milliseconds this thread dispatcher operation was executing within the thread dispatcher since its last watchdog event or -1 if no watchdog event has yet occurred.
        /// </value>
        /// 
        int WatchdogTimeMilliseconds { get; }

        /// <summary>
        ///     Cancels the processing of the thread dispatcher operation.
        /// </summary>
        /// <returns>
        ///     true if the operation could be canceled, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         A thread dispatcher operation can only be canceled if its is still pending (<see cref="State" /> is <see cref="ThreadDispatcherOperationState.Waiting" />).
        ///     </note>
        ///     <note type="implement">
        ///         This method must be callable from the dispatcher thread.
        ///     </note>
        /// </remarks>
        bool Cancel ();

        /// <summary>
        ///     Waits a specified amount of time for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the thread dispatcher operation to finish processing before the method returns. </param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing within the specified timeout, false otherwise or if the wait was cancelled.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         This method must not be callable from the dispatcher thread.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        /// <exception cref="InvalidOperationException"><see cref="Wait"/> was called from the dispatcher thread. </exception>
        bool Wait (TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        ///     Waits a specified amount of time for the thread dispatcher operation to finish processing.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the thread dispatcher operation to finish processing before the method returns. </param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the thread dispatcher operation finished processing within the specified timeout, false otherwise or if the wait was cancelled.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         This method must be callable from the dispatcher thread.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        /// <exception cref="InvalidOperationException"><see cref="Wait"/> was called from the dispatcher thread. </exception>
        Task<bool> WaitAsync (TimeSpan timeout, CancellationToken cancellationToken);
    }
}
