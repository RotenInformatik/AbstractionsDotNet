using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Composition;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_BeginShutdown
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task BeginShutdown_DiscardOtherThread_Success(IThreadDispatcher instance)
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

            instance.BeginShutdown(ThreadDispatcherShutdownMode.DiscardPending);
            await instance.WaitForShutdownAsync();
            instance.WaitForShutdown();

            // Assert
            Assert.True(count < 5);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task BeginShutdown_DiscardSameThread_Success(IThreadDispatcher instance)
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

            ManualResetEvent mre = new ManualResetEvent(false);

            instance.Post(1, new Action(() =>
            {
                instance.BeginShutdown(ThreadDispatcherShutdownMode.DiscardPending);
                mre.Set();
            }));

            mre.WaitOne();
            await instance.WaitForShutdownAsync();
            instance.WaitForShutdown();

            // Assert
            Assert.True(count < 5);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task BeginShutdown_FinishOtherThread_Success(IThreadDispatcher instance)
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

            instance.BeginShutdown(ThreadDispatcherShutdownMode.FinishPending);
            instance.WaitForShutdown();
            await instance.WaitForShutdownAsync();

            // Assert
            Assert.True(count == 5);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task BeginShutdown_FinishSameThread_Success(IThreadDispatcher instance)
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

            ManualResetEvent mre = new ManualResetEvent(false);

            instance.Post(new Action(() =>
            {
                instance.BeginShutdown(ThreadDispatcherShutdownMode.FinishPending);
                mre.Set();
            }));

            mre.WaitOne();
            instance.WaitForShutdown();
            await instance.WaitForShutdownAsync();

            // Assert
            Assert.True(count == 5);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }
    }
}
