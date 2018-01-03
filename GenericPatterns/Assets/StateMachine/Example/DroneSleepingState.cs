using UnityEngine;
using System.Collections;
using Piacenti.SimpleStateMachine;
public class DroneSleepingState : State<Drone>
{
    private static DroneSleepingState _instance;
    public static DroneSleepingState Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DroneSleepingState();
            return _instance;
        }
    }
    public override void Enter(Drone owner)
    {
        Debug.Log("Sleeping...");
        owner.StartCoroutine(SleepForSeconds(owner, 2));
    }
    public override void Exit(Drone owner)
    {
        Debug.Log("Awaking!");
    }
    public override void Update(Drone owner)
    {
    }
    private IEnumerator SleepForSeconds(Drone owner, float seconds) {
       
        yield return new WaitForSeconds(seconds);
        owner.StateMachine.ChangeState(DroneRecordingState.Instance);

    }
    
}
