using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFall : MonoBehaviour
{
    //handles the state of in air
    //FROM when all forces caused by user input stop acting on the player (except awsd motion)
    //this can include upward motion
    //UNTIL the player lands on the ground
    //By default transitions to Normal

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("States")]
    public bool falling = false;

    [Header("Falling")]
    public float extraFallingForce;

    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartFall()
    {
        falling = true;

        StartCoroutine("whileFalling");
    }

    public void EndFall()
    {
        falling = false;
    }

    IEnumerator whileFalling()
    {
        //until landing on the ground
        while (falling && !pm.grounded)
        {
            //awsd and horizontal drag handled in Move script
            //TODO
            //vertical drag (to create a falling terminal velocity)

            //add extra downward force while moving down
            if (rb.velocity.y < 0)
            {
                rb.AddForce(-transform.up * extraFallingForce * Time.deltaTime, ForceMode.Impulse);
            }
            
            yield return new WaitForFixedUpdate();
        }


        //override transition
        if (!falling) yield break;

        //default transition
        pm.OnFallCompleted();

    }
}