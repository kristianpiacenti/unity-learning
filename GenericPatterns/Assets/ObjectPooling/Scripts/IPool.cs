/*TUTORIAL
    https://www.youtube.com/watch?v=_mxLLRQ1BQY 
*/

namespace Piacenti.Pooling
{
    public interface IPool<T>
    {
        T GetInstance();
    }
}
