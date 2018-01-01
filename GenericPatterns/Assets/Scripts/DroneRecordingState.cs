using UnityEngine;
using System.Collections;
using KristianPiacenti.SimpleStateMachine;

public class DroneRecordingState : State<Drone>
{
    private static DroneRecordingState _instance;
    public static DroneRecordingState Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DroneRecordingState();
            return _instance;
        }
    }
    public override void Enter(Drone owner)
    {
        Debug.Log("Starting Recording...");
        owner.StartCoroutine(RecordForSeconds(owner,4));
    }
    public override void Exit(Drone owner)
    {
        Debug.Log("Stopped Recording!");

    }
    public override void Update(Drone owner)
    {

    }
    private IEnumerator RecordForSeconds(Drone owner, float seconds) {
        yield return new WaitForSeconds(seconds);
        owner.StateMachine.ChangeState(DroneSleepingState.Instance);
    }

}
