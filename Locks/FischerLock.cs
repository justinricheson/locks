using System.Threading;

namespace Locks
{
    /// <summary>
    /// Time-based lock, currently not working
    /// </summary>
    public class FischerLock : ILock
    {
        private int _numProcesses;
        private volatile int _turn = -1;

        public FischerLock(int numProcesses)
        {
            _numProcesses = numProcesses;
        }

        public void Request(int pid)
        {
            while (true)
            {
                while (_turn != -1) ;
                Thread.MemoryBarrier();
                _turn = pid;
                Thread.MemoryBarrier();
                Thread.Sleep(5);
                Thread.MemoryBarrier();
                if (_turn == pid)
                {
                    return;
                }
            }
        }

        public void Release(int pid)
        {
            _turn = -1;
        }
    }
}
