using System;

using RI.Abstractions.Composition;

using Xunit;




namespace RI.Abstractions.Tests.Composition
{
    public sealed class SimpleContainer_GetService
    {
        [Fact]
        public static void GetService_Empty_ReturnsNull ()
        {
            // Arrange
            SimpleContainer container = new SimpleContainer();

            // Act + Assert
            Assert.Null(container.GetService(typeof(string)));
        }

        [Fact]
        public static void GetService_NullType_ArgumentNullException()
        {
            // Arrange
            SimpleContainer container = new SimpleContainer();

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => container.GetService(null));
        }

        [Fact]
        public static void GetService_TemporaryTypeOne_ReturnsNull()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Temporary(typeof(string), typeof(string)),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act + Assert
            Assert.Null(container.GetService(typeof(string)));
        }

        [Fact]
        public static void GetService_TemporaryTypeMultiple_ReturnsNull()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Temporary(typeof(string), typeof(string)),
                CompositionRegistration.Temporary(typeof(string), typeof(string)),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act + Assert
            Assert.Null(container.GetService(typeof(string)));
        }

        [Fact]
        public static void GetService_TemporaryInstanceOne_ReturnsNull()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Temporary(typeof(string), "Instance"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act + Assert
            Assert.Null(container.GetService(typeof(string)));
        }

        [Fact]
        public static void GetService_TemporaryInstanceMultiple_ReturnsNull()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Temporary(typeof(string), "Instance"),
                CompositionRegistration.Temporary(typeof(string), "Instance"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act + Assert
            Assert.Null(container.GetService(typeof(string)));
        }

        [Fact]
        public static void GetService_TemporaryFactoryOne_ReturnsNull()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Temporary(typeof(string), _ => "Factory"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act + Assert
            Assert.Null(container.GetService(typeof(string)));
        }

        [Fact]
        public static void GetService_TemporaryFactoryMultiple_ReturnsNull()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Temporary(typeof(string), _ => "Factory"),
                CompositionRegistration.Temporary(typeof(string), _ => "Factory"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act + Assert
            Assert.Null(container.GetService(typeof(string)));
        }

        [Fact]
        public static void GetService_SingletonTypeOne_ReturnsInstance()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Singleton(typeof(object), typeof(object)),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            object value = container.GetService(typeof(object));

            // Assert
            Assert.NotNull(value);
            Assert.IsType<object>(value);
        }

        [Fact]
        public static void GetService_SingletonTypeMultiple_ReturnsInstance()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Singleton(typeof(object), typeof(object)),
                CompositionRegistration.Singleton(typeof(object), typeof(object)),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            object value = container.GetService(typeof(object));

            // Assert
            Assert.NotNull(value);
            Assert.IsType<object>(value);
        }

        [Fact]
        public static void GetService_SingletonInstanceOne_ReturnsInstance()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Singleton(typeof(string), "Instance"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            string value = (string)container.GetService(typeof(string));

            // Assert
            Assert.NotNull(value);
            Assert.StrictEqual("Instance", value);
        }

        [Fact]
        public static void GetService_SingletonInstanceMultiple_ReturnsFirstInstance()
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
            string value = (string)container.GetService(typeof(string));

            // Assert
            Assert.NotNull(value);
            Assert.StrictEqual("Instance1", value);
        }

        [Fact]
        public static void GetService_SingletonFactoryOne_ReturnsInstance()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Singleton(typeof(string), _ => "Factory"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            string value = (string)container.GetService(typeof(string));

            // Assert
            Assert.NotNull(value);
            Assert.StrictEqual("Factory", value);
        }

        [Fact]
        public static void GetService_SingletonFactoryMultiple_ReturnsFirstInstance()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Singleton(typeof(string), _ => "Factory1"),
                CompositionRegistration.Singleton(typeof(string), _ => "Factory2"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            string value = (string)container.GetService(typeof(string));

            // Assert
            Assert.NotNull(value);
            Assert.StrictEqual("Factory1", value);
        }

        [Fact]
        public static void GetService_TransientTypeOne_ReturnsInstance()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Transient(typeof(object), typeof(object)),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            object value = container.GetService(typeof(object));

            // Assert
            Assert.NotNull(value);
            Assert.IsType<object>(value);
        }

        [Fact]
        public static void GetService_TransientTypeMultiple_ReturnsInstance()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Transient(typeof(object), typeof(object)),
                CompositionRegistration.Transient(typeof(object), typeof(object)),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            object value = container.GetService(typeof(object));

            // Assert
            Assert.NotNull(value);
            Assert.IsType<object>(value);
        }

        [Fact]
        public static void GetService_TransientFactoryOne_ReturnsInstance()
        {
            // Arrange
            CompositionRegistration[] regs = new[]
            {
                CompositionRegistration.Transient(typeof(string), _ => "Factory"),
            };
            SimpleContainer container = new SimpleContainer();
            container.Register(regs);

            // Act
            string value = (string)container.GetService(typeof(string));

            // Assert
            Assert.NotNull(value);
            Assert.StrictEqual("Factory", value);
        }

        [Fact]
        public static void GetService_TransientFactoryMultiple_ReturnsFirstInstance()
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
            string value = (string)container.GetService(typeof(string));

            // Assert
            Assert.NotNull(value);
            Assert.StrictEqual("Factory1", value);
        }
    }
}
