using System.Threading;

namespace Locks
{
    /// <summary>
    /// Like the naive lock, but uses CAS instructions to provide atomicity, and correct behavior
    /// </summary>
    public class CASLock : ILock
    {
        private int _lock;

        public void Request(int pid)
        {
            while (Interlocked.CompareExchange(ref _lock, 1, 0) != 0) ;
        }

        public void Release(int pid)
        {
            _lock = 0;
        }
    }
}
