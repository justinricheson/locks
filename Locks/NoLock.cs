namespace Locks
{
    /// <summary>
    /// Dummy lock for testing unlocked behavior
    /// </summary>
    public class NoLock : ILock
    {
        public void Request(int pid) { }
        public void Release(int pid) { }
    }
}
