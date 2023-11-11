using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airdashing : MonoBehaviour
{

    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;


    private float horizontalInput;
    private float verticalInput;

    private bool canDash;
    private bool beenDashed;

    [Header("Airdashing")]
    public float dashForce;
    public float dashUpwardForce;
  



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        canDash = false;
        beenDashed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.state == PlayerMovement.MovementState.air && !beenDashed) {
            canDash = true;
        }

        if (pm.grounded)
        {
            canDash = false;
            beenDashed = false;

        }
        if (canDash && Input.GetMouseButtonDown(1))
        {
            AirDashMovement();
        }

    }

    private Vector3 GetDirection(Transform forwardT)
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();
        direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;

        if (horizontalInput == 0 && verticalInput == 0)
        {
            direction = forwardT.forward;
        }
        return direction.normalized;
    }


    private void AirDashMovement()
    {
        Vector3 direction = GetDirection(orientation);
        Vector3 totalForce = direction * (dashForce*10) + orientation.up * dashUpwardForce;
        rb.AddForce(totalForce, ForceMode.Impulse);
        canDash = false;
        beenDashed = true;
    }
}
