using System;
using System.Collections.Generic;

using RI.Abstractions.Composition;




namespace RI.Abstractions.Builder
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IBuilder"/>.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class BuilderBase : IBuilder
    {
        private readonly List<CompositionRegistration> _registrations = new List<CompositionRegistration>();

        /// <inheritdoc />
        public bool AlreadyBuilt { get; private set; } = false;

        /// <inheritdoc />
        public List<CompositionRegistration> Registrations
        {
            get
            {
                this.ThrowIfAlreadyBuilt();
                return this._registrations;
            }
        }

        /// <inheritdoc />
        public void Build ()
        {

        }

        protected abstract void PrepareRegistrations ();

        protected abstract void PerformRegistrations ();
    }
}
