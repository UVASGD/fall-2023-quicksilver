using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Swinging")]
    public float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;

    [Header("Flags")]
    public bool inRange = false;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public PlayerCam pc;
    private PlayerMovement pm;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        inRange = CheckRange();
        if (Input.GetMouseButtonDown(0)) StartSwing();
        if (Input.GetMouseButtonUp(0)) StopSwing();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private bool CheckRange()
    {
        RaycastHit hit;
        return Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable);
    }

    private void StartSwing()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            pm.state = PlayerMovement.MovementState.grappling;
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            // Range Grapple Will Try To Maintain
            joint.maxDistance = distanceFromPoint * 0.75f;
            joint.minDistance = distanceFromPoint * 0.0f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            pc.DoFOV(95f);
        }
    }

    private void StopSwing()
    {
        pc.DoFOV(pc.fov);
        lr.positionCount = 0;
        pm.state = PlayerMovement.MovementState.air;
        Destroy(joint);
    }

    void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }
}
