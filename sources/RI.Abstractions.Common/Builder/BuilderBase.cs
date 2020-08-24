using System;
using System.Collections.Generic;
using System.Linq;

using RI.Abstractions.Composition;
using RI.Abstractions.Logging;




namespace RI.Abstractions.Builder
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IBuilder" />.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         When building (calling <see cref="Build" />), <see cref="BuilderBase" /> expects exactly one instance of <see cref="ILogger" />, one instance of <see cref="ICompositionContainer" />, and zero instances of <see cref="IBuilder" /> already registered.
    ///     </note>
    ///     <para>
    ///         <see cref="BuilderBase" /> adds itself as a <see cref="CompositionRegistrationMode.Temporary" /> registration with <see cref="IBuilder" /> as the contract.
    ///     </para>
    ///     <para>
    ///         During <see cref="Build" />, after <see cref="PrepareRegistrations" /> but before <see cref="PerformRegistrations" /> is called, all <see cref="CompositionRegistrationMode.Temporary" />, <see cref="ILogger" />, and <see cref="ICompositionContainer" /> registrations will be removed.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class BuilderBase : IBuilder
    {
        #region Instance Fields

        private readonly List<CompositionRegistration> _registrations = new List<CompositionRegistration>();

        #endregion




        #region Virtuals

        /// <summary>
        ///     Called to perform the actual registration with the used composition container.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="compositionContainer"> The used composition container. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The default implementation calls <see cref="ICompositionContainer.Register" /> on <paramref name="compositionContainer" />.
        ///     </note>
        /// </remarks>
        protected virtual void PerformRegistrations (ILogger logger, ICompositionContainer compositionContainer)
        {
            compositionContainer.Register(this.Registrations);
        }

        /// <summary>
        ///     Called to perform the builder-specific registration check and completion (if any).
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The default implementation does nothing.
        ///     </note>
        /// </remarks>
        protected virtual void PrepareRegistrations (ILogger logger) { }

        #endregion




        #region Interface: IBuilder

        /// <inheritdoc />
        public bool AlreadyBuilt { get; private set; }

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
            this.ThrowIfAlreadyBuilt();

            try
            {
                this.ThrowIfNotExactContractCount(typeof(IBuilder), 0);
                this.AddTemporary(typeof(IBuilder), this);

                this.ThrowIfNotExactContractCount(typeof(ILogger), 1);
                ILogger logger = this.GetInstance<ILogger>();

                this.ThrowIfNotExactContractCount(typeof(ICompositionContainer), 1);
                ICompositionContainer compositionContainer = this.GetInstance<ICompositionContainer>();

                this.PrepareRegistrations(logger);

                logger.LogInformation(this.GetType()
                                          .Name, null, $"Registrations:{Environment.NewLine}{string.Join(Environment.NewLine, this.Registrations.Select(x => x.ToString()))}");

                this.RemoveContracts(typeof(ICompositionContainer));
                this.RemoveContracts(typeof(IBuilder));
                this.RemoveTemporaryContracts();

                this.PerformRegistrations(logger, compositionContainer);

                this.AlreadyBuilt = true;
            }
            catch (BuilderException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new BuilderException(exception);
            }
        }

        #endregion
    }
}
