using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    //movement scripts
    Jump jump;
    FreeFall freeFall;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayerMask;

    [Header("States")]
    public MovementState state = MovementState.falling;
    public bool grounded = false;

    void Start()
    {
        jump = GetComponent<Jump>();
        freeFall = GetComponent<FreeFall>();

        groundLayerMask = LayerMask.GetMask("whatIsGround");
    }

    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);

        ProcessInput();

        //check for falling (normal doesn't have a coroutine so need to check here)
        if (state == MovementState.normal && !grounded) initiateFall();
    }

    void ProcessInput()
    {

        if (Input.GetKeyDown(jumpKey) && grounded)
        {
            initiateJump();
        }
        if (Input.GetKeyUp(jumpKey))
        {
            jump.JumpKeyReleased();
        }
    }

    //override transitions
    private void initiateJump()
    {
        endCurrentState();
        state = MovementState.jumping;
        jump.StartJump();
    }
    private void initiateFall()
    {
        endCurrentState();
        state = MovementState.falling;
        freeFall.StartFall();
    }

    //default transitions
    public void OnJumpCompleted()
    {
        jump.EndJump();
        state = MovementState.falling;
        freeFall.StartFall();
    }
    public void OnFallCompleted()
    {
        freeFall.EndFall();
        state = MovementState.normal;
    }


    //other/resource/enums
    private void endCurrentState()
    {
        if (state == MovementState.falling)
        {
            freeFall.EndFall();
        }
        if (state == MovementState.jumping)
        {
            jump.EndJump();
        }
    }

    public enum MovementState
    {
        normal,
        falling,
        jumping
    }

}
