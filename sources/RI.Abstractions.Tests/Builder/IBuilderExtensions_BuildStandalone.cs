using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using RI.Abstractions.Builder;
using RI.Abstractions.Composition;
using RI.Abstractions.Logging;
using RI.Abstractions.Tests.Fakes;

using Xunit;




namespace RI.Abstractions.Tests.Builder
{
    public sealed class IBuilderExtensions_BuildStandalone
    {
        [Theory]
        [MemberData(nameof(IBuilderExtensions_BuildStandalone.GetBuilders))]
        public static void BuildStandalone_Empty_Success(IBuilder builder)
        {
            //Act + Assert
            builder.BuildStandalone();
        }

        [Theory]
        [MemberData(nameof(IBuilderExtensions_BuildStandalone.GetBuilders))]
        public static void BuildStandalone_SimpleContainer_FailTooManyContainers(IBuilder builder)
        {
            //Arrange
            builder.UseSimpleContainer(new SimpleContainer());

            //Act + Assert
            Assert.Throws<BuilderException>(() => builder.BuildStandalone());
        }

        [Theory]
        [MemberData(nameof(IBuilderExtensions_BuildStandalone.GetBuilders))]
        public static void BuildStandalone_Empty_Logger(IBuilder builder)
        {
            //Act
            IServiceProvider serviceProvider = builder.BuildStandalone();

            //Assert
            Assert.Empty(serviceProvider.GetServices(typeof(NullLogger)));
            Assert.Single(serviceProvider.GetServices(typeof(ILogger)));
        }

        [Theory]
        [MemberData(nameof(IBuilderExtensions_BuildStandalone.GetBuilders))]
        public static void BuildStandalone_Empty_Container(IBuilder builder)
        {
            //Act
            IServiceProvider serviceProvider = builder.BuildStandalone();

            //Assert
            Assert.Empty(serviceProvider.GetServices(typeof(SimpleContainer)));
            Assert.Empty(serviceProvider.GetServices(typeof(ICompositionContainer)));
        }

        public static IEnumerable<object[]> GetBuilders() =>
            _BuilderTestFactory.GetBuilders();
    }
}
