using System;
using System.Collections.Generic;

using RI.Abstractions.Builder;




namespace RI.Abstractions.Composition
{
    /// <summary>
    ///     Composition container abstraction.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ICompositionContainer" /> is used to abstract away the used composition, container and/or dependency injection library and mechanism.
    ///     </para>
    ///     <para>
    ///         <see cref="ICompositionContainer" /> is intended to provide a mechanism (through <see cref="Register" />) for one-time registration of services (e.g. by a <see cref="IBuilder" /> implementation during the build stage)
    ///     </para>
    /// </remarks>
    public interface ICompositionContainer
    {
        /// <summary>
        ///     Registers the configured registrations in this composition container.
        /// </summary>
        /// <param name="registrations"> The sequence of configured registrations. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         <paramref name="registrations" /> should only be enumerated once.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="registrations" /> is null. </exception>
        /// <exception cref="NotSupportedException"> <paramref name="registrations" /> contains unsupported registrations not supported by this composition container. </exception>
        void Register (IEnumerable<CompositionRegistration> registrations);
    }
}
