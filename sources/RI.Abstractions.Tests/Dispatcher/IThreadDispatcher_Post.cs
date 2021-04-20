using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Composition;
using RI.Abstractions.Dispatcher;
using RI.Abstractions.Tests.Composition;
using RI.Abstractions.Tests.Fakes;
using RI.Abstractions.Tests.Utilities;

using Xunit;




namespace RI.Abstractions.Tests.Dispatcher
{
    public sealed class IThreadDispatcher_Post
    {
        public static IEnumerable<object[]> GetDispatchers() =>
            _DispatcherTestFactory.GetDispatchers();

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_SimpleOtherThread_Success(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act
            IThreadDispatcherOperation op = instance.Post(new Action(() => { }));
            await op.WaitAsync();

            // Assert
            Assert.Equal(ThreadDispatcherOperationState.Finished, op.State);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_SimpleSameThread_Success(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();

            // Act
            IThreadDispatcherOperation op2 = null;
            IThreadDispatcherOperation op1 = instance.Post(new Action(() =>
            {
                op2 = instance.Post(new Action(() => { }));
            }));
            await op1.WaitAsync();
            await op2.WaitAsync();
            
            // Assert
            Assert.Equal(ThreadDispatcherOperationState.Finished, op1.State);
            Assert.Equal(ThreadDispatcherOperationState.Finished, op2.State);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_WithParameters_Success(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();
            int param1 = 0;
            string param2 = null;

            // Act
            IThreadDispatcherOperation op = instance.Post(new Action<int, string>((a, b) =>
            {
                param1 = a;
                param2 = b;
            }), 123, "123");
            await op.WaitAsync();

            // Assert
            Assert.Equal(123, param1);
            Assert.True(string.Equals("123", param2, StringComparison.InvariantCultureIgnoreCase));

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_WithResult_Success(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();
            int ret = 0;

            // Act
            IThreadDispatcherOperation op = instance.Post(new Func<int>(() => 123));
            await op.WaitAsync();

            // Assert
            Assert.Equal(123, (int)op.Result);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_DifferentPriorities_CorrectOrder(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();
            List<int> output = new List<int>();

            // Act
            IThreadDispatcherOperation op1 = instance.Post(0, new Action(() => Thread.Sleep(100)));
            IThreadDispatcherOperation op2 = instance.Post(0, new Action(() => output.Add(0)));
            IThreadDispatcherOperation op3 = instance.Post(1, new Action(() => output.Add(1)));
            IThreadDispatcherOperation op4 = instance.Post(1, new Action(() => output.Add(2)));
            IThreadDispatcherOperation op5 = instance.Post(123456789, new Action(() => output.Add(3)));

            await op1.WaitAsync();
            await op2.WaitAsync();
            await op3.WaitAsync();
            await op4.WaitAsync();
            await op5.WaitAsync();

            // Assert
            Assert.Equal(3, output[0]);
            Assert.Equal(1, output[1]);
            Assert.Equal(2, output[2]);
            Assert.Equal(0, output[3]);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_CultureCapture_SuccessfulCapture(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance, new CultureInfo("de-ch"));
            await thread.StartAsync();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            CultureInfo output1 = null;
            CultureInfo output2 = null;

            // Act

            IThreadDispatcherOperation op1 = instance.Post(ThreadDispatcherOptions.CaptureCulture, new Action(() =>
            {
                output1 = Thread.CurrentThread.CurrentCulture;
            }));

            IThreadDispatcherOperation op2 = instance.Post(ThreadDispatcherOptions.None, new Action(() =>
            {
                output2 = Thread.CurrentThread.CurrentCulture;
            }));

            await op1.WaitAsync();
            await op2.WaitAsync();

            // Assert
            Assert.True(string.Equals("en-us", output1.Name, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(string.Equals("de-ch", output2.Name, StringComparison.InvariantCultureIgnoreCase));

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_CaptureSynchronizationContext_SuccessfulCapture(IThreadDispatcher instance)
        {
            // Arrange
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();
            SynchronizationContext output1 = null;
            SynchronizationContext output2 = null;
            SynchronizationContext original = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new FakeSynchronizationContext());

            // Act

            IThreadDispatcherOperation op1 = instance.Post(ThreadDispatcherOptions.CaptureSynchronizationContext, new Action(() =>
            {
                output1 = SynchronizationContext.Current;
            }));

            IThreadDispatcherOperation op2 = instance.Post(ThreadDispatcherOptions.None, new Action(() =>
            {
                output2 = SynchronizationContext.Current;
            }));

            await op1.WaitAsync();
            await op2.WaitAsync();

            // Assert
            Assert.Equal(typeof(FakeSynchronizationContext), output1.GetType());
            Assert.Equal(typeof(ThreadDispatcherSynchronizationContext), output2.GetType());

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
            SynchronizationContext.SetSynchronizationContext(original);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_DefaultPriorities_CorrectOrder(IThreadDispatcher instance)
        {
            // Arrange
            instance.DefaultPriority = 100;
            DispatcherThread thread = new DispatcherThread(instance);
            await thread.StartAsync();
            List<int> output = new List<int>();

            // Act
            IThreadDispatcherOperation op1 = instance.Post(0, new Action(() => Thread.Sleep(100)));
            IThreadDispatcherOperation op2 = instance.Post(new Action(() => output.Add(0)));
            IThreadDispatcherOperation op3 = instance.Post(new Action(() => output.Add(1)));
            IThreadDispatcherOperation op4 = instance.Post(1, new Action(() => output.Add(2)));
            IThreadDispatcherOperation op5 = instance.Post(123456789, new Action(() => output.Add(3)));

            await op1.WaitAsync();
            await op2.WaitAsync();
            await op3.WaitAsync();
            await op4.WaitAsync();
            await op5.WaitAsync();

            // Assert
            Assert.Equal(3, output[0]);
            Assert.Equal(0, output[1]);
            Assert.Equal(1, output[2]);
            Assert.Equal(2, output[3]);

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }

        [Theory]
        [MemberData(nameof(IThreadDispatcher_Post.GetDispatchers))]
        public async Task Post_DefaultOptions_SuccessfulCapture(IThreadDispatcher instance)
        {
            // Arrange
            instance.DefaultOptions = ThreadDispatcherOptions.CaptureCulture;
            DispatcherThread thread = new DispatcherThread(instance, new CultureInfo("de-ch"));
            await thread.StartAsync();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            CultureInfo output1 = null;
            CultureInfo output2 = null;
            CultureInfo output3 = null;
            CultureInfo output4 = null;

            // Act

            IThreadDispatcherOperation op1 = instance.Post(ThreadDispatcherOptions.CaptureCulture, new Action(() =>
            {
                output1 = Thread.CurrentThread.CurrentCulture;
            }));

            IThreadDispatcherOperation op2 = instance.Post(ThreadDispatcherOptions.Default, new Action(() =>
            {
                output2 = Thread.CurrentThread.CurrentCulture;
            }));

            IThreadDispatcherOperation op3 = instance.Post(ThreadDispatcherOptions.None, new Action(() =>
            {
                output3 = Thread.CurrentThread.CurrentCulture;
            }));

            IThreadDispatcherOperation op4 = instance.Post(new Action(() =>
            {
                output4 = Thread.CurrentThread.CurrentCulture;
            }));

            await op1.WaitAsync();
            await op2.WaitAsync();

            // Assert
            Assert.True(string.Equals("en-us", output1.Name, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(string.Equals("en-us", output2.Name, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(string.Equals("de-ch", output3.Name, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(string.Equals("en-us", output4.Name, StringComparison.InvariantCultureIgnoreCase));

            // Cleanup
            await thread.StopAsync(ThreadDispatcherShutdownMode.DiscardPending);
        }
    }
}
