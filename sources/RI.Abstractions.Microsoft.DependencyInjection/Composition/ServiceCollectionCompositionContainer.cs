using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;




namespace RI.Abstractions.Composition
{
    /// <summary>
    ///     Composition container abstraction implementation for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class ServiceCollectionCompositionContainer : ICompositionContainer
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ServiceCollectionCompositionContainer" />.
        /// </summary>
        /// <param name="services"> The Service Collection to use. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="services" /> is null. </exception>
        public ServiceCollectionCompositionContainer (IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            this.UsedServiceCollection = services;

            this.Registered = false;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used Service Collection.
        /// </summary>
        /// <value>
        ///     The used Service Collection.
        /// </value>
        public IServiceCollection UsedServiceCollection { get; }

        #endregion




        #region Interface: ICompositionContainer

        /// <inheritdoc />
        public void Register (IEnumerable<CompositionRegistration> registrations)
        {
            if (registrations == null)
            {
                throw new ArgumentNullException(nameof(registrations));
            }

            if (this.Registered)
            {
                throw new InvalidOperationException("Services already registered.");
            }

            this.Registered = true;

            foreach (CompositionRegistration registration in registrations)
            {
                switch (registration)
                {
                    case { } r when (r.Mode == CompositionRegistrationMode.Transient) && (r.Implementation != null):
                        if (r.AlwaysRegister)
                        {
                            this.UsedServiceCollection.AddTransient(r.Contract, r.Implementation);
                        }
                        else
                        {
                            this.UsedServiceCollection.TryAddTransient(r.Contract, r.Implementation);
                        }

                        break;

                    case { } r when (r.Mode == CompositionRegistrationMode.Transient) && (r.Factory != null):
                        if (r.AlwaysRegister)
                        {
                            this.UsedServiceCollection.AddTransient(r.Contract, r.Factory);
                        }
                        else
                        {
                            this.UsedServiceCollection.TryAddTransient(r.Contract, r.Factory);
                        }

                        break;

                    case { } r when (r.Mode == CompositionRegistrationMode.Singleton) && (r.Implementation != null):
                        if (r.AlwaysRegister)
                        {
                            this.UsedServiceCollection.AddSingleton(r.Contract, r.Implementation);
                        }
                        else
                        {
                            this.UsedServiceCollection.TryAddSingleton(r.Contract, r.Implementation);
                        }

                        break;

                    case { } r when (r.Mode == CompositionRegistrationMode.Singleton) && (r.Factory != null):
                        if (r.AlwaysRegister)
                        {
                            this.UsedServiceCollection.AddSingleton(r.Contract, r.Factory);
                        }
                        else
                        {
                            this.UsedServiceCollection.TryAddSingleton(r.Contract, r.Factory);
                        }

                        break;

                    case { } r when (r.Mode == CompositionRegistrationMode.Singleton) && (r.Instance != null):
                        if (r.AlwaysRegister)
                        {
                            this.UsedServiceCollection.AddSingleton(r.Contract, r.Instance);
                        }
                        else
                        {
                            this.UsedServiceCollection.TryAddSingleton(r.Contract, _ => r.Instance);
                        }

                        break;

                    case { } r when r.Mode == CompositionRegistrationMode.Temporary:
                        break;

                    default:
                        throw new NotSupportedException($"{nameof(ServiceCollectionCompositionContainer)} does not support registration: {registration}");
                }
            }
        }

        /// <inheritdoc />
        public bool Registered { get; private set; }

        #endregion
    }
}
