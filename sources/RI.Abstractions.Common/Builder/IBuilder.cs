using System;
using System.Collections.Generic;

using RI.Abstractions.Composition;




namespace RI.Abstractions.Builder
{
    /// <summary>
    /// Builder abstraction.
    /// </summary>
    /// <remarks>
    ///     <para>
    /// <see cref="IBuilder"/> is used to construct and configure complex object/service setups.
    /// </para>
    /// <para>
    ///<see cref="IBuilder"/> is intended for one-time use, meaning that a builder instance can only perform construction and configuration only once (using <see cref="Build"/>).
    /// </para>
    /// </remarks>
    public interface IBuilder
    {
        /// <summary>
        ///     Gets whether <see cref="Build" /> has already been called.
        /// </summary>
        /// <value>
        /// true if <see cref="Build"/> was called, false otherwise.
        /// </value>
        bool AlreadyBuilt { get; }

        /// <summary>
        ///     Gets the list of all the object/service registrations.
        /// </summary>
        /// <value>
        /// The list of all the object/service registrations.
        /// </value>
        /// <exception cref="InvalidOperationException"> <see cref="Build" /> has already been called. </exception>
        List<CompositionRegistration> Registrations { get; }

        /// <summary>
        ///     Finishes the configuration and registers all necessary objects/services in the used composition container to construct the intended object/service setup.
        /// </summary>
        /// <exception cref="InvalidOperationException"> <see cref="Build" /> has already been called. </exception>
        /// <exception cref="BuilderException"> Configuration or registration of objects/services failed. </exception>
        void Build();
    }
}
