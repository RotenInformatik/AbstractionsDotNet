﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_DoProcessingAsync
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task DoProcessingAsync_SpecificSameThread_InvalidOperationException(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act + Assert

            for (int i1 = 0; i1 < 5; i1++)
            {
                instance.Post(i1, new Action(() =>
                {
                    Thread.Sleep(100);
                }));
            }

            instance.Post(5, new Func<Task>(async () =>
            {
                Assert.ThrowsAsync<InvalidOperationException>(async () => await instance.DoProcessingAsync(2));
            }));

            instance.DoProcessing();

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task DoProcessingAsync_SpecificOtherThread_Success(IThreadDispatcher instance)
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
                    Thread.Sleep(500);
                    count++;
                }));
            }

            await instance.DoProcessingAsync(2);

            // Assert

            Assert.Equal(3, count);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task DoProcessingAsync_AllSameThread_InvalidOperationException(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act + Assert

            for (int i1 = 0; i1 < 5; i1++)
            {
                instance.Post(i1, new Action(() =>
                {
                    Thread.Sleep(100);
                }));
            }

            instance.Post(5, new Func<Task>(async () =>
            {
                Assert.ThrowsAsync<InvalidOperationException>(async () => await instance.DoProcessingAsync());
            }));

            instance.DoProcessing();


            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.FinishPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task DoProcessingAsync_AllOtherThread_Success(IThreadDispatcher instance)
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

            await instance.DoProcessingAsync();

            // Assert

            Assert.Equal(5, count);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.FinishPending);
        }
    }
}
