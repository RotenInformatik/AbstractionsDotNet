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
        /// <param name="source"> The optional source/origin of the log message. </param>
        /// <param name="exception"> The optional exception associated with the log message. </param>
        /// <param name="format"> The optional log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc. which are expanded using <paramref name="args" />). </param>
        /// <param name="args"> The optional message arguments expanded into <paramref name="format" />. </param>
        /// <remarks>
        /// <note type="implement">
        /// <paramref name="source"/>, <paramref name="exception"/>, <paramref name="format"/>, and <paramref name="args"/> can be null in any combination.
        /// </note>
        /// <note type="implement">
        /// <see cref="Log"/> must not throw any exceptions or cause recursive logging.
        /// </note>
        /// </remarks>
        void Log (DateTime timestampUtc, int threadId, LogLevel level, string source, Exception exception, string format, params object[] args);
    }
}
