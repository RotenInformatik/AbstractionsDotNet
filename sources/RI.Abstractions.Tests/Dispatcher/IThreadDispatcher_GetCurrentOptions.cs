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
    public sealed class IThreadDispatcher_GetCurrentOptions
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task GetCurrentOptions_Running_DifferentOptions(IThreadDispatcher instance)
        {
            // Arrange

            DispatcherThread thread = new DispatcherThread(instance, new CultureInfo("de-ch"));
            await thread.StartAsync();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            ThreadDispatcherOptions options1 = ThreadDispatcherOptions.None;
            ThreadDispatcherOptions options2 = ThreadDispatcherOptions.None;
            ThreadDispatcherOptions options3 = ThreadDispatcherOptions.None;

            // Act

            IThreadDispatcherOperation op1 = instance.Post(ThreadDispatcherOptions.CaptureCulture, new Action(() =>
            {
                options1 = instance.GetCurrentOptions()
                                   .Value;
            }));

            IThreadDispatcherOperation op2 = instance.Post(ThreadDispatcherOptions.CaptureAll, new Action(() =>
            {
                options2 = instance.GetCurrentOptions()
                                   .Value;
            }));

            IThreadDispatcherOperation op3 = instance.Post(new Action(() =>
            {
                options3 = instance.GetCurrentOptions()
                                   .Value;
            }));

            await op1.WaitAsync();
            await op2.WaitAsync();
            await op3.WaitAsync();

            // Assert

            Assert.Equal(ThreadDispatcherOptions.CaptureCulture, options1);
            Assert.Equal(ThreadDispatcherOptions.CaptureAll, options2);
            Assert.Equal(ThreadDispatcherOptions.None, options3);

            // Cleanup

            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task GetCurrentOptions_NotRunning_NoOptions(IThreadDispatcher instance)
        {
            // Act + Assert

            Assert.Null(instance.GetCurrentOptions());
            Assert.Equal(ThreadDispatcherOptions.None, instance.GetCurrentOptionsOrDefault(ThreadDispatcherOptions.None));
        }
    }
}
