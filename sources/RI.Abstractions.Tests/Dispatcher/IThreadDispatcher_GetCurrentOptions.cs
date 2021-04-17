using System.Collections.Generic;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_GetCurrentOptions
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();
    }
}
