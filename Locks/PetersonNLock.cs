using System.Threading;

namespace Locks
{
    /// <summary>
    /// Generalization of the Peterson lock for N > 2
    /// </summary>
    public class PetersonNLock : ILock
    {
        private int _numProcesses;
        private volatile int[] _gate;
        private volatile int[] _last;

        public PetersonNLock(int numProcesses)
        {
            _numProcesses = numProcesses;
            _gate = new int[_numProcesses];
            _last = new int[_numProcesses];
        }

        public void Request(int pid)
        {
            for (int k = 1; k < _numProcesses; k++)
            {
                _gate[pid] = k;
                Thread.MemoryBarrier(); // Prevent reordering
                _last[k] = pid;

                for (int j = 0; j < _numProcesses; j++)
                {
                    while (j != pid && _gate[j] >= k && _last[k] == pid) ;
                }
            }
        }

        public void Release(int pid)
        {
            _gate[pid] = 0;
        }
    }
}
