/*TUTORIAL
    https://www.youtube.com/watch?v=PaLD1t-kIwM
*/

namespace Piacenti.SimpleStateMachine
{
    public abstract class State<T> where T : class
    {
        
        public abstract void Enter(T owner);
        public abstract void Exit(T owner);
        public abstract void Update(T owner);
       
    }
}