using System.Collections.Generic;

using RI.Abstractions.Tests.Composition;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_BeginShutdown
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();
    }
}
