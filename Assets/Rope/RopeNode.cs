using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour
{
    private Rope rope;
    private Rigidbody rb;

    private float[] x = new float[10];
    private float[] y = new float[10];
    private float[] z = new float[10];

    private int i = -1;

    private Vector3 extraVelFromConstraint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rope = transform.parent.GetComponent<Rope>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //this has to be in first update not start
        //setup x y and z
        if (i == -1)
            for (int i = 0; i < 10; i++)
            {
                x[i] = transform.position.x;
                y[i] = transform.position.y;
                z[i] = transform.position.z;
            }

        //cap velocity
        if (rb.velocity.magnitude > rope.maxNodeVelocity)
        {
            rb.velocity = rb.velocity.normalized * rope.maxNodeVelocity;
        }

        //add new pos to x y and z
        i++;
        if (i % 10 == 0)
        {
            x[i / 10] = transform.position.x;
            y[i / 10] = transform.position.y;
            z[i / 10] = transform.position.z;
            if (i == 90) i = 0;
        }

        //jiggle reduction
        float jiggle = stdv(x) + stdv(y) + stdv(z);
        if (jiggle > rope.maxJiggle)
        {
            rb.velocity *= 0.1f;
        }

        //max length
        if (!hasConnection()) return;
        Vector3 jointVector = transform.position - getConnection().transform.position;
        if (jointVector.magnitude > rope.maxJointLength)
        {
            getConnection().transform.position = transform.position - jointVector.normalized * 0.95f * rope.maxJointLength;
            /*
            rb.AddForce(jointVector.normalized * rope.constraintForce, ForceMode.Impulse);
            Vector3 accel = jointVector.normalized * rope.constraintForce / rb.mass;
            extraVelFromConstraint += accel * Time.deltaTime;
        } else
        {
            rb.velocity -= extraVelFromConstraint;
            extraVelFromConstraint = Vector3.zero;*/
        }

        //^^theres a return in there
    }

    private float stdv(float[] a)
    {
        float sum = 0;
        foreach (float f in a)
        {
            sum += f;
        }
        float ave = sum / a.Length;
        float sumOfSquareDifferences = 0;
        foreach (float f in a)
        {
            sumOfSquareDifferences += (f-ave)*(f-ave);
        }
        return Mathf.Sqrt(sumOfSquareDifferences / a.Length);
    }

    private bool hasConnection()
    {
        if (GetComponent<SpringJoint>()) return true;
        return false;
    }

    private GameObject getConnection()
    {
        return GetComponent<SpringJoint>().connectedBody.gameObject;
    }

}
