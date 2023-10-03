using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    //handles the state of jumping
    //FROM when the jump button is pressed
    //UNTIL the jump button is released or the jump sustain runs out (not the peak of the jump)
    //By default transitions to FreeFall

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("States")]
    public bool jumping = false;
    public bool jumpKeyStillDown = false;

    [Header("Jumping")]
    public float initialJumpForce;
    public float maxJumpSustainTime;
    public float jumpSustainForce;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartJump()
    {
        jumping = true;
        jumpKeyStillDown = true;

        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //add initial jump force
        rb.AddForce(transform.up * initialJumpForce, ForceMode.Impulse);

        StartCoroutine("whileJumping");
    }

    public void JumpKeyReleased()
    {
        jumpKeyStillDown = false;
    }

    public void EndJump()
    {
        jumpKeyStillDown = false;
        jumping = false;
    }

    IEnumerator whileJumping()
    {
        //jump sustain
        float remainingJumpSustain = maxJumpSustainTime;
        while (jumping && jumpKeyStillDown && remainingJumpSustain > 0)
        {
            remainingJumpSustain -= Time.fixedDeltaTime;

            //add small force to increase jump height
            rb.AddForce(transform.up * jumpSustainForce * (remainingJumpSustain / maxJumpSustainTime), ForceMode.Force);

            yield return new WaitForFixedUpdate();
        }

        //override transition
        if (!jumping) yield break;

        //default transition
        pm.OnJumpCompleted();

    }
}
