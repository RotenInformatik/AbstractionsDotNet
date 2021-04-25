using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using RI.Abstractions.Logging;
using RI.Abstractions.Tests.Fakes;
using RI.Abstractions.Tests.Logging;




namespace RI.Abstractions.Tests.Builder
{
    public sealed class _BuilderTestFactory
    {
        public static IEnumerable<object[]> GetBuilders() =>
            new List<object[]>
            {
                _BuilderTestFactory.GetFakeBuilder(),
            };

        public static object[] GetFakeBuilder()
        {
            return new object[]
            {
                new FakeBuilder(),
            };
        }
    }
}
