using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_Send
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Send_SimpleOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act
            int result = (int)instance.Send(new Func<int, int>((a) => a), 42);
            
            // Assert
            Assert.Equal(42, result);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Send_SimpleSameThread_Success(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act
            int result = (int)instance.Send(new Func<int>(() =>
            {
                return (int)instance.Send(new Func<int, int>((a) => a), 42);
            }));

            // Assert
            Assert.Equal(42, result);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }
    }
}
