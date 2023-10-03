using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal : MonoBehaviour
{
    //handles the state of on the ground
    //FROM when all forces caused by user input stop acting on the player (except awsd motion)
    //UNTIL the player leaves ground
    //Default transitions to FreeFall

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("States")]
    public bool normal = false;

    [Header("Normal")]
    public float stickingForce;

    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartNormal()
    {
        normal = true;

        StartCoroutine("whileNormal");
    }

    public void EndNormal()
    {
        normal = false;
    }

    IEnumerator whileNormal()
    {
        while (normal && pm.grounded)
        {
            yield return new WaitForFixedUpdate();
        }

        //override transition
        if (!normal) yield break;

        //default transition
        pm.OnNormalCompleted();
    }

}
