using System.Threading;

namespace Locks
{
    /// <summary>
    /// Peterson lock for N = 2
    /// </summary>
    public class PetersonLock : ILock
    {
        private volatile bool[] _wantsCs = new bool[2];
        private volatile int _turn;

        public void Request(int pid)
        {
            int j = pid == 1 ? 0 : 1;

            _wantsCs[pid] = true;
            Thread.MemoryBarrier(); // Prevent reordering
            _turn = j;

            while (_wantsCs[j] && _turn == j) ;
        }

        public void Release(int pid)
        {
            _wantsCs[pid] = false;
        }
    }
}
