using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
    //handles the state of sliding
    //FROM when the slide button is pressed
    //UNTIL the slide button is released or the slide sustain runs out
    //By default transitions to normal

    private PlayerMovementStateMachine pm;
    private Rigidbody rb;

    [Header("States")]
    public bool sliding = false;
    public bool slideKeyStillDown = false;

    [Header("Sliding")]
    public float boostForce;
    [Range(0f, 1f)] public float slideDrag;
    [Range(0.1f, 1f)] public float slideYScale;
    public float minSlidingVelocity;


    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovementStateMachine>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartSlide()
    {
        sliding = true;
        slideKeyStillDown = true;

        //crouch
        pm.playerObj.localScale = new Vector3(pm.playerObj.localScale.x, pm.playerObj.localScale.y * slideYScale, pm.playerObj.localScale.z);
        
        //apply boost
        //TODO maybe make it so you can slide from slightly in the air and your downward velocity gets added to your boost
        rb.AddForce(pm.orientation.forward * boostForce, ForceMode.Impulse);

        //make sure player sticks to ground
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        StartCoroutine("whileSliding");
    }

    public void SlideKeyReleased()
    {
        slideKeyStillDown = false;
    }

    public void EndSlide()
    {
        //uncrouch
        pm.playerObj.localScale = new Vector3(pm.playerObj.localScale.x, pm.playerObj.localScale.y / slideYScale, pm.playerObj.localScale.z);

        slideKeyStillDown = false;
        sliding = false;
    }

    IEnumerator whileSliding()
    {
        //jump sustain
        while (sliding && slideKeyStillDown && rb.velocity.magnitude > minSlidingVelocity)
        {
            //apply drag
            //TODO maybe try force based?
            Vector3 velocityReduction = rb.velocity.normalized * Mathf.Min(0.25f,(2/rb.velocity.magnitude + rb.velocity.magnitude * slideDrag));
            rb.velocity -= velocityReduction;

            yield return new WaitForFixedUpdate();
        }

        //override transition
        if (!sliding) yield break;

        //default transition
        pm.OnSlideCompleted();

    }
}
