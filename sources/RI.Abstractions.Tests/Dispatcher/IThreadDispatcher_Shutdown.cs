using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_Shutdown
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Shutdown_DiscardOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            int count = 0;

            for (int i1 = 0; i1 < 5; i1++)
            {
                instance.Post(new Action(() =>
                {
                    Thread.Sleep(100);
                    count++;
                }));
            }

            instance.Shutdown(ThreadDispatcherShutdownMode.DiscardPending);

            // Assert

            Assert.True(count < 5);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Shutdown_DiscardSameThread_InvalidOperationException(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act + Assert

            instance.Send(new Action(() =>
            {
                Assert.Throws<InvalidOperationException>(() => instance.Shutdown(ThreadDispatcherShutdownMode.DiscardPending));
            }));

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Shutdown_FinishOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act
            int count = 0;

            for (int i1 = 0; i1 < 5; i1++)
            {
                instance.Post(new Action(() =>
                {
                    count++;
                }));
            }

            instance.Shutdown(ThreadDispatcherShutdownMode.FinishPending);

            // Assert

            Assert.Equal(5, count);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Shutdown_FinishSameThread_InvalidOperationException(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act + Assert

            instance.Send(new Action(() =>
            {
                Assert.Throws<InvalidOperationException>(() => instance.Shutdown(ThreadDispatcherShutdownMode.FinishPending));
            }));

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }
    }
}
