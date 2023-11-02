using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("KeyBindings")]
    public KeyCode jumpKey = KeyCode.Space;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;
    public int RaysToShoot = 16;
    private int leftDetect;
    private int rightDetect;

    [Header("Exiting")]
    public float exitWallTime;
    private bool exitingWall;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;
    private PlayerMovement pm;
    private PlayerCam cam;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        cam = Camera.main.GetComponent<PlayerCam>();

        wallRunTimer = maxWallRunTime;
        exitWallTimer = exitWallTime;
        leftDetect = rightDetect = RaysToShoot / 2;
    }

    private void Update()
    {
        CheckForWall();
        Debug.Log(wallLeft + "," + wallRight);
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallRunning)
        {
            WallRunMovement();
        }
    }

    private void CheckForWall()
    {

        //Shoots 4 Rays On Both Left and Right Side for More Generous Wall Detection
        float totalAngle = 180;
        float delta = totalAngle / (RaysToShoot * 2);
        float offset = 45;
        for (int i = 0; i < RaysToShoot; i++)
        {
            var dir = Quaternion.Euler(0, offset + i * delta, 0) * orientation.forward;

            //This looks complicated, but essentially it makes it so if any of the rays hit than it counts as wall running (before it was they all had to hit)
            if (!Physics.Raycast(transform.position, -dir, out rightWallHit, wallCheckDistance, whatIsWall) && leftDetect > 0)
            {
                leftDetect -= 1;
            } else if (Physics.Raycast(transform.position, -dir, out rightWallHit, wallCheckDistance, whatIsWall) && leftDetect < (RaysToShoot / 2)) {
                leftDetect += 1;
            }

            if (!Physics.Raycast(transform.position, dir, out leftWallHit, wallCheckDistance, whatIsWall) && rightDetect > 0)
            {
                rightDetect -= 1;
            } else if (Physics.Raycast(transform.position, dir, out leftWallHit, wallCheckDistance, whatIsWall) && rightDetect < (RaysToShoot / 2))
            {
                rightDetect += 1;
            }
        }
        wallLeft = !(leftDetect == 0);
        wallRight = !(rightDetect == 0);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //Get Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallRunning)
                StartWallRun();

            //Wallrun Timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime; 

            if (wallRunTimer < 0 && pm.wallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            //Wall Jump
            if (Input.GetKeyDown(jumpKey)) WallJump();
        }

        //State 2 - Exiting Wall
        else if (exitingWall)
        {
            if (pm.wallRunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
            {
                exitingWall = false;
                exitWallTimer = exitWallTime;
            }
        }

        //State 3 - Not Wall Running
        else
        {
            if (pm.wallRunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.wallRunning = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        wallRunTimer = maxWallRunTime;

        //Apply Camera Effects
        cam.DoFOV(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);
    }

    private void WallRunMovement()
    {
        rb.useGravity = useGravity;
       
        // Get Forward Direction
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
        wallForward = new Vector3(wallForward.x, wallForward.y, wallForward.z);

        //Check if Player Running "Backwards" on Wall
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        //Forward Force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //Push Towards Wall
        if (!(wallLeft && horizontalInput > 0f) || !(wallRight && horizontalInput < 0f)) 
            rb.AddForce(-wallNormal * 100f, ForceMode.Force);

        //Counteract Gravity
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallRunning = false;
        wallRunTimer = maxWallRunTime;

        //Reset Camera Effects
        cam.DoFOV(cam.fov);
        cam.DoTilt(0);
    }

    private void WallJump()
    {
        //Enter Exiting State
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight? rightWallHit.normal: leftWallHit.normal;

        Vector3 force = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //Reset y Velocity And Add Force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(force, ForceMode.Impulse);
    }
}
