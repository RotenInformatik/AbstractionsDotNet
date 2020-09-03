using System;




namespace RI.Abstractions.Logging
{
    /// <summary>
    ///     Logger abstraction implementation which does nothing.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class NullLogger : ILogger
    {
        #region Interface: ILogger

        /// <inheritdoc />
        public void Log (DateTime timestampUtc, int threadId, LogLevel level, string source, Exception exception, string format, params object[] args) { }

        #endregion
    }
}
