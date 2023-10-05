using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFX : MonoBehaviour
{
    Rigidbody rb;

    public ParticleSystem speedLinesPS;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb.velocity.magnitude >= 12)
        {
            speedLinesPS.Play();
        }
        else
        {
            speedLinesPS.Stop();
        }
    }
}
