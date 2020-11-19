namespace RI.Abstractions.Tests.Fakes
{
    public interface IFakeServiceC
    {
        IFakeServiceA ServiceA { get; }

        IFakeServiceB ServiceB { get; }
    }
}
