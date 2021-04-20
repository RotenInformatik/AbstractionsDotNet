using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_GetCurrentPriority
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task GetCurrentPriority_Running_DifferentPriorities(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance, new CultureInfo("de-ch"));
            await thread.StartAsync();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            int priority1 = -1;
            int priority2 = -1;
            int priority3 = -1;

            // Act

            IThreadDispatcherOperation op1 = instance.Post(1, new Action(() =>
            {
                priority1 = instance.GetCurrentPriority()
                                    .Value;
            }));

            IThreadDispatcherOperation op2 = instance.Post(2, new Action(() =>
            {
                priority2 = instance.GetCurrentPriority()
                                    .Value;
            }));

            IThreadDispatcherOperation op3 = instance.Post(new Action(() =>
            {
                priority3 = instance.GetCurrentPriority()
                                    .Value;
            }));

            await op1.WaitAsync();
            await op2.WaitAsync();
            await op3.WaitAsync();

            // Assert

            Assert.Equal(1, priority1);
            Assert.Equal(2, priority2);
            Assert.Equal(0, priority3);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task GetCurrentPriority_NotRunning_NoPriority(IThreadDispatcher instance)
        {
            // Act + Assert

            Assert.Null(instance.GetCurrentPriority());
            Assert.Equal(100, instance.GetCurrentPriorityOrDefault(100));
        }
    }
}
