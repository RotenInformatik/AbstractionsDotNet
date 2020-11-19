namespace RI.Abstractions.Tests.Fakes
{
    public sealed class FakeServiceB : IFakeServiceB
    {
        public IFakeServiceA ServiceA { get; }

        public FakeServiceB(IFakeServiceA serviceA)
        {
            this.ServiceA = serviceA;
        }
    }
}
