using Piacenti.SimpleStateMachine;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Drone : MonoBehaviour {
    public StateMachine<Drone> StateMachine {
        get;
        private set;
    }
    private void Awake()
    {
        StateMachine = new StateMachine<Drone>(this);
    }
    private void Start()
    {
        StateMachine.ChangeState(DroneSleepingState.Instance);
    }
    private void Update()
    {
        StateMachine.Update();
    }
}
