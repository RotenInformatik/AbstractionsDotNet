using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_Idle
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Idle_Raise_CorrectAmountAndOrder(IThreadDispatcher instance)
        {
            // Arrange
            List<int> calls = new List<int>();
            int idleCalls = 0;

            instance.Idle += (sender, args) =>
            {
                idleCalls++;
                calls.Add(idleCalls);
            };

            instance.Post(new Action(() => calls.Add(10)));
            instance.Post(new Action(async () => await Task.Delay(100)));

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act + Assert

            Thread.Sleep(2000);
            calls.Add(200);

            instance.Post(new Action(() => calls.Add(30)));
            instance.Post(new Action(() => instance.BeginShutdown(ThreadDispatcherShutdownMode.AllowNew)));
            instance.Post(new Action(() => calls.Add(40)));

            Thread.Sleep(2000);
            calls.Add(300);

            await thread.StopAsync(ThreadDispatcherShutdownMode.AllowNew);
            calls.Add(400);

            Assert.Equal(9, calls.Count);
            Assert.Equal(10, calls[0]);
            Assert.Equal(1, calls[1]);
            Assert.Equal(2, calls[2]);
            Assert.Equal(200, calls[3]);
            Assert.Equal(30, calls[4]);
            Assert.Equal(40, calls[5]);
            Assert.Equal(3, calls[6]);
            Assert.Equal(300, calls[7]);
            Assert.Equal(400, calls[8]);
        }
    }
}
