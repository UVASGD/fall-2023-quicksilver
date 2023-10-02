using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    //movement scripts
    Jump jump;
    FreeFall freeFall;
    Normal normal;
    Move move;

    [Header("States")]
    public MovementState state = MovementState.falling;
    public bool grounded = false;

    [Header("Input")]
    public float horizontalInput = 0;
    public float verticalInput = 0;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayerMask;


    void Start()
    {
        jump = GetComponent<Jump>();
        freeFall = GetComponent<FreeFall>();
        normal = GetComponent<Normal>();

        groundLayerMask = LayerMask.GetMask("whatIsGround");
    }

    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);

        //get axis input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        ProcessInput();

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

    void ApplyAWSDMovement()
    {
        
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
    private void initiateNormal()
    {
        endCurrentState();
        state = MovementState.normal;
        normal.StartNormal();
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
        normal.StartNormal();
    }
    public void OnNormalCompleted()
    {
        normal.EndNormal();
        state = MovementState.falling;
        freeFall.StartFall();
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
        if (state == MovementState.normal)
        {
            normal.EndNormal();
        }
    }

    public enum MovementState
    {
        normal,
        falling,
        jumping
    }

}
