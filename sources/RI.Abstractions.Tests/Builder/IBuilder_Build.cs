using System.Collections.Generic;

using RI.Abstractions.Builder;
using RI.Abstractions.Composition;
using RI.Abstractions.Logging;
using RI.Abstractions.Tests.Composition;
using RI.Abstractions.Tests.Fakes;

using Xunit;




namespace RI.Abstractions.Tests.Builder
{
    public sealed class IBuilder_Build
    {
        [Theory]
        [MemberData(nameof(IBuilder_Build.GetBuilders))]
        public static void Build_Empty_FailNoContainer (IBuilder builder)
        {
           //Act + Assert
            Assert.Throws<BuilderException>(() => builder.Build());
        }

        [Theory]
        [MemberData(nameof(IBuilder_Build.GetBuilders))]
        public static void Build_SimpleContainer_Success(IBuilder builder)
        {
            //Arrange
            builder.UseSimpleContainer(new SimpleContainer());

            //Act + Assert
            builder.Build();
        }

        [Theory]
        [MemberData(nameof(IBuilder_Build.GetBuilders))]
        public static void Build_SimpleContainer_Logger(IBuilder builder)
        {
            //Arrange
            SimpleContainer container = new SimpleContainer();
            builder.UseSimpleContainer(container);

            //Act
            builder.Build();

            //Assert
            Assert.Empty(container.GetServices(typeof(NullLogger)));
            Assert.Single(container.GetServices(typeof(ILogger)));
        }

        [Theory]
        [MemberData(nameof(IBuilder_Build.GetBuilders))]
        public static void Build_SimpleContainer_Container(IBuilder builder)
        {
            //Arrange
            SimpleContainer container = new SimpleContainer();
            builder.UseSimpleContainer(container);

            //Act
            builder.Build();

            //Assert
            Assert.Empty(container.GetServices(typeof(SimpleContainer)));
            Assert.Empty(container.GetServices(typeof(ICompositionContainer)));
        }

        public static IEnumerable<object[]> GetBuilders() =>
            _BuilderTestFactory.GetBuilders();
    }
}
