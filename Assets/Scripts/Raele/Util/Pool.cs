using System;
using System.Collections.Generic;

namespace Raele.Util
{
    public class Pool<R>
    {
        public Queue<R> AvailableItems { get; private set; }
        public Dictionary<object, List<R>> AllocatedItems { get; private set; }
        public Func<R> Creator { get; set; }
        public Action<R> OnReturned { get; set; }
        public Action<R> OnPicked { get; set; }

        /**
         * The Pool will use the creator callback to create new items when it runs out of items.
         * onReturned will be called on each item that is returned to the Pool, so that it is teared down.
         * onPicked will be called on all existing items in the pool before they are picked, so that they
         * are set up for use again. onPicked is not called on items just created by the creator callback.
         */
        public Pool(Func<R> creator, Action<R> onPicked = null, Action<R> onReturned = null, int initialSize = 0)
        {
            this.Creator = creator;
            this.AvailableItems = new Queue<R>();
            this.AllocatedItems = new Dictionary<object, List<R>>();
            this.OnReturned = onReturned;
            this.OnPicked = onPicked;
            this.Preload(initialSize);
        }

        /**
         * Ensures there are at least n available items on the pool. Since the items start in the pool, not in the world,
         * onReturned will be called for each of them.
         */
        public void Preload(int n)
        {
            while (this.AvailableItems.Count < n)
            {
                this.Return(this.Creator());
            }
        }

        /**
         * Removes and returns an item from the pull. After the item is no longer needed, Return should be called so that
         * the item is put back in the pull.
         */
        public R Pick()
        {
            R item = default(R);

            if (this.AvailableItems.Count > 0)
            {
                item = this.AvailableItems.Dequeue();
            }
            else
            {
                item = this.Creator();
            }

            if (this.OnPicked != null)
            {
                this.OnPicked(item);
            }

            return item;
        }

        /**
         * When you pick an item from the pool, you can attribute that item to an "key" so that the Pool keeps that of the items you are using for you.
         * Then, when you want to return the items, you can call Release passing the same "key" as you did when you picked the item, and the Pool will
         * make all the items associated to that "key" available to be picked up again.
         */
        public R Pick(object key)
        {
            R item = this.Pick();

            if (!this.AllocatedItems.ContainsKey(key))
            {
                this.AllocatedItems[key] = new List<R>();
            }

            this.AllocatedItems[key].Add(item);

            return item;
        }

        public void Release(object key)
        {
            this.AllocatedItems[key].ForEach(item => this.Return(item));
            this.AllocatedItems[key].Clear();
        }

        /**
         * Put an item in the pull. OnReturn will be called for the item.
         */
        public void Return(R item)
        {
            if (this.OnReturned != null)
            {
                this.OnReturned(item);
            }
            this.AvailableItems.Enqueue(item);
        }

        /**
         * Permanently removes all objects from the pool. New picks will create entirely new objects using the Creator callback function.
         */
        public void Clear()
        {
            this.AvailableItems.Clear();
            this.AllocatedItems.Clear();
        }
    }
}
