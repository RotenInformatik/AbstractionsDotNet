using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Logging;




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
        public Logger (Microsoft.Extensions.Logging.ILogger logger)
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
        public Logger (ILoggerFactory loggerFactory, string categoryName = nameof(Logger))
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
        ///     The logger to use.
        /// </value>
        public Microsoft.Extensions.Logging.ILogger UsedLogger { get; }

        private Func<Exception, string> ExceptionFormatter { get; set; }

        #endregion




        #region Instance Methods

        private string FormatException (Exception exception)
        {
            if (this.ExceptionFormatter == null)
            {
                try
                {
                    MethodInfo method = AppDomain.CurrentDomain.GetAssemblies()
                                                 .SelectMany(x => x.GetExportedTypes())
                                                 .Where(x => x.FullName == "RI.Utilities.Exceptions.ExceptionExtensions")
                                                 .Select(x => x.GetMethod("ToDetailedString", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new[]
                                                 {
                                                     typeof(Exception),
                                                     typeof(char),
                                                 }, null))
                                                 .FirstOrDefault();

                    if (method != null)
                    {
                        this.ExceptionFormatter = x => (string)method.Invoke(null, new object[]
                        {
                            x,
                            '-',
                        });
                    }
                }
                catch
                {
                    // We want this to silently fail because this is just a "let us give it a try..." approach.
                }
            }

            if (this.ExceptionFormatter == null)
            {
                this.ExceptionFormatter = x => x.ToString();
            }

            return this.ExceptionFormatter(exception);
        }

        private string FormatTimestamp (DateTime timestamp)
        {
            return timestamp.ToString("yyyy'-'MM'-'dd'-'HH'-'mm'-'ss'-'fff", CultureInfo.InvariantCulture);
        }

        private Microsoft.Extensions.Logging.LogLevel TranslateLogLevel (LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
                LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
                LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
                LogLevel.Fatal => Microsoft.Extensions.Logging.LogLevel.Critical,
                _ => Microsoft.Extensions.Logging.LogLevel.None
            };
        }

        #endregion




        #region Interface: ILogger

        /// <inheritdoc />
        public void Log (DateTime timestampUtc, int threadId, LogLevel level, string source, Exception exception, string format, params object[] args)
        {
            if (exception != null)
            {
                this.UsedLogger.Log(this.TranslateLogLevel(level), exception, $"[{this.FormatTimestamp(timestampUtc)}] [{threadId}] [{level}] [{source}]{Environment.NewLine}[MESSAGE]{Environment.NewLine}{string.Format(format, args)}{Environment.NewLine}[EXCEPTION]{Environment.NewLine}{this.FormatException(exception)}");
            }
            else
            {
                this.UsedLogger.Log(this.TranslateLogLevel(level), $"[{this.FormatTimestamp(timestampUtc)}] [{threadId}] [{level}] [{source}] {string.Format(format, args)}");
            }
        }

        #endregion
    }
}
