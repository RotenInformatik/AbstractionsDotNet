using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RI.Abstractions.Builder;




namespace RI.Abstractions.Composition
{
    /// <summary>
    ///     Simple composition container abstraction implementation with basic dependency injection functionality.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         <see cref="SimpleContainer" /> does not provide the same functionality as full-fledged dependency injection libraries.
    ///         As the name suggests, it provides only simple composition container and dependency injection functionality.
    ///     </note>
    ///     <note type="important">
    ///         <see cref="SimpleContainer" /> only provides the following functionality:
    ///         Support for <see cref="IEnumerable{T}" /> to get multiple instances; constructors with or without parameters.
    ///     </note>
    ///     <note type="important">
    ///         Per <see cref="SimpleContainer" /> instance, <see cref="Register" /> can only be called once and <see cref="GetService" /> and <see cref="GetServices" /> can only be called after <see cref="Register" />.
    ///     </note>
    ///     <note type="note">
    ///         If multiple constructors are available, the one of which the most parameters can be resolved will be used (best match).
    ///         If there is no best match constructor, an exception is thrown.
    ///         If there are multiple best match constructors, an exception is thrown.
    ///     </note>
    ///     <note type="note">
    ///         <see cref="SimpleContainer" /> is used internally by <see cref="IBuilderExtensions.BuildStandalone" />.
    ///     </note>
    ///     <note type="note">
    ///         <see cref="CompositionRegistrationMode.Temporary" /> registrations are ignored.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SimpleContainer : IServiceProvider, ICompositionContainer
    {
        #region Instance Properties/Indexer

        private Dictionary<Type, List<object>> Cache { get; } = new Dictionary<Type, List<object>>();

        private List<CompositionRegistration> Registrations { get; set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets all service objects of the specified type.
        /// </summary>
        /// <param name="serviceType"> An object that specifies the type of service object to get. </param>
        /// <returns> A list with objects of type <paramref name="serviceType" /> or an empty list if there is no service object of type <paramref name="serviceType" />. </returns>
        public IList<object> GetServices (Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (this.Registrations == null)
            {
                throw new InvalidOperationException("No service registrations.");
            }

            if (serviceType.IsGenericType)
            {
                if (serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    serviceType = serviceType.GetGenericArguments()[0];
                }
            }

            if (this.Cache.ContainsKey(serviceType))
            {
                return new List<object>(this.Cache[serviceType]);
            }

            if (this.Registrations.All(x => x.Contract != serviceType))
            {
                this.Cache.Add(serviceType, new List<object>());
                return new List<object>();
            }

            List<object> instances = new List<object>();

            List<CompositionRegistration> registrations = this.Registrations.Where(x => x.Contract == serviceType)
                                                              .ToList();

            foreach (CompositionRegistration registration in registrations)
            {
                //TODO: Recursive detection
                instances.Add(this.CreateInstance(registration, serviceType));
            }

            this.Cache.Add(serviceType, new List<object>(instances));
            return instances;
        }

        private object ConstructInstance (Type implementation, Type requestedType)
        {
            List<ConstructorInfo> constructors = implementation.GetConstructors(BindingFlags.Public)
                                                               .ToList();

            constructors.Sort((x, y) => x.GetParameters()
                                         .Length.CompareTo(y.GetParameters()
                                                            .Length));

            constructors.Reverse();

            List<ConstructorInfo> candidates = new List<ConstructorInfo>();

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                bool validCandidate = true;

                foreach (ParameterInfo parameter in parameters)
                {
                    Type type = parameter.ParameterType;

                    if (type.IsGenericType)
                    {
                        if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        {
                            type = type.GetGenericArguments()[0];
                        }
                    }

                    if (this.Registrations.All(x => x.Contract != type))
                    {
                        validCandidate = false;
                        break;
                    }
                }

                if (validCandidate)
                {
                    candidates.Add(constructor);
                }
            }

            if (candidates.Count == 0)
            {
                throw new NotSupportedException($"No best match constructor found for {implementation.Name}");
            }

            if (candidates.Count != 1)
            {
                throw new NotSupportedException($"Multiple best match constructors found for {implementation.Name}");
            }

            ConstructorInfo ctr = candidates[0];
            ParameterInfo[] parameterTypes = ctr.GetParameters();
            object[] parameterInstances = new object[parameterTypes.Length];

            for (int i1 = 0; i1 < parameterTypes.Length; i1++)
            {
                ParameterInfo parameterType = parameterTypes[i1];

                if ((parameterType.ParameterType == implementation) || (parameterType.ParameterType == requestedType))
                {
                    throw new NotSupportedException($"{implementation.Name} cannot have itself as the type of a constructor parameter.");
                }

                parameterInstances[i1] = this.GetService(parameterType.ParameterType);
            }

            return ctr.Invoke(parameterInstances);
        }

        private object CreateInstance (CompositionRegistration registration, Type requestedType)
        {
            object instance;

            if ((registration.Instance != null) && (registration.Mode != CompositionRegistrationMode.Transient))
            {
                instance = registration.Instance;
            }
            else if (registration.Factory != null)
            {
                instance = registration.Factory(this);
            }
            else if (registration.Implementation != null)
            {
                instance = this.ConstructInstance(registration.Implementation, requestedType);
            }
            else
            {
                throw new NotSupportedException($"Registration not supported (invalid registration): {registration}");
            }

            if (instance == null)
            {
                throw new NotSupportedException($"Registration not supported (construction returned null): {registration}");
            }

            return instance;
        }

        #endregion




        #region Interface: ICompositionContainer

        /// <inheritdoc />
        public void Register (IEnumerable<CompositionRegistration> registrations)
        {
            if (registrations == null)
            {
                throw new ArgumentNullException(nameof(registrations));
            }

            if (this.Registrations != null)
            {
                throw new InvalidOperationException("Services already registered.");
            }

            this.Registrations = new List<CompositionRegistration>(registrations.Where(x => x.Mode != CompositionRegistrationMode.Temporary));
        }

        #endregion




        #region Interface: IServiceProvider

        /// <inheritdoc />
        public object GetService (Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (this.Registrations == null)
            {
                throw new InvalidOperationException("No service registrations.");
            }

            if (serviceType.IsGenericType)
            {
                if (serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    serviceType = serviceType.GetGenericArguments()[0];
                    return this.GetServices(serviceType);
                }
            }

            return this.GetServices(serviceType)
                       .FirstOrDefault();
        }

        #endregion
    }
}
