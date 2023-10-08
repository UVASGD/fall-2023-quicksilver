using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    //handles the state of swinging
    //FROM successfully engaging a grapple
    //UNTIL grapple relseased OR TODO (grounded? time? speed?)
    //Default transitions to FreeFall

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("States")]
    public bool swinging = false;
    public bool swingKeyStillDown = false;

    [Header("Swinging")]
    public float maxRopeLength = 25f;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public PlayerCam camScript;

    //for calculations
    [HideInInspector] public Vector3 swingPoint;
    private SpringJoint joint;
    public float startRopeLength;
    public float extraGravity;

    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartSwing()
    {
        swinging = true;
        swingKeyStillDown = true;

        createAndConfigureJoint();

        startRopeLength = getRopeLength();

        //prepare line renderer (for showing the rope)
        lr.positionCount = 2;
        
        //change the FOV
        camScript.DoFOV(90f);

        StartCoroutine("whileSwinging");
    }

    public void SwingKeyReleased()
    {
        swingKeyStillDown = false;
    }

    public void EndSwing()
    {
        //reset FOV
        camScript.DoFOV(camScript.fov);

        //stop showing the rope
        lr.positionCount = 0;

        //remove the joint component
        Destroy(joint);

        swingKeyStillDown = false;
        swinging = false;
    }

    IEnumerator whileSwinging()
    {
        while (swinging && swingKeyStillDown)
        {
            Vector3 rope = getRopeVector();
            bool ropeIsTaut = rope.magnitude >= startRopeLength; //* 0.95f; ???

            //velocities
            Vector3 vel = rb.velocity;
            Vector3 vRad = Vector3.Project(vel, rope.normalized);
            Vector3 vTan = vel - vRad;
            /*
            if (ropeIsTaut)
            {
                rb.AddForce
            }
            */
            //extra falling gravity
            if (vel.y < 0)
            {
                //adjust to push the correct amount in the radial direction
                Vector3 extraGravityVector = Vector3.zero;
                if (ropeIsTaut)
                {
                    //adjust to push the correct amount in the radial direction
                    extraGravityVector = vRad * extraGravity;
                    extraGravityVector *= Mathf.Cos(Vector3.Angle(vel, Vector3.down));
                } else
                {
                    //regular extra gravity
                    extraGravityVector = Vector3.down * extraGravity;
                }
                rb.AddForce(extraGravityVector, ForceMode.Impulse);
            }


            yield return new WaitForFixedUpdate();
        }

        //override transition
        if (!swinging) yield break;

        //default transition
        pm.OnSwingCompleted();
    }

    //rope getters
    private Vector3 getRopeVector()
    {
        if (!swinging) return Vector3.zero;
        return player.position - swingPoint;
    }
    private float getRopeLength()
    {
        if (!swinging) return 0;
        return getRopeVector().magnitude;
    }

    //joint
    private void createAndConfigureJoint()
    {
        //create joint
        joint = player.gameObject.AddComponent<SpringJoint>();

        //attach joint to swing point (it is already attached to player)
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        //set range that the joint will try to maintain
        float distanceFromSwingPoint = Vector3.Distance(player.position, swingPoint);
        joint.maxDistance = distanceFromSwingPoint * 0.8f;
        joint.minDistance = distanceFromSwingPoint * 0.25f;

        //set joint constants
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }

    
    //rope drawing
    private void LateUpdate()
    {
        DrawRope();
    }
    void DrawRope()
    {
        //check if joint is active
        if (!swinging) return;

        //move the line renderer
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

}
