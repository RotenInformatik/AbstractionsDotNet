using System;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IThreadDispatcherTimer" /> type.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public static class IThreadDispatcherTimerExtensions
    {
        /// <summary>
        ///     Starts the timer using its already/previously set interval.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <returns>
        /// true if the timer was not started before and had to be started, false if the timer was already running.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// If the timer is already started, nothing should happen.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="timer"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The interval was not set previously.</exception>
        public static bool Start (this IThreadDispatcherTimer timer)
        {
            if (timer == null)
            {
                throw new ArgumentNullException(nameof(timer));
            }

            if (timer.Interval == TimeSpan.Zero)
            {
                throw new InvalidOperationException("The interval was not set previously.");
            }

            return timer.Start(timer.Interval);
        }

        /// <summary>
        ///     Restarts the timer using its already/previously set interval.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <returns>
        /// true if the timer was not started before and had to be started, false if the timer was already running.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// If the timer is already started, it should be stopped first and started again.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="timer"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The interval was not set previously.</exception>
        public static bool ReStart(this IThreadDispatcherTimer timer)
        {
            if (timer == null)
            {
                throw new ArgumentNullException(nameof(timer));
            }

            if (timer.Interval == TimeSpan.Zero)
            {
                throw new InvalidOperationException("The interval was not set previously.");
            }

            bool wasStopped = !timer.IsRunning;
            timer.Stop();
            timer.Start();
            return wasStopped;
        }

        /// <summary>
        ///     Restarts the timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <param name="interval">The new interval of the timer.</param>
        /// <returns>
        /// true if the timer was not started before and had to be started, false if the timer was already running.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// If the timer is already started, it should be stopped first and started again.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="timer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is zero or negative.</exception>
        public static bool ReStart(this IThreadDispatcherTimer timer, TimeSpan interval)
        {
            if (timer == null)
            {
                throw new ArgumentNullException(nameof(timer));
            }

            if (interval.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(interval));
            }

            bool wasStopped = !timer.IsRunning;
            timer.Stop();
            timer.Start(interval);
            return wasStopped;
        }
    }
}
