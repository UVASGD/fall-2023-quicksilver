using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    //covers AWSD movement for all MovementStates where it is simple/similar to normal grounded movement

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("Gravity")]
    public float gravity;
    public float downGravity;

    [Header("Speeds")]
    public float forwardSpeed;
    public float strafeSpeed;
    public float backwardSpeed;
    [Range(0, 1)] public float airMultipler;

    [Header("Drags")]
    [Range(0, 1)] public float groundDrag;
    [Range(0, 0.01f)] public float airDrag;
    [Range(0, 0.01f)] public float verticalFallingDrag;

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
        ApplyGravity();
        MovePlayer();
        ApplyDrag();
    }

    private void ApplyGravity()
    {
        if (rb.velocity.y > 0) rb.AddForce(Vector3.down * gravity);
        else rb.AddForce(Vector3.down * downGravity);
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
            ApplyLinearDrag(groundDrag);
        if (pm.state == PlayerMovementStateMachine.MovementState.jumping)
            ApplySquareDrag(airDrag);
        if (pm.state == PlayerMovementStateMachine.MovementState.falling)
        {
            ApplySquareDrag(airDrag);
            ApplyVerticalFallingDrag();
        }
    }

    private void ApplyLinearDrag(float drag)
    {
        Vector3 horVel = pm.getHorVel();
        rb.velocity -= horVel * drag;
    }
    private void ApplySquareDrag(float drag)
    {
        Vector3 horVel = pm.getHorVel();
        rb.velocity -= horVel * horVel.magnitude * drag;
    }
    private void ApplyVerticalFallingDrag()
    {
        if (rb.velocity.y > 0) return;
        rb.velocity += Vector3.up * Mathf.Pow(rb.velocity.y, 4) * verticalFallingDrag * Mathf.Pow(10,-4);
    }

}
