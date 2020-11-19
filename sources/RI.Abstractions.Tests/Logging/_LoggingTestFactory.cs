using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

using RI.Abstractions.Logging;




namespace RI.Abstractions.Tests.Logging
{
    public static class _LoggingTestFactory
    {
        public static IEnumerable<object[]> GetLoggers () =>
            new List<object[]>
            {
                _LoggingTestFactory.GetNullLogger(),
                _LoggingTestFactory.GetCallbackLogger(),
                _LoggingTestFactory.GetLoggerFactoryLogger(),
                _LoggingTestFactory.GetLoggerLogger(),
            };

        public static object[] GetNullLogger ()
        {
            return new object[]
            {
                new NullLogger(),
            };
        }

        public static object[] GetCallbackLogger ()
        {
            return new object[]
            {
                new CallbackLogger((utc, id, level, source, exception, format, args) => { }),
            };
        }

        public static object[] GetLoggerFactoryLogger ()
        {
            return new object[]
            {
                new Logger(LoggerFactory.Create(options => { })),
            };
        }

        public static object[] GetLoggerLogger()
        {
            return new object[]
            {
                new Logger(LoggerFactory.Create(options => { }).CreateLogger("Test")),
            };
        }
    }
}
