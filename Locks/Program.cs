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

                var numThreads = 3;
                var l = new AndersonLock(numThreads);
                var ts = new List<Thread>();
                for (int j = 0; j < numThreads; j++)
                {
                    int k = j; // Capture j or all thread ids will be the same
                    ts.Add(new Thread(() => Inc(k, l)));
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
                    for (int j = 0; j < 1000; j++)
                    {
                        l.Request(pid);
                        _i++;
                        l.Release(pid);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
