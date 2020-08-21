using System;
using System.Threading;




namespace RI.Abstractions.Logging
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="ILogger" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class ILoggerExtensions
    {
        /// <summary>
        /// Writes a log message.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <param name="level">The log level of the log message.</param>
        /// <param name="source">The source/origin of the log message.</param>
        /// <param name="exception">Optional exception associated with the log message.</param>
        /// <param name="format">Optional log message (with optional string expansion arguments such as <c>{0}</c>, <c>{1}</c>, etc. which are expanded by <paramref name="args"/>).</param>
        /// <param name="args">Optional message arguments expanded into <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public static void Log (this ILogger logger, LogLevel level, string source, Exception exception = null, string format = null, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(DateTime.UtcNow, Thread.CurrentThread.ManagedThreadId, level, source, exception, format, args);
        }

        /// <summary>
        /// Writes a log message to the log sink.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <param name="source">The source/origin of the log message.</param>
        /// <param name="exception">Optional exception associated with the log message.</param>
        /// <param name="format">Optional log message (with optional string expansion arguments such as <c>{0}</c>, <c>{1}</c>, etc. which are expanded by <paramref name="args"/>).</param>
        /// <param name="args">Optional message arguments expanded into <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public static void LogDebug (this ILogger logger, string source, Exception exception = null, string format = null, params object[] args) => logger.Log(LogLevel.Debug, source, exception, format, args);

        /// <summary>
        /// Writes a log message to the log sink.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <param name="source">The source/origin of the log message.</param>
        /// <param name="exception">Optional exception associated with the log message.</param>
        /// <param name="format">Optional log message (with optional string expansion arguments such as <c>{0}</c>, <c>{1}</c>, etc. which are expanded by <paramref name="args"/>).</param>
        /// <param name="args">Optional message arguments expanded into <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public static void LogInformation(this ILogger logger, string source, Exception exception = null, string format = null, params object[] args) => logger.Log(LogLevel.Information, source, exception, format, args);

        /// <summary>
        /// Writes a log message to the log sink.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <param name="source">The source/origin of the log message.</param>
        /// <param name="exception">Optional exception associated with the log message.</param>
        /// <param name="format">Optional log message (with optional string expansion arguments such as <c>{0}</c>, <c>{1}</c>, etc. which are expanded by <paramref name="args"/>).</param>
        /// <param name="args">Optional message arguments expanded into <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public static void LogWarning(this ILogger logger, string source, Exception exception = null, string format = null, params object[] args) => logger.Log(LogLevel.Warning, source, exception, format, args);

        /// <summary>
        /// Writes a log message to the log sink.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <param name="source">The source/origin of the log message.</param>
        /// <param name="exception">Optional exception associated with the log message.</param>
        /// <param name="format">Optional log message (with optional string expansion arguments such as <c>{0}</c>, <c>{1}</c>, etc. which are expanded by <paramref name="args"/>).</param>
        /// <param name="args">Optional message arguments expanded into <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public static void LogError(this ILogger logger, string source, Exception exception = null, string format = null, params object[] args) => logger.Log(LogLevel.Error, source, exception, format, args);

        /// <summary>
        /// Writes a log message to the log sink.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <param name="source">The source/origin of the log message.</param>
        /// <param name="exception">Optional exception associated with the log message.</param>
        /// <param name="format">Optional log message (with optional string expansion arguments such as <c>{0}</c>, <c>{1}</c>, etc. which are expanded by <paramref name="args"/>).</param>
        /// <param name="args">Optional message arguments expanded into <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public static void LogFatal(this ILogger logger, string source, Exception exception = null, string format = null, params object[] args) => logger.Log(LogLevel.Fatal, source, exception, format, args);
    }
}
