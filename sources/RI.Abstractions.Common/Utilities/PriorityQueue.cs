using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;




namespace RI.Abstractions.Utilities
{
    internal sealed class PriorityQueue <T> : ICollection, IEnumerable<T>, IEnumerable
    {
        #region Instance Constructor/Destructor

        public PriorityQueue ()
        {
            this.SyncRoot = new object();
            this.Chain = new LinkedList<PriorityItem>();
            this.Table = new Hashtable();
        }

        #endregion




        #region Instance Properties/Indexer

        private LinkedList<PriorityItem> Chain { get; }

        private object SyncRoot { get; }

        private Hashtable Table { get; }

        #endregion




        #region Instance Methods

        public void CopyTo (T[] array, int index)
        {
            foreach (T item in this)
            {
                array[index] = item;
                index++;
            }
        }

        private T Get (bool remove, out int priority)
        {
            if (this.Chain.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            PriorityItem priorityItem = this.Chain.Last?.Value;

            if (priorityItem == null)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            priority = priorityItem.Priority;
            T item = remove ? priorityItem.Dequeue() : priorityItem.Peek();

            if (priorityItem.Count == 0)
            {
                this.Chain.RemoveLast();
                this.Table.Remove(priority);
            }

            return item;
        }

        #endregion




        #region Interface: IPriorityQueue<T>

        public int Count
        {
            get
            {
                int count = 0;

                foreach (PriorityItem chainItem in this.Chain)
                {
                    count += chainItem.Count;
                }

                return count;
            }
        }

        public int HighestPriority
        {
            get
            {
                if (this.Chain.Count == 0)
                {
                    return -1;
                }

                return (this.Chain.Last?.Value.Priority).GetValueOrDefault(-1);
            }
        }

        bool ICollection.IsSynchronized => false;

        public int LowestPriority
        {
            get
            {
                if (this.Chain.Count == 0)
                {
                    return -1;
                }

                return (this.Chain.First?.Value.Priority).GetValueOrDefault(-1);
            }
        }

        object ICollection.SyncRoot => this.SyncRoot;

        public void Clear ()
        {
            this.Chain.Clear();
            this.Table.Clear();
        }

        void ICollection.CopyTo (Array array, int index)
        {
            int i1 = 0;

            foreach (T item in this)
            {
                array.SetValue(item, index + i1);
                i1++;
            }
        }


        public void CopyTo (PriorityQueue<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (T item in this)
            {
                other.Enqueue(item);
            }
        }

        public T Dequeue ()
        {
            return this.Get(true, out int _);
        }

        public T Dequeue (out int priority)
        {
            return this.Get(true, out priority);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException"),]
        public void Enqueue (T item)
        {
            int lowestPriority = this.LowestPriority;
            this.Enqueue(item, lowestPriority == -1 ? 0 : lowestPriority);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException"),]
        public void Enqueue (T item, int priority)
        {
            if (priority < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            int firstPriority = 0;
            int lastPriority = 0;

            if (this.Table.Count > 0)
            {
                firstPriority = this.Chain.First.Value.Priority;
                lastPriority = this.Chain.Last.Value.Priority;
            }

            PriorityItem priorityItem;

            if (this.Table.Count == 0)
            {
                priorityItem = new PriorityItem(priority);
                this.Table.Add(priority, priorityItem);
                this.Chain.AddFirst(priorityItem);
            }
            else if (this.Table.ContainsKey(priority))
            {
                priorityItem = (PriorityItem)this.Table[priority];
            }
            else if (priority < firstPriority)
            {
                priorityItem = new PriorityItem(priority);
                this.Table.Add(priority, priorityItem);
                this.Chain.AddFirst(priorityItem);
            }
            else if (priority > lastPriority)
            {
                priorityItem = new PriorityItem(priority);
                this.Table.Add(priority, priorityItem);
                this.Chain.AddLast(priorityItem);
            }
            else if ((priority - firstPriority) < (lastPriority - priority))
            {
                priorityItem = new PriorityItem(priority);
                this.Table.Add(priority, priorityItem);
                LinkedListNode<PriorityItem> node = this.Chain.First;

                while (node.Value.Priority < priority)
                {
                    node = node.Next;
                }

                this.Chain.AddBefore(node, priorityItem);
            }
            else
            {
                priorityItem = new PriorityItem(priority);
                this.Table.Add(priority, priorityItem);
                LinkedListNode<PriorityItem> node = this.Chain.Last;

                while (node.Value.Priority > priority)
                {
                    node = node.Previous;
                }

                this.Chain.AddAfter(node, priorityItem);
            }

            priorityItem.Enqueue(item);
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator ()
        {
            LinkedListNode<PriorityItem> node = this.Chain.Last;

            while (node != null)
            {
                foreach (T item in node.Value)
                {
                    yield return item;
                }

                node = node.Previous;
            }
        }

        public int MoveTo (PriorityQueue<T> queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            int count = 0;

            while (this.Count > 0)
            {
                T item = this.Dequeue(out int priority);
                queue.Enqueue(item, priority);
                count++;
            }

            return count;
        }

        public T Peek ()
        {
            return this.Get(false, out int _);
        }

        public T Peek (out int priority)
        {
            return this.Get(false, out priority);
        }

        #endregion




        #region Type: PriorityItem

        private sealed class PriorityItem : Queue<T>
        {
            #region Instance Constructor/Destructor

            public PriorityItem (int priority)
            {
                if (priority < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(priority));
                }

                this.Priority = priority;
            }

            #endregion




            #region Instance Properties/Indexer

            public int Priority { get; }

            #endregion
        }

        #endregion
    }
}
