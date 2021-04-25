using System;




namespace RI.Abstractions.Dispatcher
{
	/// <summary>
	///     Describes the mode of a thread dispatcher timer.
	/// </summary>
	[Serializable]
	public enum ThreadDispatcherTimerMode
	{
		/// <summary>
		///     The timer is only executed once after the specified interval.
		/// </summary>
		OneShot = 0,

		/// <summary>
		///     The timer is executed repeatedly in the specified interval.
		/// </summary>
		Continuous = 1,
	}
}
