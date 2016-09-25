using System.Threading;

namespace Locks
{
    /// <summary>
    /// Craig, Landin, and Hagersten lock
    /// Requests form an implicit queue.
    /// All the thrashing from cache invalidation causes this
    /// to run really slow (spinning on _prev.Value is the culprit)
    /// </summary>
    public class ClhQueueLock : ILock
    {
        private class Node
        {
            public bool IsLocked { get; set; }
        }

        private Node _tail;
        private ThreadLocal<Node> _curr;
        private ThreadLocal<Node> _prev;

        public ClhQueueLock()
        {
            _tail = new Node();
            _curr = new ThreadLocal<Node>(() => new Node());
            _prev = new ThreadLocal<Node>(() => new Node());
        }

        public void Request(int pid)
        {
            _curr.Value.IsLocked = true;
            Thread.MemoryBarrier();
            _prev.Value = Interlocked.Exchange(ref _tail, _curr.Value);
            Thread.MemoryBarrier();
            while (_prev.Value.IsLocked)
            {
                Thread.Sleep(0);
            }
        }

        public void Release(int pid)
        {
            _curr.Value.IsLocked = false;
            Thread.MemoryBarrier();
            _curr.Value = _prev.Value;
        }
    }
}
