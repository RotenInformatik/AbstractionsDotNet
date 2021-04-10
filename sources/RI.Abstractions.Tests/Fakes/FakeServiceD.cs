using System.Diagnostics.CodeAnalysis;




namespace RI.Abstractions.Tests.Fakes
{
    public sealed class FakeServiceD : IFakeServiceD
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public FakeServiceD (IFakeServiceA serviceA, IFakeServiceB serviceB) { }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public FakeServiceD (IFakeServiceB serviceB, IFakeServiceA serviceA) { }
    }
}
