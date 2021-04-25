using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_PostDelayed
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task PostDelayed_OneShotOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            ManualResetEvent mre = new ManualResetEvent(false);

            IThreadDispatcherTimer timer = instance.PostDelayed(ThreadDispatcherTimerMode.OneShot, 100, new Action(() =>
            {
                mre.Set();
            }));

            timer.Start();
            mre.WaitOne();

            // Assert

            Assert.Equal(1, timer.ExecutionCount);
            Assert.False(timer.IsRunning);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task PostDelayed_OneShotSameThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            ManualResetEvent mre = new ManualResetEvent(false);
            IThreadDispatcherTimer timer2 = null;
            IThreadDispatcherTimer timer1 = instance.PostDelayed(ThreadDispatcherTimerMode.OneShot, 100, new Action(() =>
            {
                timer2 = instance.PostDelayed(ThreadDispatcherTimerMode.OneShot, 100, new Action(() =>
                {
                    mre.Set();
                }));

                timer2.Start();
            }));

            timer1.Start();
            mre.WaitOne();

            // Assert

            Assert.Equal(1, timer1.ExecutionCount);
            Assert.False(timer1.IsRunning);
            Assert.Equal(1, timer2.ExecutionCount);
            Assert.False(timer2.IsRunning);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task PostDelayed_ContinuousOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            ManualResetEvent mre = new ManualResetEvent(false);
            IThreadDispatcherTimer timer = null;
            timer = instance.PostDelayed(ThreadDispatcherTimerMode.Continuous, 100, new Action(() =>
            {
                if (timer.ExecutionCount == 3)
                {
                    mre.Set();
                    timer.Stop();
                }
            }));

            timer.Start();
            mre.WaitOne();

            // Assert

            Assert.Equal(3, timer.ExecutionCount);
            Assert.False(timer.IsRunning);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task PostDelayed_ContinuousShotSameThread_Success(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act

            ManualResetEvent mre = new ManualResetEvent(false);
            IThreadDispatcherTimer timer2 = null;

            IThreadDispatcherTimer timer1 = instance.PostDelayed(ThreadDispatcherTimerMode.OneShot, 100, new Action(() =>
            {
                timer2 = instance.PostDelayed(ThreadDispatcherTimerMode.Continuous, 100, new Action(() =>
                {
                    if (timer2.ExecutionCount == 3)
                    {
                        mre.Set();
                        timer2.Stop();
                    }
                }));

                timer2.Start();
            }));

            timer1.Start();
            mre.WaitOne();

            // Assert

            Assert.Equal(1, timer1.ExecutionCount);
            Assert.False(timer1.IsRunning);
            Assert.Equal(3, timer2.ExecutionCount);
            Assert.False(timer2.IsRunning);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }
    }
}
