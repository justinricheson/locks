using System.Threading;

namespace Locks
{
    /// <summary>
    /// Lamport fast mutex, based on the splitter algorithm.
    /// Provides fast and slow paths for improved speed with low contention.
    /// </summary>
    public class LamportFast : ILock
    {
        private int _numProcesses;
        private volatile int _x = -1;
        private volatile int _y = -1;
        private volatile bool[] _flags;

        public LamportFast(int numProcesses)
        {
            _numProcesses = numProcesses;
            _flags = new bool[numProcesses];
        }

        public void Request(int pid)
        {
            while (true)
            {
                _flags[pid] = true;
                _x = pid;
                Thread.MemoryBarrier();
                if (_y != -1)
                {
                    _flags[pid] = false;
                }
                else
                {
                    _y = pid;
                    Thread.MemoryBarrier();
                    if (_x == pid)
                    {
                        return; // Fast path
                    }
                    else
                    {
                        _flags[pid] = false;
                        for (int i = 0; i < _numProcesses; i++)
                        {
                            while (_flags[i]) ;
                        }
                        if (_y == pid)
                        {
                            return; // Slow path
                        }
                    }
                }
                
                while (_y != -1) ;
            }
        }

        public void Release(int pid)
        {
            _y = -1;
            _flags[pid] = false;
        }
    }
}
