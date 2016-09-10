namespace Locks
{
    /// <summary>
    /// Naive, non-functional lock
    /// </summary>
    public class NaiveLock : ILock
    {
        private volatile bool _locked;

        public void Request(int pid)
        {
            while (_locked) ;
            _locked = true;
        }

        public void Release(int pid)
        {
            _locked = false;
        }
    }
}
