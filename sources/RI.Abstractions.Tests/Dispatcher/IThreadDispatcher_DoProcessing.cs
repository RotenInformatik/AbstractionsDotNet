using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_DoProcessing
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task DoProcessing_SpecificSameThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            int count = 0;
            int finalCount = 0;

            for (int i1 = 0; i1 < 5; i1++)
            {
                instance.Post(i1, new Action(() =>
                {
                    Thread.Sleep(100);
                    count++;
                }));
            }

            instance.Post(5, new Action(() =>
            {
                instance.DoProcessing(2);
                finalCount = count;
            }));

            instance.DoProcessing();

            // Assert

            Assert.Equal(3, finalCount);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task DoProcessing_SpecificOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            int count = 0;

            for (int i1 = 0; i1 < 5; i1++)
            {
                instance.Post(i1, new Action(() =>
                {
                    Thread.Sleep(100);
                    count++;
                }));
            }

            instance.DoProcessing(2);

            // Assert

            Assert.Equal(3, count);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task DoProcessing_AllSameThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            int count = 0;

            for (int i1 = 0; i1 < 5; i1++)
            {
                instance.Post(i1, new Action(() =>
                {
                    Thread.Sleep(100);
                    count++;
                }));
            }

            instance.Post(5, new Action(() =>
            {
                instance.DoProcessing();
                Assert.Equal(5, count);
            }));

            instance.DoProcessing();

            // Assert

            Assert.Equal(5, count);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task DoProcessing_AllOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            int count = 0;

            for (int i1 = 0; i1 < 5; i1++)
            {
                instance.Post(i1, new Action(() =>
                {
                    Thread.Sleep(100);
                    count++;
                }));
            }

            instance.DoProcessing();

            // Assert

            Assert.Equal(5, count);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }
    }
}
