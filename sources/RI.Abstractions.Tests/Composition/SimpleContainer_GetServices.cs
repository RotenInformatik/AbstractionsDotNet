using System;
using System.Collections.Generic;
using System.Linq;

using RI.Abstractions.Composition;

using Xunit;




namespace RI.Abstractions.Tests.Composition
{
    public sealed class SimpleContainer_GetServices
    {
        [Fact]
        public static void GetServices_Empty_ReturnsEmpty()
        {
            // Arrange
            SimpleContainer container = new SimpleContainer();

            // Act + Assert
            Assert.Empty(container.GetServices(typeof(string)));
        }

        [Fact]
        public static void GetServices_NullType_ArgumentNullException()
        {
            // Arrange
            SimpleContainer container = new SimpleContainer();

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => container.GetServices(null));
        }

        [Fact]
        public static void GetServices_TemporaryOne_ReturnsEmpty()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Temporary(typeof(string), "Instance"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act + Assert
            Assert.Empty(container.GetServices(typeof(string)));
        }

        [Fact]
        public static void GetServices_TemporaryMultiple_ReturnsEmpty()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Temporary(typeof(string), "Instance"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act + Assert
            Assert.Empty(container.GetServices(typeof(string)));
        }

        [Fact]
        public static void GetServices_SingletonOne_ReturnsOne()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Singleton(typeof(string), "Instance"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            List<string> value = container.GetServices(typeof(string))?.Cast<string>().ToList();

            // Assert
            Assert.NotNull(value);
            Assert.True(value.Count == 1);
            Assert.Equal(value[0], "Instance");
        }

        [Fact]
        public static void GetServices_SingletonMultiple_ReturnsAll()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Singleton(typeof(string), "Instance1"),
                CompositionRegistration.Singleton(typeof(string), "Instance2"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            List<string> value = container.GetServices(typeof(string))?.Cast<string>().ToList();

            // Assert
            Assert.NotNull(value);
            Assert.True(value.Count == 2);
            Assert.Equal(value[0], "Instance1");
            Assert.Equal(value[1], "Instance2");
        }

        [Fact]
        public static void GetServices_TransientOne_ReturnsOne()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Transient(typeof(string), _ => "Factory"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            List<string> value = container.GetServices(typeof(string))?.Cast<string>().ToList();

            // Assert
            Assert.NotNull(value);
            Assert.True(value.Count == 1);
            Assert.Equal(value[0], "Factory");
        }

        [Fact]
        public static void GetServices_TransientMultiple_ReturnsAll()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Transient(typeof(string), _ => "Factory1"),
                CompositionRegistration.Transient(typeof(string), _ => "Factory2"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            List<string> value = container.GetServices(typeof(string))?.Cast<string>().ToList();

            // Assert
            Assert.NotNull(value);
            Assert.True(value.Count == 2);
            Assert.Equal(value[0], "Factory1");
            Assert.Equal(value[1], "Factory2");
        }

        [Fact]
        public static void GetServices_NotAlwaysTransient_ReturnsAll()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Transient(typeof(string), _ => "Factory1", false),
                CompositionRegistration.Transient(typeof(string), _ => "Factory2", true),
                CompositionRegistration.Transient(typeof(string), _ => "Factory3", false),
                CompositionRegistration.Transient(typeof(string), _ => "Factory4", true),
                CompositionRegistration.Transient(typeof(string), _ => "Factory5", false),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            List<string> value = container.GetServices(typeof(string))?.Cast<string>().ToList();

            // Assert
            Assert.NotNull(value);
            Assert.True(value.Count == 3);
            Assert.Equal(value[0], "Factory1");
            Assert.Equal(value[1], "Factory2");
            Assert.Equal(value[2], "Factory4");
        }

        [Fact]
        public static void GetServices_NotAlwaysSingleton_ReturnsAll()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Singleton(typeof(string), _ => "Factory1", false),
                CompositionRegistration.Singleton(typeof(string), _ => "Factory2", true),
                CompositionRegistration.Singleton(typeof(string), _ => "Factory3", false),
                CompositionRegistration.Singleton(typeof(string), _ => "Factory4", true),
                CompositionRegistration.Singleton(typeof(string), _ => "Factory5", false),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            List<string> value = container.GetServices(typeof(string))?.Cast<string>().ToList();

            // Assert
            Assert.NotNull(value);
            Assert.True(value.Count == 3);
            Assert.Equal(value[0], "Factory1");
            Assert.Equal(value[1], "Factory2");
            Assert.Equal(value[2], "Factory4");
        }
    }
}
