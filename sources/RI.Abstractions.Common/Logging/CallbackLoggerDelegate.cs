using System;




namespace RI.Abstractions.Logging
{
    /// <summary>
    ///     Delegate to receive log message writing callbacks.
    /// </summary>
    /// <param name="timestampUtc"> The UTC timestamp to be associated with the log message. </param>
    /// <param name="threadId"> The ID of the thread the log message originates from. </param>
    /// <param name="level"> The log level of the log message. </param>
    /// <param name="source"> The source/origin of the log message. </param>
    /// <param name="exception"> Optional exception associated with the log message. </param>
    /// <param name="format"> The log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc. which are expanded using <paramref name="args" />). </param>
    /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
    /// <remarks>
    /// <note type="implement">
    /// <paramref name="source"/>, <paramref name="exception"/>, <paramref name="format"/>, and <paramref name="args"/> can be null in any combination.
    /// </note>
    /// </remarks>
    public delegate void CallbackLoggerDelegate (DateTime timestampUtc, int threadId, LogLevel level, string source, Exception exception, string format, object[] args);
}
