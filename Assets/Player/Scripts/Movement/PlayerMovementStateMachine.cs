using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    [HideInInspector] public Rigidbody rb;
    //movement scripts
    Jump jump;
    FreeFall freeFall;
    Normal normal;
    Slide slide;
    Swing swing;
    WallRun wallRun;

    [Header("States")]
    public MovementState state = MovementState.falling;
    public bool grounded = false;

    [Header("Input")]
    public float horizontalInput = 0;
    public float verticalInput = 0;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode slideKey = KeyCode.C;

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
        slide = GetComponent<Slide>();
        swing = GetComponent<Swing>();
        wallRun = GetComponent<WallRun>();

        rb = GetComponent<Rigidbody>();

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

        manageCrouch();
    }

    void ProcessInput()
    {
        //Jump
        if (Input.GetKeyDown(jumpKey) && canJump())
            initiateJump();
        if (Input.GetKeyUp(jumpKey))
            jump.JumpKeyReleased();

        //Slide
        if (Input.GetKeyDown(slideKey) && canSlide())
            initiateSlide();
        if (Input.GetKeyUp(slideKey))
            slide.SlideKeyReleased();
 
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
    private void initiateSlide()
    {
        endCurrentState();
        state = MovementState.sliding;
        slide.StartSlide();
    }
    private void initiateSwing()
    {
        endCurrentState();
        state = MovementState.swinging;
        swing.StartSwing();
    }
    private void initiateWallRun()
    {
        endCurrentState();
        state = MovementState.wallRunning;
        wallRun.StartWallRun();
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
    public void OnSlideCompleted()
    {
        slide.EndSlide();
        state = MovementState.falling;  //will automatically transition to normal if grounded, won't apply ground friction if slid off edge
        freeFall.StartFall();
    }
    public void OnSwingCompleted()
    {
        swing.EndSwing();
        state = MovementState.falling;  //will automatically transition to normal if grounded
        freeFall.StartFall();
    }
    public void OnWallRunCompleted()
    {
        wallRun.EndWallRun();
        state = MovementState.falling;  //will automatically transition to normal if grounded
        freeFall.StartFall();
    }

    //what transitions are allowed
    private bool canJump()
    {
        if (state == MovementState.falling ||
            state == MovementState.jumping)
            return false;
        else return true;
    }
    private bool canSlide()
    {
        //TODO maybe make it so you can slide from slightly in the air and your downward velocity gets added to your boost
        if (state == MovementState.falling ||
            state == MovementState.jumping ||
            state == MovementState.sliding)
            return false;
        else return true;
    }
    private bool canSwing()
    {
        //TODO
        return true;
    }
    private bool canWallRun()
    {
        //TODO
        return true;
    }


    //other/resource/enums
    private void endCurrentState()
    {
        switch(state)
        {
            case MovementState.normal:
                normal.EndNormal();
                break;
            case MovementState.falling:
                freeFall.EndFall();
                break;
            case MovementState.jumping:
                jump.EndJump();
                break;
            case MovementState.sliding:
                slide.EndSlide();
                break;
            case MovementState.swinging:
                swing.EndSwing();
                break;
            case MovementState.wallRunning:
                wallRun.EndWallRun();
                break;
        }
    }

    public enum MovementState
    {
        normal,
        falling,
        jumping,
        sliding,
        swinging,
        wallRunning
    }

    public void manageCrouch()
    {
        //CROUCH
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); //Makes Sure Player isn't hovering after crouching
        }

        //Release Crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / crouchYScale, transform.localScale.z);
        }
    }
}
