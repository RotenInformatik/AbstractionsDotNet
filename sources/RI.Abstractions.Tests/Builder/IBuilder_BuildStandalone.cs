using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using RI.Abstractions.Builder;
using RI.Abstractions.Composition;
using RI.Abstractions.Logging;
using RI.Abstractions.Tests.Fakes;

using Xunit;




namespace RI.Abstractions.Tests.Builder
{
    public sealed class IBuilder_BuildStandalone
    {
        [Fact]
        public static void Build_Empty_Success()
        {
            //Arrange
            FakeBuilder builder = new FakeBuilder();

            //Act + Assert
            builder.BuildStandalone();
        }

        [Fact]
        public static void Build_SimpleContainer_FailTooManyContainers()
        {
            //Arrange
            FakeBuilder builder = new FakeBuilder();
            builder.UseSimpleContainer(new SimpleContainer());

            //Act + Assert
            Assert.Throws<BuilderException>(() => builder.BuildStandalone());
        }

        [Fact]
        public static void Build_Empty_Logger()
        {
            //Arrange
            FakeBuilder builder = new FakeBuilder();

            //Act
            IServiceProvider serviceProvider = builder.BuildStandalone();

            //Assert
            Assert.Empty(serviceProvider.GetServices(typeof(NullLogger)));
            Assert.Single(serviceProvider.GetServices(typeof(ILogger)));
        }

        [Fact]
        public static void Build_Empty_Container()
        {
            //Arrange
            FakeBuilder builder = new FakeBuilder();

            //Act
            IServiceProvider serviceProvider = builder.BuildStandalone();

            //Assert
            Assert.Empty(serviceProvider.GetServices(typeof(SimpleContainer)));
            Assert.Empty(serviceProvider.GetServices(typeof(ICompositionContainer)));
        }
    }
}
