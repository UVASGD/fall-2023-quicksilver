using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    //handles the state of swinging
    //FROM
    //UNTIL 
    //Default transitions to TODO

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("States")]
    public bool wallRunning = false;

    [Header("WallRunning")]
    public bool temp; //TODO

    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartWallRun()
    {
        wallRunning = true;

        StartCoroutine("whileSwinging");
    }

    public void EndWallRun()
    {
        wallRunning = false;
    }

    IEnumerator whileWallRunning()
    {
        //TODO
        while (wallRunning)
        {
            yield return new WaitForFixedUpdate();
        }

        //override transition
        if (!wallRunning) yield break;

        //default transition
        pm.OnWallRunCompleted();
    }

}
