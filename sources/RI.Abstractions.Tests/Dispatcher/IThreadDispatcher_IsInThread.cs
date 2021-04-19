using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_IsInThread
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task IsInThread_OtherThread_False(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act + Assert
            Assert.False(instance.IsInThread());
            instance.Send(new Action(() =>
            {
            }));
            Assert.False(instance.IsInThread());

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task IsInThread_SameThread_True(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act + Assert
            instance.Send(new Action(() =>
            {
                Assert.True(instance.IsInThread());
            }));

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }
    }
}
