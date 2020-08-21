using System;

using Microsoft.Extensions.Logging;

using RI.Utilities.Dates;
using RI.Utilities.Exceptions;




namespace RI.Abstractions.Logging
{
    /// <summary>
    ///     Logger abstraction implementation for Microsoft.Extensions.Logging.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class Logger : ILogger
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="Logging.Logger" />.
        /// </summary>
        /// <param name="logger"> The logger to use. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public Logger(Microsoft.Extensions.Logging.ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.UsedLogger = logger;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Logging.Logger" />.
        /// </summary>
        /// <param name="loggerFactory"> The logger factory to use. </param>
        /// <param name="categoryName"> The category name for messages produced by the logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="loggerFactory" /> is null. </exception>
        public Logger(ILoggerFactory loggerFactory, string categoryName = nameof(Logging.Logger))
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this.UsedLogger = loggerFactory.CreateLogger(categoryName);
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the logger to use.
        /// </summary>
        /// <value>
        /// The logger to use.
        /// </value>
        public Microsoft.Extensions.Logging.ILogger UsedLogger { get; }

        #endregion

        /// <inheritdoc />
        public void Log(DateTime timestampUtc, int threadId, LogLevel level, string source, Exception exception, string format, params object[] args)
        {
            if (exception != null)
            {
                this.UsedLogger.Log(this.TranslateLogLevel(level), exception, $"[{timestampUtc.ToSortableString('-')}] [{threadId}] [{level}] [{source}]{Environment.NewLine}[MESSAGE]{Environment.NewLine}{string.Format(format, args)}{Environment.NewLine}[EXCEPTION]{Environment.NewLine}{exception.ToDetailedString()}");
            }
            else
            {
                this.UsedLogger.Log(this.TranslateLogLevel(level), $"[{timestampUtc.ToSortableString('-')}] [{threadId}] [{level}] [{source}] {string.Format(format, args)}");
            }
        }

        private Microsoft.Extensions.Logging.LogLevel TranslateLogLevel (LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return Microsoft.Extensions.Logging.LogLevel.Debug;
                case LogLevel.Information:
                    return Microsoft.Extensions.Logging.LogLevel.Information;
                case LogLevel.Warning:
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                case LogLevel.Error:
                    return Microsoft.Extensions.Logging.LogLevel.Error;
                case LogLevel.Fatal:
                    return Microsoft.Extensions.Logging.LogLevel.Critical;
                default:
                    return Microsoft.Extensions.Logging.LogLevel.None;
            }
        }
    }
}
