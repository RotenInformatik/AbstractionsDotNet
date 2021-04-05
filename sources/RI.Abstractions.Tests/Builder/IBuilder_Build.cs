using RI.Abstractions.Builder;
using RI.Abstractions.Composition;
using RI.Abstractions.Logging;
using RI.Abstractions.Tests.Fakes;

using Xunit;




namespace RI.Abstractions.Tests.Builder
{
    public sealed class IBuilder_Build
    {
        [Fact]
        public static void Build_Empty_FailNoContainer ()
        {
            //Arrange
            FakeBuilder builder = new FakeBuilder();

            //Act + Assert
            Assert.Throws<BuilderException>(() => builder.Build());
        }

        [Fact]
        public static void Build_SimpleContainer_Success()
        {
            //Arrange
            FakeBuilder builder = new FakeBuilder();
            builder.UseSimpleContainer(new SimpleContainer());

            //Act + Assert
            builder.Build();
        }

        [Fact]
        public static void Build_SimpleContainer_Logger()
        {
            //Arrange
            FakeBuilder builder = new FakeBuilder();
            SimpleContainer container = new SimpleContainer();
            builder.UseSimpleContainer(container);

            //Act
            builder.Build();

            //Assert
            Assert.Empty(container.GetServices(typeof(NullLogger)));
            Assert.Single(container.GetServices(typeof(ILogger)));
        }

        [Fact]
        public static void Build_SimpleContainer_Container()
        {
            //Arrange
            FakeBuilder builder = new FakeBuilder();
            SimpleContainer container = new SimpleContainer();
            builder.UseSimpleContainer(container);

            //Act
            builder.Build();

            //Assert
            Assert.Empty(container.GetServices(typeof(SimpleContainer)));
            Assert.Empty(container.GetServices(typeof(ICompositionContainer)));
        }
    }
}
