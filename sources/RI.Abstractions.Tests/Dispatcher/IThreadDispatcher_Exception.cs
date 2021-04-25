using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_Exception
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Exception_DontCatch_ThreadDispatcherException(IThreadDispatcher instance)
        {
            // Arrange

            bool? canContinue = null;
            instance.CatchExceptions = false;

            instance.Exception += (sender, args) =>
            {
                canContinue = args.CanContinue;
            };
            
            // Act + Assert

            Assert.Throws<ThreadDispatcherException>(() =>
            {
                instance.Post(new Action(() => throw new Exception()));
                instance.Post(new Action(() =>
                {
                    canContinue = true;
                }));
                instance.Run();
            });

            Assert.False(canContinue.Value);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Exception_Catch_NoException(IThreadDispatcher instance)
        {
            // Arrange

            bool? canContinue = null;
            instance.CatchExceptions = true;

            instance.Exception += (sender, args) =>
            {
                canContinue = args.CanContinue;
            };

            // Act + Assert

            instance.Post(new Action(() => throw new Exception()));
            instance.Post(new Action(() => instance.BeginShutdown(ThreadDispatcherShutdownMode.FinishPending)));
            instance.Run();

            Assert.True(canContinue.Value);
        }
    }
}
