namespace Locks
{
    /// <summary>
    /// Lamport Bakery Algorithm
    /// </summary>
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
