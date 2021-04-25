using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Abstractions.Dispatcher
{
    /// <summary>
    ///     A <see cref="TaskScheduler" /> which uses a <see cref="IThreadDispatcher" /> for execution.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class ThreadDispatcherTaskScheduler : TaskScheduler
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadDispatcherTaskScheduler" />.
        /// </summary>
        /// <param name="dispatcher"> The used dispatcher. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public ThreadDispatcherTaskScheduler (IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            this.SyncRoot = new object();
            this.Dispatcher = dispatcher;

            this.Tasks = new Dictionary<Task, IThreadDispatcherOperation>();
            this.TaskExecutor = this.ExecuteTask;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used dispatcher.
        /// </summary>
        /// <value>
        ///     The used dispatcher.
        /// </value>
        public IThreadDispatcher Dispatcher { get; }

        /// <summary>
        ///     Gets the object which can be used for thread synchronization when accessing <see cref="IThreadDispatcherOperation"/>.
        /// </summary>
        /// <value>
        ///     The object which can be used for thread synchronization.
        /// </value>
        /// <remarks>
        /// <note type="implement">
        /// This property must never be null.
        /// </note>
        /// </remarks>
        public object SyncRoot { get; }

        private Func<Task, bool> TaskExecutor { get; }

        private Dictionary<Task, IThreadDispatcherOperation> Tasks { get; }

        #endregion




        #region Instance Methods

        private bool ExecuteTask (Task task)
        {
            bool execute;
            lock (this.SyncRoot)
            {
                execute = this.Tasks.Remove(task);
            }

            return execute ? this.TryExecuteTask(task) : false;
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override int MaximumConcurrencyLevel => 1;

        /// <inheritdoc />
        protected override IEnumerable<Task> GetScheduledTasks ()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(this.SyncRoot, ref lockTaken);
                if (lockTaken)
                {
                    return this.Tasks.Keys.ToArray();
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(this.SyncRoot);
                }
            }
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        protected override void QueueTask (Task task)
        {
            lock (this.SyncRoot)
            {
                if (this.Tasks.ContainsKey(task))
                {
                    return;
                }

                IThreadDispatcherOperation operation = this.Dispatcher.Post(null, this.Dispatcher.GetCurrentPriorityOrDefault(this.Dispatcher.DefaultPriority).Value, this.Dispatcher.GetCurrentOptionsOrDefault(this.Dispatcher.DefaultOptions).Value, this.TaskExecutor, task);
                this.Tasks.Add(task, operation);
            }
        }

        /// <inheritdoc />
        protected override bool TryDequeue (Task task)
        {
            lock (this.SyncRoot)
            {
                if (!this.Tasks.ContainsKey(task))
                {
                    return false;
                }

                IThreadDispatcherOperation operation = this.Tasks[task];
                this.Tasks.Remove(task);
                return operation.Cancel();
            }
        }

        /// <inheritdoc />
        protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued)
        {
            lock (this.SyncRoot)
            {
                if (!this.Dispatcher.IsInThread())
                {
                    return false;
                }

                if (taskWasPreviouslyQueued)
                {
                    if (this.TryDequeue(task))
                    {
                        return this.TryExecuteTask(task);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return this.TryExecuteTask(task);
                }
            }
        }

        #endregion
    }
}
