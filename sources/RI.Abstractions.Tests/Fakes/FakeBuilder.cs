using System;

using RI.Abstractions.Builder;
using RI.Abstractions.Composition;
using RI.Abstractions.Logging;




namespace RI.Abstractions.Tests.Fakes
{
    public sealed class FakeBuilder : BuilderBase
    {
        protected override void PerformRegistrations (ILogger logger, ICompositionContainer compositionContainer)
        {
            base.PerformRegistrations(logger, compositionContainer);
            this.OnPerformRegistrations?.Invoke(logger, compositionContainer);
        }

        protected override void PrepareRegistrations (ILogger logger, ICompositionContainer compositionContainer)
        {
            base.PrepareRegistrations(logger, compositionContainer);
            this.OnPrepareRegistrations?.Invoke(logger, compositionContainer);
        }

        public event Action<ILogger, ICompositionContainer> OnPerformRegistrations;

        public event Action<ILogger, ICompositionContainer> OnPrepareRegistrations;
    }
}
