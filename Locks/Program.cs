using System;
using System.Collections.Generic;
using System.Threading;

namespace Locks
{
    public class Program
    {
        private static volatile int _i;

        public static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                _i = 0;

                var numThreads = 10;
                var l = new FischerLock(10);
                var ts = new List<Thread>();
                for (int j = 0; j < numThreads; j++)
                {
                    ts.Add(new Thread(() => Inc(j, l)));
                }

                foreach (var t in ts)
                {
                    t.Start();
                }

                foreach (var t in ts)
                {
                    t.Join();
                }

                Console.WriteLine(_i);
            }

            Console.Read();
        }

        private static void Inc(int pid, ILock l)
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    l.Request(pid);
                    for (int j = 0; j < 1000; j++)
                    {
                        _i++;
                    }
                    l.Release(pid);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
