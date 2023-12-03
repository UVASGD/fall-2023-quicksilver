using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject endPos;
    public float speed = 1.0f;

    private bool moving = false;
    private bool opening = true;
    public GameObject Door_One;


    private void Update()
    {
        if (moving)
        {
            if (opening)
            {
                MoveDoor(endPos.transform.position);
            }
        }
    }

    void MoveDoor(Vector3 goalPos)
    {
        float dist = Vector3.Distance(Door_One.transform.position, goalPos);
        if (dist > .1f)
        {
            Door_One.transform.position = Vector3.Lerp(Door_One.transform.position, (goalPos), speed * Time.deltaTime);
        }
    }

    public bool Moving
    {
        get { return moving; }
        set { moving = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!moving)
            {
                moving = true;
            }
        }
    }
}
