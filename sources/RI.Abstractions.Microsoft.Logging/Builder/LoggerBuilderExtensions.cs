using System;

using Microsoft.Extensions.Logging;

using RI.Abstractions.Logging;

using ILogger = Microsoft.Extensions.Logging.ILogger;




namespace RI.Abstractions.Builder
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IBuilder" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class LoggerBuilderExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Adds registrations for using a logger.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="logger"> The logger to use. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="logger" /> is null. </exception>
        public static T UseLogger<T> (this T builder, ILogger logger)
        where T : IBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            builder.AddSingleton(typeof(Logging.ILogger), new Logger(logger));

            return builder;
        }

        /// <summary>
        ///     Adds registrations for using a logger factory.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="loggerFactory"> The logger factory to use. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="loggerFactory" /> is null. </exception>
        public static T UseLoggerFactory<T> (this T builder, ILoggerFactory loggerFactory)
            where T : IBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            builder.AddSingleton(typeof(Logging.ILogger), new Logger(loggerFactory));

            return builder;
        }

        #endregion
    }
}
