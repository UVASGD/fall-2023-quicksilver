using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    //movement scripts
    Jump jump;
    FreeFall freeFall;
    Normal normal;
    public Move move;

    [Header("States")]
    public MovementState state = MovementState.falling;
    public bool grounded = false;

    [Header("Input")]
    public float horizontalInput = 0;
    public float verticalInput = 0;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayerMask;

    [Header("Crouch")]
    [Range(0.1f,1f)] public float crouchYScale;


    void Start()
    {
        jump = GetComponent<Jump>();
        freeFall = GetComponent<FreeFall>();
        normal = GetComponent<Normal>();
        move = GetComponent<Move>();

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

        //crouch
        manageCrouch();
    }

    void ProcessInput()
    {
        //Jump
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

    public void manageCrouch()
    {
        //CROUCH
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * crouchYScale, transform.localScale.z);
            move.rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); //Makes Sure Player isn't hovering after crouching
        }

        //Release Crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / crouchYScale, transform.localScale.z);
        }
    }

}
