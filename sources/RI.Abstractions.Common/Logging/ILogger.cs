using System;




namespace RI.Abstractions.Logging
{
    /// <summary>
    ///     Logger abstraction.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ILogger" /> is used to abstract away the used logging library and mechanism.
    ///     </para>
    /// </remarks>
    public interface ILogger
    {
        /// <summary>
        ///     Writes a log message.
        /// </summary>
        /// <param name="timestampUtc"> The UTC timestamp to be associated with the log message. </param>
        /// <param name="threadId"> The ID of the thread the log message originates from. </param>
        /// <param name="level"> The log level of the log message. </param>
        /// <param name="source"> The source/origin of the log message. </param>
        /// <param name="exception"> Optional exception associated with the log message. </param>
        /// <param name="format"> The log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc. which are expanded using <paramref name="args" />). </param>
        /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
        void Log (DateTime timestampUtc, int threadId, LogLevel level, string source, Exception exception, string format, params object[] args);
    }
}
