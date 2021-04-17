using System.Collections.Generic;

using RI.Abstractions.Composition;
using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Composition;




namespace RI.Abstractions.Tests.Dispatcher
{
    public class _DispatcherTestFactory
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            new List<object[]>
            {
                _CompositionTestFactory.GetSimpleContainer(),
                _CompositionTestFactory.GetServiceCollectionContainer(),
            };

        public static object[] GetSimpleDispatcher()
        {
            return new object[]
            {
                new SimpleDispatcher(),
            };
        }
    }
}
