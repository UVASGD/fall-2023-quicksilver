using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("References")]
    public ParticleSystem ps;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;

    [HideInInspector]
    public float moveSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float strafeDampener;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [Range(0, 1)]
    public float airDecay;
    private float airDrag = 1;
    private float startAirDrag;

    [Header("Not anymore but Crouching")]
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public float slopeMultiplier;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    [HideInInspector]
    public Rigidbody rb;

    [Header("Flags")]
    public bool sliding;
    public bool grounded;
    private bool readyToJump;
    public bool wallRunning;
    public bool canSlide;
    public bool airDashing;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        wallRunning,
        grappling,
        sliding,
        air
    }

    private void Start()
    {
        canSlide = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        startAirDrag = airDrag;
    }

    private void Update()
    {
        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        
        MyInput();
        SpeedControl();
        StateHandler();

        //Handle Drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        //Speed FX
        if (rb.velocity.magnitude >= 13)
        {
            ps.Play();
        }
        else
        {
            ps.Stop();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //JUMP
        if (Input.GetKey(jumpKey) && readyToJump && grounded && state != MovementState.air)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        //Mode - Grappling (Handled in Swinging Script)
        if (state == MovementState.grappling)
        {
            return;
        }
        //Mode - WallRunning
        if (wallRunning)
        {
            state = MovementState.wallRunning;
            desiredMoveSpeed = wallRunSpeed;
        }

        //Mode - Sliding
        else if (sliding && state != MovementState.air)
        {
            state = MovementState.sliding;
            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }

        //Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        //Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        //Mode - Air
        else
        {
            canSlide = true;
            state = MovementState.air;
        }

        //Check If Desried Speed has Changed
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 2f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(LerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator LerpMoveSpeed() //Coroutine
    {
        //Interpolates movementSpeed to the Desired Value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        //Calculate Movement Dir
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Dampen Strafe Speed
        if (horizontalInput != 0 && verticalInput == 0)
        {
            moveSpeed *= strafeDampener;
        }

        //On Slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * slopeMultiplier * 10f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //On Ground
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            airDrag = startAirDrag;
        }

        //In Air
        else if (!grounded)
        {
            //Handle Air Drag If No Forward Input
            if (verticalInput == 0)
            {
                airDrag -= (airDecay * 0.01f);
                rb.velocity = new Vector3(rb.velocity.x *  airDrag, rb.velocity.y, rb.velocity.z * airDrag);
            }

            Vector3 movDir = (state == MovementState.wallRunning || state == PlayerMovement.MovementState.grappling) ? 
                moveDirection.normalized : moveDirection + (Vector3.down * 0.085f); // Makes falling just a lil snappier

            rb.AddForce(movDir * moveSpeed * airMultiplier * 10f, ForceMode.Force);
        }

        //Disable Gravity On Slope (Prevents Sliding)
        if(!wallRunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        
        //Limit Speed on Slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        //Limit Speed on Ground and Air
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //Limit Velocity
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }

        }
    }

    private void Jump()
    {
        exitingSlope = true;

        //Reset y Velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
