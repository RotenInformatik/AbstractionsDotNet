using System;

using Microsoft.Extensions.DependencyInjection;

using RI.Abstractions.Composition;




namespace RI.Abstractions.Builder
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IBuilder" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class ServiceCollectionBuilderExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Adds registrations for using a service collection as composition container.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="services"> The service collection to use. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="services" /> is null. </exception>
        public static T UseServiceCollection<T>(this T builder, IServiceCollection services)
            where T : IBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            builder.AddTemporary(typeof(ICompositionContainer), new ServiceCollectionCompositionContainer(services));

            return builder;
        }

        #endregion
    }
}
