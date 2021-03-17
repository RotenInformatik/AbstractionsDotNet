using System;




namespace RI.Abstractions.Composition
{
    /// <summary>
    ///     A single registration for a <see cref="ICompositionContainer" />.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class CompositionRegistration
    {
        #region Static Methods

        /// <summary>
        ///     Creates a singleton registration specifying an implementation type.
        /// </summary>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The created service registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="contract" /> or <paramref name="implementation" /> is null. </exception>
        public static CompositionRegistration Singleton (Type contract, Type implementation, bool alwaysRegister = true)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            return new CompositionRegistration
            {
                Contract = contract,
                Implementation = implementation,
                Factory = null,
                Instance = null,
                Mode = CompositionRegistrationMode.Singleton,
                AlwaysRegister = alwaysRegister,
            };
        }

        /// <summary>
        ///     Creates a singleton registration specifying a factory.
        /// </summary>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The created service registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="contract" /> or <paramref name="factory" /> is null. </exception>
        public static CompositionRegistration Singleton (Type contract, Func<IServiceProvider, object> factory, bool alwaysRegister = true)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return new CompositionRegistration
            {
                Contract = contract,
                Implementation = null,
                Factory = factory,
                Instance = null,
                Mode = CompositionRegistrationMode.Singleton,
                AlwaysRegister = alwaysRegister,
            };
        }

        /// <summary>
        ///     Creates a singleton registration specifying an implementation instance.
        /// </summary>
        /// <param name="contract"> The contract type. </param>
        /// <param name="instance"> The implementation instance. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The created service registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="contract" /> or <paramref name="instance" /> is null. </exception>
        public static CompositionRegistration Singleton (Type contract, object instance, bool alwaysRegister = true)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return new CompositionRegistration
            {
                Contract = contract,
                Implementation = null,
                Factory = null,
                Instance = instance,
                Mode = CompositionRegistrationMode.Singleton,
                AlwaysRegister = alwaysRegister,
            };
        }

        /// <summary>
        ///     Creates a registration which will not be registered, specifying an implementation type.
        /// </summary>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <returns> The created service registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="contract" /> or <paramref name="implementation" /> is null. </exception>
        public static CompositionRegistration Temporary (Type contract, Type implementation)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            return new CompositionRegistration
            {
                Contract = contract,
                Implementation = implementation,
                Factory = null,
                Instance = null,
                Mode = CompositionRegistrationMode.Temporary,
                AlwaysRegister = false,
            };
        }

        /// <summary>
        ///     Creates a registration which will not be registered, specifying a factory.
        /// </summary>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <returns> The created service registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="contract" /> or <paramref name="factory" /> is null. </exception>
        public static CompositionRegistration Temporary (Type contract, Func<IServiceProvider, object> factory)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return new CompositionRegistration
            {
                Contract = contract,
                Implementation = null,
                Factory = factory,
                Instance = null,
                Mode = CompositionRegistrationMode.Temporary,
                AlwaysRegister = false,
            };
        }

        /// <summary>
        ///     Creates a registration which will not be registered, specifying an implementation instance.
        /// </summary>
        /// <param name="contract"> The contract type. </param>
        /// <param name="instance"> The implementation instance. </param>
        /// <returns> The created service registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="contract" /> or <paramref name="instance" /> is null. </exception>
        public static CompositionRegistration Temporary (Type contract, object instance)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return new CompositionRegistration
            {
                Contract = contract,
                Implementation = null,
                Factory = null,
                Instance = instance,
                Mode = CompositionRegistrationMode.Temporary,
                AlwaysRegister = false,
            };
        }

        /// <summary>
        ///     Creates a transient registration specifying an implementation type.
        /// </summary>
        /// <param name="contract"> The contract type. </param>
        /// <param name="implementation"> The implementation type. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The created service registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="contract" /> or <paramref name="implementation" /> is null. </exception>
        public static CompositionRegistration Transient (Type contract, Type implementation, bool alwaysRegister = true)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            return new CompositionRegistration
            {
                Contract = contract,
                Implementation = implementation,
                Factory = null,
                Instance = null,
                Mode = CompositionRegistrationMode.Transient,
                AlwaysRegister = alwaysRegister,
            };
        }

        /// <summary>
        ///     Creates a transient registration specifying a factory.
        /// </summary>
        /// <param name="contract"> The contract type. </param>
        /// <param name="factory"> The factory. </param>
        /// <param name="alwaysRegister"> whether the registration should be registered even if the contract (<paramref name="contract" />) is already registered. </param>
        /// <returns> The created service registration. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="contract" /> or <paramref name="factory" /> is null. </exception>
        public static CompositionRegistration Transient (Type contract, Func<IServiceProvider, object> factory, bool alwaysRegister = true)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return new CompositionRegistration
            {
                Contract = contract,
                Implementation = null,
                Factory = factory,
                Instance = null,
                Mode = CompositionRegistrationMode.Transient,
                AlwaysRegister = alwaysRegister,
            };
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets whether the registration should be registered even if the contract (<see cref="Contract" />) is already registered in the used <see cref="ICompositionContainer" />.
        /// </summary>
        /// <value>
        ///     true if the registration should always be registered, false otherwise.
        /// </value>
        public bool AlwaysRegister { get; internal set; }

        /// <summary>
        ///     Gets the contract type.
        /// </summary>
        /// <value>
        ///     The contract type.
        /// </value>
        public Type Contract { get; internal set; }

        /// <summary>
        ///     Gets the factory.
        /// </summary>
        /// <value>
        ///     The factory or null if implementation type or implementation instance is used.
        /// </value>
        public Func<IServiceProvider, object> Factory { get; internal set; }

        /// <summary>
        ///     Gets the implementation type.
        /// </summary>
        /// <value>
        ///     The implementation type or null if factory or implementation instance is used.
        /// </value>
        public Type Implementation { get; internal set; }

        /// <summary>
        ///     Gets the implementation instance.
        /// </summary>
        /// <value>
        ///     The implementation instance or null if implementation type or factory is used.
        /// </value>
        public object Instance { get; internal set; }

        /// <summary>
        ///     Gets the registration mode.
        /// </summary>
        /// <value>
        ///     The registration mode.
        /// </value>
        public CompositionRegistrationMode Mode { get; internal set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Attempts to get or create the instance registered by this registration.
        /// </summary>
        /// <param name="serviceProviderForFactories"> An optional <see cref="IServiceProvider" /> which is forwarded to potential factory methods if applicable. </param>
        /// <returns> The instance. </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The instance creation capabilities are limited as it only supports basic construction of instances, either by using a registered instance, factory method, or parameterless constructor.
        ///         Constructors which have parameters are not supported.
        ///     </note>
        /// </remarks>
        /// <exception cref="NotSupportedException"> The registration does not support instance creation (e.g. does not have a parameterless constructor, the creation threw an exception, etc.) </exception>
        public object GetOrCreateInstance (IServiceProvider serviceProviderForFactories = null)
        {
            try
            {
                object instance;

                if ((this.Instance != null) && (this.Mode != CompositionRegistrationMode.Transient))
                {
                    instance = this.Instance;
                }
                else if ((this.Factory != null) && (this.Mode != CompositionRegistrationMode.Singleton))
                {
                    instance = this.Factory(serviceProviderForFactories);
                }
                else if ((this.Implementation != null) && (this.Mode != CompositionRegistrationMode.Singleton))
                {
                    instance = Activator.CreateInstance(this.Implementation);
                }
                else
                {
                    throw new NotSupportedException($"Registration not supported (invalid registration): {this}");
                }

                this.Instance = instance;
                this.Factory = null;
                this.Implementation = null;

                return instance;
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new NotSupportedException($"Registration not supported (instance creation failed): {this}", exception);
            }
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override string ToString ()
        {
            return $"{nameof(CompositionRegistration)}; Mode={this.Mode}; AlwaysRegister={this.AlwaysRegister}; Contract={this.Contract?.Name ?? "[null]"}; Implementation={this.Implementation?.Name ?? "[null]"}; Factory={this.Factory?.Method.DeclaringType?.Name ?? "[null]"}.{this.Factory?.Method.Name ?? "[null]"}; Instance={this.Instance?.ToString() ?? "[null]"}";
        }

        #endregion
    }
}
