namespace RI.Abstractions.Tests.Fakes
{
    public sealed class FakeServiceD : IFakeServiceD
    {
        public FakeServiceD (IFakeServiceA serviceA, IFakeServiceB serviceB) { }

        public FakeServiceD (IFakeServiceB serviceB, IFakeServiceA serviceA) { }
    }
}
