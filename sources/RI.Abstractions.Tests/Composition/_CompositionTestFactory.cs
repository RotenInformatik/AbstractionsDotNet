using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using RI.Abstractions.Composition;




namespace RI.Abstractions.Tests.Composition
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class _CompositionTestFactory
    {
        public static IEnumerable<object[]> GetContainers() =>
            new List<object[]>
            {
                _CompositionTestFactory.GetSimpleContainer(),
                _CompositionTestFactory.GetServiceCollectionContainer(),
            };

        public static object[] GetSimpleContainer()
        {
            return new object[]
            {
                new SimpleContainer(),
            };
        }

        public static object[] GetServiceCollectionContainer()
        {
            return new object[]
            {
                new ServiceCollectionCompositionContainer(new ServiceCollection()),
            };
        }

        public static IEnumerable<object[]> GetContainersAndRegistrations()
        {
            List<object[]> data = new List<object[]>();

            foreach (object[] container in _CompositionTestFactory.GetContainers())
            {
                object[] combined = new object[container.Length + 1];
                int combinedIndex = 0;

                foreach (object containerData in container)
                {
                    combined[combinedIndex] = containerData;
                    combinedIndex++;
                }

                combined[combinedIndex] = _CompositionTestFactory.GetValidRegistrations();
                
                data.Add(combined);
            }

            return data;
        }

        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        public static CompositionRegistration[] GetValidRegistrations () =>
            new[]
            {
                CompositionRegistration.Singleton(typeof(string), typeof(string), true),
                CompositionRegistration.Singleton(typeof(string), "Instance", true),
                CompositionRegistration.Singleton(typeof(string), _ => "Factory", true),

                CompositionRegistration.Singleton(typeof(string), typeof(string), false),
                CompositionRegistration.Singleton(typeof(string), "Instance", false),
                CompositionRegistration.Singleton(typeof(string), _ => "Factory", false),

                CompositionRegistration.Temporary(typeof(string), typeof(string)),
                CompositionRegistration.Temporary(typeof(string), "Instance"),
                CompositionRegistration.Temporary(typeof(string), _ => "Factory"),

                CompositionRegistration.Transient(typeof(string), typeof(string), true),
                CompositionRegistration.Transient(typeof(string), _ => "Factory", true),

                CompositionRegistration.Transient(typeof(string), typeof(string), false),
                CompositionRegistration.Transient(typeof(string), _ => "Factory", false),
            };
    }
}
