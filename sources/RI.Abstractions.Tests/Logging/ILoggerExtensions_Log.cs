using System;
using System.Collections.Generic;

using RI.Abstractions.Logging;

using Xunit;




namespace RI.Abstractions.Tests.Logging
{
    public sealed class ILoggerExtensions_Log
    {
        [Theory]
        [MemberData(nameof(ILoggerExtensions_Log.GetLoggers))]
        public void Log_WithNothing_NoException(ILogger instance)
        {
            // Act
            instance.Log(LogLevel.Debug, null);
        }

        [Theory]
        [MemberData(nameof(ILoggerExtensions_Log.GetLoggers))]
        public void Log_WithSource_NoException(ILogger instance)
        {
            // Act
            instance.Log(LogLevel.Debug, "Source");
        }

        [Theory]
        [MemberData(nameof(ILoggerExtensions_Log.GetLoggers))]
        public void Log_WithException_NoException(ILogger instance)
        {
            // Act
            instance.Log(LogLevel.Debug, null, exception:new Exception("Exception"));
        }

        [Theory]
        [MemberData(nameof(ILoggerExtensions_Log.GetLoggers))]
        public void Log_WithMessage_NoException(ILogger instance)
        {
            // Act
            instance.Log(LogLevel.Debug, null, format:"Message");
        }

        [Theory]
        [MemberData(nameof(ILoggerExtensions_Log.GetLoggers))]
        public void Log_WithMessageAndArgs_NoException(ILogger instance)
        {
            // Act
            instance.Log(LogLevel.Debug, null, format:"Message: {0}", args:123);
        }

        [Theory]
        [MemberData(nameof(ILoggerExtensions_Log.GetLoggers))]
        public void Log_WithEverything_NoException(ILogger instance)
        {
            // Act
            instance.Log(LogLevel.Debug, "Source", new Exception("Exception"), "Message: {0} {1}", 123, 789);
        }

        public static IEnumerable<object[]> GetLoggers() =>
            _LoggingTestFactory.GetLoggers();
    }
}
