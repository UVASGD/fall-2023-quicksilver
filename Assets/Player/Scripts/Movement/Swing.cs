using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    //handles the state of swinging
    //FROM
    //UNTIL 
    //Default transitions to TODO

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("States")]
    public bool swinging = false;

    [Header("Swinging")]
    public bool temp; //TODO

    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartSwing()
    {
        swinging = true;

        StartCoroutine("whileSwinging");
    }

    public void EndSwing()
    {
        swinging = false;
    }

    IEnumerator whileSwinging()
    {
        //TODO
        while (swinging)
        {
            yield return new WaitForFixedUpdate();
        }

        //override transition
        if (!swinging) yield break;

        //default transition
        pm.OnWallRunCompleted();
    }

}
