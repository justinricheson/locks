namespace Locks
{
    public interface ILock
    {
        void Request(int pid);
        void Release(int pid);
    }
}
