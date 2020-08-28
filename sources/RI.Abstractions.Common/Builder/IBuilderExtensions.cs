using System;
using System.Collections.Generic;
using System.Linq;

using RI.Abstractions.Composition;
using RI.Abstractions.Logging;




namespace RI.Abstractions.Builder
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IBuilder" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class IBuilderExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Adds a singleton registration specifying an implementation type if the specified contract is not already registered.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <returns> The added registration or null if the specified contract is already registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="implementation" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddDefaultSingleton (this IBuilder builder, Type contract, Type implementation)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(contract) != 0)
            {
                return null;
            }

            return builder.AddDefaultSingleton(contract, implementation);
        }

        /// <summary>
        ///     Adds a registration specifying a factory if the specified contract is not already registered.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <returns> The added registration or null if the specified contract is already registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="factory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddDefaultSingleton (this IBuilder builder, Type contract, Func<IServiceProvider, object> factory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(contract) != 0)
            {
                return null;
            }

            return builder.AddDefaultSingleton(contract, factory);
        }

        /// <summary>
        ///     Adds a singleton registration specifying an implementation instance if the specified contract is not already registered.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="instance"> The implementation instance. </param>
        /// <returns> The added registration or null if the specified contract is already registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="instance" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddDefaultSingleton (this IBuilder builder, Type contract, object instance)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(contract) != 0)
            {
                return null;
            }

            return builder.AddDefaultSingleton(contract, instance);
        }

        /// <summary>
        ///     Adds a temporary registration specifying an implementation type if the specified contract is not already registered.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <returns> The added registration or null if the specified contract is already registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="implementation" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddDefaultTemporary (this IBuilder builder, Type contract, Type implementation)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(contract) != 0)
            {
                return null;
            }

            return builder.AddDefaultTemporary(contract, implementation);
        }

        /// <summary>
        ///     Adds a temporary registration specifying a factory if the specified contract is not already registered.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <returns> The added registration or null if the specified contract is already registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="factory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddDefaultTemporary (this IBuilder builder, Type contract, Func<IServiceProvider, object> factory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(contract) != 0)
            {
                return null;
            }

            return builder.AddDefaultTemporary(contract, factory);
        }

        /// <summary>
        ///     Adds a temporary registration specifying an implementation instance if the specified contract is not already registered.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="instance"> The implementation instance. </param>
        /// <returns> The added registration or null if the specified contract is already registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="instance" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddDefaultTemporary (this IBuilder builder, Type contract, object instance)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(contract) != 0)
            {
                return null;
            }

            return builder.AddDefaultTemporary(contract, instance);
        }

        /// <summary>
        ///     Adds a transient registration specifying an implementation type if the specified contract is not already registered.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <returns> The added registration or null if the specified contract is already registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="implementation" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddDefaultTransient (this IBuilder builder, Type contract, Type implementation)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(contract) != 0)
            {
                return null;
            }

            return builder.AddDefaultTransient(contract, implementation);
        }

        /// <summary>
        ///     Adds a transient registration specifying a factory if the specified contract is not already registered.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <returns> The added registration or null if the specified contract is already registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="factory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddDefaultTransient (this IBuilder builder, Type contract, Func<IServiceProvider, object> factory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(contract) != 0)
            {
                return null;
            }

            return builder.AddDefaultTransient(contract, factory);
        }

        /// <summary>
        ///     Adds a singleton registration specifying an implementation type.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The added registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="implementation" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddSingleton (this IBuilder builder, Type contract, Type implementation, bool alwaysRegister = true)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration registration = CompositionRegistration.Singleton(contract, implementation, alwaysRegister);
            builder.Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        ///     Adds a registration specifying a factory.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The added registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="factory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddSingleton (this IBuilder builder, Type contract, Func<IServiceProvider, object> factory, bool alwaysRegister = true)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration registration = CompositionRegistration.Singleton(contract, factory, alwaysRegister);
            builder.Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        ///     Adds a singleton registration specifying an implementation instance.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="instance"> The implementation instance. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The added registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="instance" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddSingleton (this IBuilder builder, Type contract, object instance, bool alwaysRegister = true)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration registration = CompositionRegistration.Singleton(contract, instance, alwaysRegister);
            builder.Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        ///     Adds a temporary registration specifying an implementation type.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <returns> The added registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="implementation" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddTemporary (this IBuilder builder, Type contract, Type implementation)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration registration = CompositionRegistration.Temporary(contract, implementation);
            builder.Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        ///     Adds a temporary registration specifying a factory.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <returns> The added registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="factory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddTemporary (this IBuilder builder, Type contract, Func<IServiceProvider, object> factory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration registration = CompositionRegistration.Temporary(contract, factory);
            builder.Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        ///     Adds a temporary registration specifying an implementation instance.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="instance"> The implementation instance. </param>
        /// <returns> The added registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="instance" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddTemporary (this IBuilder builder, Type contract, object instance)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration registration = CompositionRegistration.Temporary(contract, instance);
            builder.Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        ///     Adds a transient registration specifying an implementation type.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The added registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="implementation" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddTransient (this IBuilder builder, Type contract, Type implementation, bool alwaysRegister = true)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration registration = CompositionRegistration.Transient(contract, implementation, alwaysRegister);
            builder.Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        ///     Adds a transient registration specifying a factory.
        /// </summary>
        /// <param name="builder"> The builder being used. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The added registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" />, <paramref name="contract" />, or <paramref name="factory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration AddTransient (this IBuilder builder, Type contract, Func<IServiceProvider, object> factory, bool alwaysRegister = true)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration registration = CompositionRegistration.Transient(contract, factory, alwaysRegister);
            builder.Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        ///     Finishes the configuration and registers all necessary objects/services in an independent and standalone container to construct the intended object/service setup.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <returns> The container which can be used to access the built objects/services. </returns>
        /// <exception cref="InvalidOperationException"> <see cref="IBuilder.Build" /> or <see cref="BuildStandalone" /> has already been called. </exception>
        /// <exception cref="BuilderException"> Configuration or registration of objects/services failed. </exception>
        public static IServiceProvider BuildStandalone (this IBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ThrowIfAlreadyBuilt();

            if (builder.CountContracts(typeof(ICompositionContainer)) != 0)
            {
                throw new BuilderException($"{builder.GetType().Name} cannot have a registered {nameof(ICompositionContainer)} when using {nameof(IBuilderExtensions.BuildStandalone)}.");
            }

            SimpleContainer container = new SimpleContainer();
            builder.UseSimpleContainer(container);
            builder.Build();
            return container;
        }

        /// <summary>
        ///     Counts the number of registrations for a specified contract.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <returns> The number of registrations for <paramref name="contract" />. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static int CountContracts (this IBuilder builder, Type contract)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            builder.ThrowIfAlreadyBuilt();

            return builder.Registrations.Count(x => x.Contract == contract);
        }

        /// <summary>
        ///     Gets the first registration of a specified contract.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <returns> The registration or null if the contract is not registered. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static CompositionRegistration GetContract (this IBuilder builder, Type contract)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            builder.ThrowIfAlreadyBuilt();

            return builder.Registrations.FirstOrDefault(x => x.Contract == contract);
        }

        /// <summary>
        ///     Gets all registrations of the specified contract.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <returns> The list of all registrations of the contract. If no contracts are registered, an empty list is returned. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static IList<CompositionRegistration> GetContracts (this IBuilder builder, Type contract)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            builder.ThrowIfAlreadyBuilt();

            return builder.Registrations.Where(x => x.Contract == contract)
                          .ToList();
        }

        /// <summary>
        ///     Gets an instance from the registrations of a an <see cref="IBuilder" />.
        /// </summary>
        /// <typeparam name="TContract"> The type of the requested instance. </typeparam>
        /// <param name="builder"> The builder. </param>
        /// <returns> The requested instance or null if no registration for the specified contract was found. </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The instance construction is limited as it only supports basic construction of instances, either by using a registered instance, factory method, or parameterless constructor.
        ///         Constructors which have parameters are not supported.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and instances should no longer be retrieved from the registrations. </exception>
        /// <exception cref="NotSupportedException"> The resolved registration does not support instance creation (e.g. does not have a parameterless constructor, the creation threw an exception, etc.) </exception>
        public static TContract GetInstance <TContract> (this IBuilder builder)
            where TContract : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ThrowIfAlreadyBuilt();

            ServiceProviderWrapper serviceProvider = new ServiceProviderWrapper(builder);
            return (TContract)serviceProvider.GetService(typeof(TContract));
        }

        /// <summary>
        ///     Gets an instance from the registrations of a an <see cref="IBuilder" />.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The type of the requested instance. </param>
        /// <returns> The requested instance or null if no registration for the specified contract was found. </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The instance construction is limited as it only supports basic construction of instances, either by using a registered instance, factory method, or parameterless constructor.
        ///         Constructors which have parameters are not supported.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and instances should no longer be retrieved from the registrations. </exception>
        /// <exception cref="NotSupportedException"> The resolved registration does not support instance creation (e.g. does not have a parameterless constructor, the creation threw an exception, etc.) </exception>
        public static object GetInstance (this IBuilder builder, Type contract)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            builder.ThrowIfAlreadyBuilt();

            ServiceProviderWrapper serviceProvider = new ServiceProviderWrapper(builder);
            return serviceProvider.GetService(contract);
        }

        /// <summary>
        ///     Gets an instance from the registrations of a an <see cref="IBuilder" />.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="registration"> The registration. </param>
        /// <returns> The requested instance. </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The instance construction is limited as it only supports basic construction of instances, either by using a registered instance, factory method, or parameterless constructor.
        ///         Constructors which have parameters are not supported.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="registration" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and instances should no longer be retrieved from the registrations. </exception>
        /// <exception cref="NotSupportedException"> The resolved registration does not support instance creation (e.g. does not have a parameterless constructor, the creation threw an exception, etc.) </exception>
        public static object GetInstance (this IBuilder builder, CompositionRegistration registration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            builder.ThrowIfAlreadyBuilt();

            ServiceProviderWrapper serviceProvider = new ServiceProviderWrapper(builder);
            return serviceProvider.GetService(registration);
        }

        /// <summary>
        ///     Gets all instances from the registrations of a an <see cref="IBuilder" />.
        /// </summary>
        /// <typeparam name="TContract"> The type of the requested instance. </typeparam>
        /// <param name="builder"> The builder. </param>
        /// <returns> The list of instances or an empty list if no registration for the specified contract was found. </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The instance construction is limited as it only supports basic construction of instances, either by using a registered instance, factory method, or parameterless constructor.
        ///         Constructors which have parameters are not supported.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and instances should no longer be retrieved from the registrations. </exception>
        /// <exception cref="NotSupportedException"> The resolved registration does not support instance creation (e.g. does not have a parameterless constructor, the creation threw an exception, etc.) </exception>
        public static IList<TContract> GetInstances <TContract> (this IBuilder builder)
            where TContract : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ThrowIfAlreadyBuilt();

            ServiceProviderWrapper serviceProvider = new ServiceProviderWrapper(builder);

            return serviceProvider.GetServices(typeof(TContract))
                                  .Cast<TContract>()
                                  .ToList();
        }

        /// <summary>
        ///     Gets all instances from the registrations of a an <see cref="IBuilder" />.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The type of the requested instance. </param>
        /// <returns> The list of instances or an empty list if no registration for the specified contract was found. </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The instance construction is limited as it only supports basic construction of instances, either by using a registered instance, factory method, or parameterless constructor.
        ///         Constructors which have parameters are not supported.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and instances should no longer be retrieved from the registrations. </exception>
        /// <exception cref="NotSupportedException"> The resolved registration does not support instance creation (e.g. does not have a parameterless constructor, the creation threw an exception, etc.) </exception>
        public static IList<object> GetInstances (this IBuilder builder, Type contract)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            builder.ThrowIfAlreadyBuilt();

            ServiceProviderWrapper serviceProvider = new ServiceProviderWrapper(builder);
            return serviceProvider.GetServices(contract);
        }

        /// <summary>
        ///     Gets an <see cref="IServiceProvider" /> which can be used to access the registrations of an <see cref="IBuilder" />.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <returns> The <see cref="IServiceProvider" /> (one new instance for each call to <see cref="GetServiceProvider" />). </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The returned <see cref="IServiceProvider" /> is limited as it only supports basic construction of instances, either by using a registered instance, factory method, or parameterless constructor.
        ///         Constructors of services which have parameters are not supported.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and instances should no longer be retrieved from the registrations. </exception>
        public static IServiceProvider GetServiceProvider (this IBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ThrowIfAlreadyBuilt();

            return new ServiceProviderWrapper(builder);
        }

        /// <summary>
        ///     Removes all registrations of a specified contract type from the registrations.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <returns> The number of removed registrations or zero if no contracts were found to be removed. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static int RemoveContracts (this IBuilder builder, Type contract)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            builder.ThrowIfAlreadyBuilt();

            return builder.Registrations.RemoveAll(x => x.Contract == contract);
        }

        /// <summary>
        ///     Removes all registrations which are registered as <see cref="CompositionRegistrationMode.Temporary" />.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <returns> The number of removed registrations or zero if no temporary registrations were found to be removed. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        public static int RemoveTemporaryContracts (this IBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ThrowIfAlreadyBuilt();

            return builder.Registrations.RemoveAll(x => x.Mode == CompositionRegistrationMode.Temporary);
        }

        /// <summary>
        ///     Checks whether the specified builder has already been built and throws a <see cref="InvalidOperationException" /> if so.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built. </exception>
        public static void ThrowIfAlreadyBuilt (this IBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (builder.AlreadyBuilt)
            {
                throw new InvalidOperationException(builder.GetType()
                                                           .Name + " has already been built.");
            }
        }

        /// <summary>
        ///     Checks whether the specified builder has an exact amount of a specified contract registered or throws a <see cref="BuilderException" /> if not.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="exactCount"> The exact amount of registered contracts required. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="exactCount" /> is less than zero. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        /// <exception cref="BuilderException"> The builder does not contain the exact amount of contracts. </exception>
        public static void ThrowIfNotExactContractCount (this IBuilder builder, Type contract, int exactCount)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (exactCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exactCount));
            }

            builder.ThrowIfAlreadyBuilt();

            int actualCount = builder.CountContracts(contract);

            if (actualCount != exactCount)
            {
                throw new BuilderException($"{builder.GetType().Name} requires exact {exactCount} contracts of type {contract.Name} but has {actualCount}.");
            }
        }

        /// <summary>
        ///     Checks whether the specified builder has a maximum amount of a specified contract registered or throws a <see cref="BuilderException" /> if not.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="maxCount"> The maximum amount of registered contracts required. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="maxCount" /> is less than zero. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        /// <exception cref="BuilderException"> The builder does contain more than the maximum amount of contracts. </exception>
        public static void ThrowIfNotMaxContractCount (this IBuilder builder, Type contract, int maxCount)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (maxCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }

            builder.ThrowIfAlreadyBuilt();

            int actualCount = builder.CountContracts(contract);

            if (actualCount > maxCount)
            {
                throw new BuilderException($"{builder.GetType().Name} requires at most {maxCount} contracts of type {contract.Name} but has {actualCount}.");
            }
        }

        /// <summary>
        ///     Checks whether the specified builder has a minimum amount of a specified contract registered or throws a <see cref="BuilderException" /> if not.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <param name="minCount"> The minimum amount of registered contracts required. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="minCount" /> is less than zero. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        /// <exception cref="BuilderException"> The builder does not contain the minimum amount of contracts. </exception>
        public static void ThrowIfNotMinContractCount (this IBuilder builder, Type contract, int minCount)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (minCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minCount));
            }

            builder.ThrowIfAlreadyBuilt();

            int actualCount = builder.CountContracts(contract);

            if (actualCount < minCount)
            {
                throw new BuilderException($"{builder.GetType().Name} requires at least {minCount} contracts of type {contract.Name} but has {actualCount}.");
            }
        }

        /// <summary>
        ///     Checks whether the specified builder has a contract which is not registered as temporary and throws a <see cref="BuilderException" /> if so.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        /// <exception cref="BuilderException"> <paramref name="contract" /> is not registered as <see cref="CompositionRegistrationMode.Temporary" />. </exception>
        public static void ThrowIfNotTemporary (this IBuilder builder, Type contract)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration nonTemporaryRegistration = builder.Registrations
                                                                      .FirstOrDefault(x => x.Mode != CompositionRegistrationMode.Temporary);

            if (nonTemporaryRegistration != null)
            {
                throw new BuilderException($"{builder.GetType().Name} has a non-temporary registration of {contract.Name}: {nonTemporaryRegistration}");
            }
        }

        /// <summary>
        ///     Checks whether the specified builder has a contract which is registered as temporary and throws a <see cref="BuilderException" /> if so.
        /// </summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="contract"> The contract type. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="contract" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder" /> has already been built and subsequent registrations cannot be used. </exception>
        /// <exception cref="BuilderException"> <paramref name="contract" /> is registered as <see cref="CompositionRegistrationMode.Temporary" />. </exception>
        public static void ThrowIfTemporary (this IBuilder builder, Type contract)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            builder.ThrowIfAlreadyBuilt();

            CompositionRegistration nonTemporaryRegistration = builder.Registrations
                                                                      .FirstOrDefault(x => x.Mode == CompositionRegistrationMode.Temporary);

            if (nonTemporaryRegistration != null)
            {
                throw new BuilderException($"{builder.GetType().Name} has a temporary registration of {contract.Name}: {nonTemporaryRegistration}");
            }
        }

        /// <summary>
        ///     Adds registrations for using a simple callback logger.
        /// </summary>
        /// <typeparam name="T"> The type of the builder. </typeparam>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="callback"> The callback delegate to call for each log message to write. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="callback" /> is null. </exception>
        public static T UseCallbackLogger <T> (this T builder, CallbackLoggerDelegate callback)
            where T : IBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            builder.AddSingleton(typeof(ILogger), new CallbackLogger(callback));

            return builder;
        }

        /// <summary>
        ///     Adds registrations for using a simple composition container.
        /// </summary>
        /// <typeparam name="T"> The type of the builder. </typeparam>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="container"> The simple container to use. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="container" /> is null. </exception>
        public static T UseSimpleContainer <T> (this T builder, SimpleContainer container)
            where T : IBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            builder.AddTemporary(typeof(ICompositionContainer), container);

            return builder;
        }

        #endregion




        #region Type: ServiceProviderWrapper

        private sealed class ServiceProviderWrapper : IServiceProvider
        {
            #region Instance Constructor/Destructor

            public ServiceProviderWrapper (IBuilder builder)
            {
                if (builder == null)
                {
                    throw new ArgumentNullException(nameof(builder));
                }

                this.Builder = builder;
            }

            #endregion




            #region Instance Properties/Indexer

            private IBuilder Builder { get; }

            #endregion




            #region Instance Methods

            public object GetService (CompositionRegistration registration)
            {
                if (registration == null)
                {
                    throw new ArgumentNullException(nameof(registration));
                }

                this.Builder.ThrowIfAlreadyBuilt();

                return registration.GetOrCreateInstance(this);
            }

            public List<object> GetServices (Type serviceType)
            {
                if (serviceType == null)
                {
                    throw new ArgumentNullException(nameof(serviceType));
                }

                this.Builder.ThrowIfAlreadyBuilt();

                List<object> instances = new List<object>();

                foreach (CompositionRegistration registration in this.Builder.Registrations)
                {
                    if (serviceType == registration.Contract)
                    {
                        instances.Add(this.GetService(registration));
                    }
                }

                return instances;
            }

            #endregion




            #region Interface: IServiceProvider

            public object GetService (Type serviceType)
            {
                if (serviceType == null)
                {
                    throw new ArgumentNullException(nameof(serviceType));
                }

                this.Builder.ThrowIfAlreadyBuilt();

                foreach (CompositionRegistration registration in this.Builder.Registrations)
                {
                    if (serviceType == registration.Contract)
                    {
                        return this.GetService(registration);
                    }
                }

                return null;
            }

            #endregion
        }

        #endregion
    }
}
