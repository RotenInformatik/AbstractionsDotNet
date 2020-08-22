using System;

using RI.Abstractions.Builder;




namespace RI.Abstractions.Composition
{
    /// <summary>
    ///     Indicates the registration mode of a <see cref="CompositionRegistration" /> in a <see cref="ICompositionContainer"/>.
    /// </summary>
    [Serializable,]
    public enum CompositionRegistrationMode
    {
        /// <summary>
        ///     The service is registered as a singleton (one instance shared with all instance requests).
        /// </summary>
        Singleton = 0,

        /// <summary>
        ///     The service is registered as a transient (one unique instance per instance requests).
        /// </summary>
        Transient = 1,

        /// <summary>
        ///     The service is not registered (e.g. when only exclusively used by a <see cref="IBuilder"/> implementation during the build stage).
        /// </summary>
        Temporary = 2,
    }
}
