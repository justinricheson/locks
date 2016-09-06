using System;
using System.Threading;

namespace Locks
{
    public class Program
    {
        private static volatile int _i = 0;
        public static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                RunTest();
            }
            Console.Read();
        }

        private static void RunTest()
        {
            _i = 0;
            var l = new PetersonNLock(3);
            var t1 = new Thread(() => Inc(0, l));
            var t2 = new Thread(() => Inc(1, l));
            var t3 = new Thread(() => Inc(2, l));

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();
            Console.WriteLine(_i);
        }

        private static void Inc(int pid, ILock l)
        {
            try
            {
                for (int i = 0; i < 1000000; i++)
                {
                    l.Request(pid);
                    _i++;
                    l.Release(pid);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public interface ILock
    {
        void Request(int pid);
        void Release(int pid);
    }

    public class NoLock : ILock
    {
        public void Request(int pid) { }
        public void Release(int pid) { }
    }

    public class StandardLock : ILock
    {
        private object _sync = new object();

        public void Request(int pid)
        {
            Monitor.Enter(_sync);
        }

        public void Release(int pid)
        {
            Monitor.Exit(_sync);
        }
    }

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

    public class BakeryLock : ILock
    {
        private int _numProcesses;
        private bool[] _choosing;
        private int[] _number;

        public BakeryLock(int numProcesses)
        {
            _numProcesses = numProcesses;
            _choosing = new bool[_numProcesses];
            _number = new int[_numProcesses];
        }

        public void Request(int pid)
        {
            _choosing[pid] = true;
            for (int j = 0; j < _numProcesses; j++)
            {
                var jNum = _number[j];
                if (jNum > _number[pid])
                {
                    _number[pid] = jNum;
                }
            }
            _number[pid]++;
            _choosing[pid] = false;

            for (int j = 0; j < _numProcesses; j++)
            {
                while (_choosing[j]) ;

                var jNum = _number[j];
                while (jNum != 0 && (
                    jNum < _number[pid]
                || (jNum == _number[pid] && j < pid))) ;
            }
        }

        public void Release(int pid)
        {
            _number[pid] = 0;
        }
    }
}
