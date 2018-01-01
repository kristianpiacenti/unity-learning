using UnityEngine;

namespace KristianPiacenti.SimpleStateMachine
{
    public class StateMachine<T> where T : class
    {

        public State<T> CurrentState { get; private set; }
        private T owner;

        public StateMachine(T _owner){
            owner = _owner;        
        }

        public void ChangeState(State<T> newState)
        {
            if (CurrentState != null)
                CurrentState.Exit(owner);
            CurrentState = newState;
            CurrentState.Enter(owner);
        }

        public void Update() {
            if (CurrentState != null)
                CurrentState.Update(owner);
        }

    }
}

