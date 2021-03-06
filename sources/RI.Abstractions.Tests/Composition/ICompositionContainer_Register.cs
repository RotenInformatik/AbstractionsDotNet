﻿using System;
using System.Collections.Generic;

using RI.Abstractions.Composition;

using Xunit;




namespace RI.Abstractions.Tests.Composition
{
    public sealed class ICompositionContainer_Register
    {
        [Theory]
        [MemberData(nameof(ICompositionContainer_Register.GetContainers))]
        public void Register_Null_ArgumentNullException(ICompositionContainer instance)
        {
            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => instance.Register(null));
        }

        [Theory]
        [MemberData(nameof(ICompositionContainer_Register.GetContainers))]
        public void Register_Empty_NoException(ICompositionContainer instance)
        {
            // Act
            instance.Register(new CompositionRegistration[0]);
        }

        [Theory]
        [MemberData(nameof(ICompositionContainer_Register.GetContainersAndRegistrations))]
        public void Register_ValidRegistrations_NoException(ICompositionContainer instance, CompositionRegistration[] validRegistrations)
        {

            // Act + Assert
            Assert.False(instance.Registered);
            instance.Register(validRegistrations);
            Assert.True(instance.Registered);
        }

        [Theory]
        [MemberData(nameof(ICompositionContainer_Register.GetContainers))]
        public void Register_Reregister_InvalidOperationException(ICompositionContainer instance)
        {
            // Act + Assert
            instance.Register(new CompositionRegistration[0]);
            Assert.Throws<InvalidOperationException>(() => instance.Register(new CompositionRegistration[0]));
        }

        public static IEnumerable<object[]> GetContainers() =>
            _CompositionTestFactory.GetContainers();

        public static IEnumerable<object[]> GetContainersAndRegistrations() =>
            _CompositionTestFactory.GetContainersAndRegistrations();
    }
}
