using System.Threading;

namespace Locks
{
    /// <summary>
    /// Standard CAS lock
    /// </summary>
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
}
