using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private List<MovingPlatformTransform> positions = new List<MovingPlatformTransform>();//the transforms the object will travel through
                                                                               //make sure to include the current starting transform as that will automatically be included as the first position
    //[SerializeField] private float travelTime;//the total time in takes the object to travel from one position to another

    private int curIndex = 0;
    private bool backwards = false;
    private float time = 0;

    private Vector3 spd;
    // Start is called before the first frame update
    void Start()
    {
        //positions.Insert(0,);//include the current transform as the starting transform
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*Debug.Log("next index: " + (curIndex + (backwards ? -1 : 1)));
        Transform nextTransform = positions[(curIndex + (backwards ? -1 : 1))];
        transform.position = Vector3.MoveTowards(transform.position, nextTransform.position, (travelTime * Time.deltaTime)/ Vector3.Distance(transform.position, nextTransform.position)); 
        //Debug.Log("moving towards " + positions[curIndex + (backwards ? -1 : 1)].position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, positions[curIndex + (backwards ? -1 : 1)].rotation, travelTime * Time.deltaTime);
        if (transform.position.Equals(positions[curIndex + (backwards ? -1 : 1)].position)/* && transform.rotation.Equals(positions[curIndex + (backwards ? -1 : 1)]))
        {
            Debug.Log("switch target");
            curIndex += backwards ? -1 : 1;
            if (backwards && curIndex == 0 || !backwards && curIndex == positions.Count - 1)
            {
                //curIndex += backwards ? -1 : 1;
                Debug.Log("switch direction");
                backwards = !backwards;
            }
        
        }*/
        Vector3 initialPosition = transform.position;
        MovingPlatformTransform nextTransform = positions[(curIndex + (backwards ? -1 : 1))];
        MovingPlatformTransform curTransform = positions[curIndex];
        transform.position = Vector3.Lerp(curTransform.transform.position, nextTransform.transform.position, time);
        transform.rotation = Quaternion.Lerp(curTransform.transform.rotation, nextTransform.transform.rotation,time);
        time += Time.deltaTime / (backwards ? curTransform.time : nextTransform.time);
        spd = (transform.position - initialPosition);
        if(time >= 1)
        {
            curIndex += backwards ? -1 : 1;
            if (backwards && curIndex == 0 || !backwards && curIndex == positions.Count - 1)
            {
                //Debug.Log("switch direction");
                backwards = !backwards;
            }
            time = 0;
        }

    }
    public Vector3 getSpeed()
    {
        return spd;
    }
    [Serializable]private class MovingPlatformTransform
    {
        [SerializeField] public Transform transform;
        [SerializeField] public float time;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent.parent = gameObject.transform;
            Debug.Log("hi");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent.parent = null;
            Debug.Log("bye");
        }
    }
}

