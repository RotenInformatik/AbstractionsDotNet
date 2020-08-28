using System;




namespace RI.Abstractions.Logging
{
    /// <summary>
    ///     Simple logger abstraction implementation using a callback.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class CallbackLogger : ILogger
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CallbackLogger" />.
        /// </summary>
        /// <param name="callback"> The callback delegate to call for each log message to write. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        public CallbackLogger (CallbackLoggerDelegate callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.Callback = callback;
        }

        #endregion




        #region Instance Properties/Indexer

        private CallbackLoggerDelegate Callback { get; }

        #endregion




        #region Interface: ILogger

        /// <inheritdoc />
        public void Log (DateTime timestampUtc, int threadId, LogLevel level, string source, Exception exception, string format, params object[] args)
        {
            this.Callback(timestampUtc, threadId, level, source, exception, format, args);
        }

        #endregion
    }
}
