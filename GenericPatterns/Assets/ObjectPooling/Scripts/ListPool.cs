/*TUTORIAL
    https://www.youtube.com/watch?v=_mxLLRQ1BQY 
*/

using System.Collections.Generic;
using System;

namespace Piacenti.Pooling
{
    public class ListPool<T> : IPool<T> where T : class
    {

        private List<T> objs;
        private Func<T> produce;
        private Func<T, bool> inUse;
        public ListPool(Func<T> produce, Func<T, bool> inUse, int initSize = 0)
        {
            this.produce = produce;
            this.inUse = inUse;
            objs = new List<T>();
            InitializePool(initSize);

        }

        private void InitializePool(int size)
        {
            while (objs.Count < size)
                objs.Add(produce());
        }
        public T GetInstance()
        {
            foreach (var item in objs)
                if (!inUse(item))
                    return item;

            objs.Add(produce());
            return objs[objs.Count - 1];
        }
    }
}
