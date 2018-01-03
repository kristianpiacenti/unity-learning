/*TUTORIAL
    https://www.youtube.com/watch?v=_mxLLRQ1BQY 
*/

using System;
namespace Piacenti.Pooling
{
    public class QueuePool<T> : IPool<T> where T : class
    {
        private T[] objs;
        private int index = -1;
        private Func<T> produce;
        public QueuePool(Func<T> produce, int size = 0)
        {
            this.produce = produce;
            InitializePool(size);

        }

        private void InitializePool(int size)
        {
            objs = new T[size];
            while (index < size - 1)
            {
                index++;
                objs[index] = produce();
            }
            index = -1;
        }
        public T GetInstance()
        {
            index = (index + 1) % objs.Length;
            return objs[index];

        }


    }
}