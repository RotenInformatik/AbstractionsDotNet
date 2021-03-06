﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using RI.Abstractions.Dispatcher;

using Xunit;




namespace RI.Abstractions.Tests.Utilities
{
    public sealed class DispatcherThread
    {
        public DispatcherThread (IThreadDispatcher dispatcher, CultureInfo culture = null)
        {
            this.Dispatcher = dispatcher;
            this.Culture = culture ?? CultureInfo.InvariantCulture;

            this.Exception = null;
            this.StartCommitted = new TaskCompletionSource<object>();
            this.StopCommitted = new TaskCompletionSource<object>();

            this.Thread = new Thread(Run);
            this.Thread.CurrentCulture = this.Culture;
            this.Thread.CurrentUICulture = this.Culture;
        }

        private void Run ()
        {
            this.Dispatcher.Post(int.MaxValue, new Action(() => this.StartCommitted.SetResult(null)));
            this.Dispatcher.Run();
            this.StopCommitted.SetResult(null);
        }

        public CultureInfo Culture { get; }

        private TaskCompletionSource<object> StartCommitted { get; }

        private TaskCompletionSource<object> StopCommitted { get; }

        private Thread Thread { get; }

        private Exception Exception { get; set; }

        public IThreadDispatcher Dispatcher { get; }

        public async Task StartAsync ()
        {
            this.Dispatcher.Exception += (sender, args) =>
            {
                if (this.Exception == null)
                {
                    this.Exception = args.Exception;
                }
            };
            this.Thread.Start();
            await this.StartCommitted.Task;
        }

        public async Task StopAsync (ThreadDispatcherShutdownMode shutdownMode)
        {
            Assert.Null(this.Exception);

            lock (this.Dispatcher.SyncRoot)
            {
                if ((!this.Dispatcher.IsShuttingDown) && this.Dispatcher.IsRunning)
                {
                    this.Dispatcher.BeginShutdown(shutdownMode);
                }
            }

            await this.StopCommitted.Task;
        }
    }
}
