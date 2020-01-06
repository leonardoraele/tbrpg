using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Raele.Util
{
    public class SynchronizedCoroutineQueue
    {
        private Queue<Func<YieldInstruction>> Queue = new Queue<Func<YieldInstruction>>();

        public bool IsEmpty => this.Queue.IsEmpty();

        public SynchronizedCoroutineQueue()
        {
            UnityEventListener.StartCoroutine(this.StartConsuming);
        }

        /// <summary>
        /// TODO !CRITICAL! Make sure this object is garbage-collectible even with the coroutine running; and that the
        /// coroutine stops once this object is no longer needed. (an unability to collect this object and stop the
        /// coroutine would be a memory leak)
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartConsuming()
        {
            while (this.Queue != null) // supposedly never ends // TODO Is this the best approach?
            {
                yield return this.Queue.IsEmpty()
                    ? null
                    : this.Queue.Dequeue()
                        .Then(R.Invoke);
            }
        }

        public SynchronizedCoroutineQueue Enqueue(Action action)
        {
            this.Queue.Enqueue(() =>
            {
                action();
                return null;
            });
            return this;
        }

        public SynchronizedCoroutineQueue Enqueue(Func<YieldInstruction> instructionFunction)
        {
            this.Queue.Enqueue(instructionFunction.AssertNotDefault());
            return this;
        }

        public SynchronizedCoroutineQueue Enqueue(Func<IEnumerator> coroutineFunction)
        {
            coroutineFunction.AssertNotDefault();
            this.Queue.Enqueue(() => UnityEventListener.StartCoroutine(coroutineFunction));
            return this;
        }
    }
}
