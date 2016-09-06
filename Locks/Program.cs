using System;
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

            Console.Read();
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
}
