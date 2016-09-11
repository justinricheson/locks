using System.Threading;

namespace Locks
{
    /// <summary>
    /// Like the CasLock but with improved performance because of reduced interlocked calls
    /// </summary>
    public class FastCasLock : ILock
    {
        private int _lock;

        public void Request(int pid)
        {
            while (_lock != 0 || Interlocked.CompareExchange(ref _lock, 1, 0) != 0) ;
        }

        public void Release(int pid)
        {
            _lock = 0;
        }
    }
}
