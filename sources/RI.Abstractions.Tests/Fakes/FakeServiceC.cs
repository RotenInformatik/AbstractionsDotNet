namespace RI.Abstractions.Tests.Fakes
{
    public sealed class FakeServiceC : IFakeServiceC
    {
        public IFakeServiceA ServiceA { get; }

        public IFakeServiceB ServiceB { get; }

        public FakeServiceC(IFakeServiceA serviceA, IFakeServiceB serviceB)
        {
            this.ServiceA = serviceA;
            this.ServiceB = serviceB;
        }
    }
}
