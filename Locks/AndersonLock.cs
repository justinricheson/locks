using System;
using System.Threading;

namespace Locks
{
    /// <summary>
    /// Uses CAS instructions to maintain a circular buffer used to assign requests round-robin.
    /// Note: Each thread spin-waits on its own variable. As long as the variables are sufficiently large
    /// to prevent false sharing, this allows efficient use of the cache system for threads awaiting a request
    /// </summary>
    public class AndersonLock : ILock
    {
        private int _numProcesses;
        private int _next;
        private int[] _slots;
        private bool[] _available;

        public AndersonLock(int numProcesses)
        {
            _numProcesses = numProcesses;
            _slots = new int[numProcesses];
            _available = new bool[numProcesses];
            _available[1] = true;
        }

        public void Request(int pid)
        {
            _slots[pid] = Interlocked.Increment(ref _next) % _numProcesses;
            Thread.MemoryBarrier();
            while (!_available[_slots[pid]]) ;
        }

        public void Release(int pid)
        {
            _available[_slots[pid]] = false;
            Thread.MemoryBarrier();
            _available[(_slots[pid] + 1) % _numProcesses] = true;
        }
    }
}
