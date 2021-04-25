using System;
using System.Collections.Generic;

using RI.Abstractions.Logging;

using Xunit;




namespace RI.Abstractions.Tests.Logging
{
    public sealed class ILogger_Log
    {
        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithNothing_NoException (ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, null, null, null);
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithSource_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, "Source", null, null);
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithException_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, null, new Exception("Exception"), null);
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithMessage_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, null, null, "Message");
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithMessageAndArgs_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, null, null, "Message: {0}", 123);
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithMessageAndTooManyArgs_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, null, null, "Message: {0}", 123, 789);
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithMessageAndTooFewArgs_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, null, null, "Message: {0} {1}", 123);
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithMessageAndEmptyArgs_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, null, null, "Message", new object[0]);
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithMessageAndNullArgs_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, null, null, "Message", null);
        }

        [Theory]
        [MemberData(nameof(ILogger_Log.GetLoggers))]
        public void Log_WithEverything_NoException(ILogger instance)
        {
            // Act
            instance.Log(DateTime.UtcNow, 0, LogLevel.Debug, "Source", new Exception("Exception"), "Message: {0} {1}", 123, 789);
        }

        public static IEnumerable<object[]> GetLoggers () =>
            _LoggingTestFactory.GetLoggers();
    }
}
