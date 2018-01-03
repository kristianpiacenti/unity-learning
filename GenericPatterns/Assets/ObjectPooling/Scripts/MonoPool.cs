/*TUTORIAL
    https://www.youtube.com/watch?v=_mxLLRQ1BQY 
*/

using UnityEngine;

namespace Piacenti.Pooling
{
    public class MonoPool : MonoBehaviour
    {
        [SerializeField]
        private int initSize = 5;
        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private PoolType poolType = PoolType.Queue;

        public IPool<GameObject> MyPool
        {
            get;
            private set;
        }


        private void Awake()
        {
            if (prefab == null)
            {
                Debug.LogError("No prefab in pool");
                return;
            }
            switch (poolType)
            {
                case PoolType.List:
                    MyPool = new ListPool<GameObject>(Produce, InUse, initSize);
                    break;
                case PoolType.Queue:
                    MyPool = new QueuePool<GameObject>(Produce, initSize);
                    break;
                default:
                    return;

            }

        }

        protected virtual GameObject Produce()
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            return obj;
        }
        protected virtual bool InUse(GameObject obj)
        {
            return obj.activeInHierarchy;
        }
    }
    public enum PoolType { Queue, List }
}