using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_SendAsync
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task SendAsync_SyncOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act
            Task<object> task = instance.SendAsync(new Func<int, int>((a) => a), 42);
            int result = (int)await task;

            // Assert
            Assert.Equal(42, result);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task SendAsync_SyncSameThread_Success(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act
            int result = (int) await instance.SendAsync(new Func<Task<int>>(async () =>
            {
                return (int) instance.Send(new Func<int, int>((a) => a), 42);
            }));

            // Assert
            Assert.Equal(42, result);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task SendAsync_AsyncSameThread_InvalidOperationException(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act
            int result = (int)await instance.SendAsync(new Func<Task<int>>(async () =>
            {
                Assert.ThrowsAsync<InvalidOperationException>(async () => await instance.SendAsync(new Action(() => { })));
                return 42;
            }));

            // Assert
            Assert.Equal(42, result);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }
    }
}
