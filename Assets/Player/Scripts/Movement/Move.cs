using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    //covers AWSD movement for all MovementStates where it is simple/similar to normal grounded movement

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("Speeds")]
    public float forwardSpeed;
    public float strafeSpeed;
    public float backwardSpeed;
    [Range(0, 1)] public float airMultipler;

    [Header("Drags")]
    [Range(0, 1)] public float groundDrag;
    [Range(0, 1)] public float airDrag;

    [Header("Stick to Ground")]
    public bool stickToGround;
    public float stickingForce;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyDrag();
    }

    private void MovePlayer()
    {
        if (pm.state == PlayerMovementStateMachine.MovementState.normal)
            MovePlayer(1);
        if (pm.state == PlayerMovementStateMachine.MovementState.falling)
            MovePlayer(airMultipler);
        if (pm.state == PlayerMovementStateMachine.MovementState.jumping)
            MovePlayer(airMultipler);
    }

    private void MovePlayer(float moveMultiplier)
    {
        //strafe
        Vector3 moveVector = pm.orientation.right * pm.horizontalInput * strafeSpeed;

        //add front-back
        if (pm.verticalInput > 0)
            moveVector += pm.orientation.forward * pm.verticalInput * forwardSpeed;
        else
            moveVector += pm.orientation.forward * pm.verticalInput * backwardSpeed;

        //apply ground adjustments
        moveVector = AdjustIfGrounded(moveVector);

        //apply multiplier (reduces movement forces while in air)
        moveVector *= moveMultiplier;

        //apply movement force
        rb.AddForce(moveVector, ForceMode.Impulse);

    }

    private Vector3 AdjustIfGrounded(Vector3 moveVector)
    {
        //if not grounded
        if (pm.state != PlayerMovementStateMachine.MovementState.normal)
            return moveVector;

        //perform raycast
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, pm.playerHeight * 0.5f + 0.3f);

        //find upward normal
        Vector3 upwardNormal = hit.normal;
        if (Vector3.Dot(hit.normal, Vector3.up) < 0)
            upwardNormal *= -1;

        //adjust move direction for slope
        moveVector = Vector3.ProjectOnPlane(moveVector, upwardNormal);

        //TODO re-semi-normalize

        //apply ground stickiness
        if (stickToGround)
            rb.AddForce(-upwardNormal * stickingForce, ForceMode.Impulse);

        return moveVector;
    }

    private void ApplyDrag()
    {
        if (pm.state == PlayerMovementStateMachine.MovementState.normal)
            ApplyDrag(groundDrag);
        if (pm.state == PlayerMovementStateMachine.MovementState.falling)
            ApplyDrag(airDrag);
        if (pm.state == PlayerMovementStateMachine.MovementState.jumping)
            ApplyDrag(airDrag);
    }

    private void ApplyDrag(float drag)
    {
        Vector3 moveVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.velocity -= moveVel * drag;
    }

}
