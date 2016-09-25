using System.Threading;

namespace Locks
{
    /// <summary>
    /// Mellor-Crummey and Scott lock
    /// Improves on the ClhQueueLock by removing the cross core
    /// spinning which causes massive cache invalidation
    /// </summary>
    public class McsQueueLock : ILock
    {
        private class Node
        {
            public bool IsLocked { get; set; } = true;
            public Node Next { get; set; }
        }

        private Node _tail;
        private ThreadLocal<Node> _curr;

        public McsQueueLock()
        {
            _curr = new ThreadLocal<Node>(() => new Node());
        }

        public void Request(int pid)
        {
            var prev = Interlocked.Exchange(ref _tail, _curr.Value);
            Thread.MemoryBarrier();
            if (prev != null)
            {
                _curr.Value.IsLocked = true;
                Thread.MemoryBarrier();
                prev.Next = _curr.Value;
                Thread.MemoryBarrier();
                while (_curr.Value.IsLocked)
                {
                    Thread.Sleep(0);
                }
            }
        }

        public void Release(int pid)
        {
            if (_curr.Value.Next == null)
            {
                if (Interlocked.CompareExchange(ref _tail, null, _curr.Value) == _curr.Value)
                {
                    return;
                }
                Thread.MemoryBarrier();
                while (_curr.Value.Next == null)
                {
                    Thread.Sleep(0);
                }
            }

            Thread.MemoryBarrier();
            _curr.Value.Next.IsLocked = false;
            _curr.Value.Next = null;
        }
    }
}
