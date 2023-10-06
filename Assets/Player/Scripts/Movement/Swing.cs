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
    public float maxSwingDistance = 25f;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public PlayerCam camScript;

    //for calculations
    [HideInInspector] public Vector3 swingPoint;
    private SpringJoint joint;
    //private Vector3 currentGrapplePosition;

    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartSwing()
    {
        swinging = true;
        swingKeyStillDown = true;

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

        //prepare line renderer (for showing the rope)
        lr.positionCount = 2;
        //currentGrapplePosition = gunTip.position;

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
            yield return new WaitForFixedUpdate();
        }

        //override transition
        if (!swinging) yield break;

        //default transition
        pm.OnSwingCompleted();
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

        //@Nate wtf is this line
        //currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        //move the line renderer
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

}
