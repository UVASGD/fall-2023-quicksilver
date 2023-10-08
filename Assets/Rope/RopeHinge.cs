using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeHinge : MonoBehaviour
{
    //nodes[0] is the root node, it is not the connectedBody on any other node
    private ArrayList nodes;

    [Header("References")]
    public GameObject ropeNodePrefab;
    public GameObject temp;

    [Header("Joint")]
    public float minNodeSeparation = 1;   //not the limit of motion, the range where force isn't applied
    public float maxNodeSeparation = 1;
    public float spring = 4.5f;
    public float damper = 0.7f;
    public float maxJointLength = 2;
    public float constraintForce = 20;

    [Header("Node")]
    public float nodeDrag = 0.01f;
    public float nodeAngularDrag = 0.5f;
    public float nodeMass = 0.1f;
    public float maxNodeVelocity = 2;
    public float maxJiggle = 2;


    void Awake()
    {
        nodes = new ArrayList();
    }

    private void Start()
    {
        GameObject firstNode = createNode(transform.position);
        nodes.Add(firstNode);
        fixRoot();
    }

    void Update()
    {
        //*
        //for debug
        if (Input.GetKeyDown(KeyCode.L))
        {
            LengthenRope(0.97f * Vector3.right + 0.2f * Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ShortenRope();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            attachEndTo(temp.GetComponent<Rigidbody>(), Vector3.right);
        }
        //*/
    }

    private void LengthenRope(Vector3 displacementFromEnd)
    {
        //lengthens the rope by adding one node to the end of the rope

        //if end is attached to something, store the fixed joint info
        Rigidbody fixedJointConnectedBody = null;
        Vector3 fixedJointConnectedAnchor = Vector3.zero;
        if (getEnd().GetComponent<FixedJoint>())
        {
            fixedJointConnectedBody = getEnd().GetComponent<FixedJoint>().connectedBody;
            fixedJointConnectedAnchor = getEnd().GetComponent<FixedJoint>().connectedAnchor;
            Destroy(getEnd().GetComponent<FixedJoint>());
        }

        //create a new rope node
        GameObject newNode = createNode(getEnd().transform.position + displacementFromEnd);

        //add a joint component to the last node
        HingeJoint joint = getEnd().AddComponent<HingeJoint>();

        //attach the other end of the joint to the new node
        joint.connectedBody = newNode.GetComponent<Rigidbody>();

        //configure the joint
        configureJoint(joint);

        //add the newNode to the end of nodes
        nodes.Add(newNode);

        //if end was attached to something, attach the new end to it
        if (fixedJointConnectedBody)
        {
            attachEndTo(fixedJointConnectedBody, fixedJointConnectedAnchor);
        }
    }

    private void ShortenRope()
    {
        //shortens the rope by removing one node from the end of the rope

        //can't destroy root
        if (nodes.Count <= 1) return;

        //remove the spring joint from the second to last node
        GameObject secondToLastNode = (GameObject)nodes[nodes.Count - 2];
        Destroy(secondToLastNode.GetComponent<HingeJoint>());

        //if end is attached to something, store the fixed joint info
        Rigidbody connectedBody = null;
        Vector3 connectedAnchor = Vector3.zero;
        if (getEnd().GetComponent<FixedJoint>())
        {
            connectedBody = getEnd().GetComponent<FixedJoint>().connectedBody;
            connectedAnchor = getEnd().GetComponent<FixedJoint>().connectedAnchor;
        }

        //destroy the last node
        Destroy(getEnd());

        //remove the last node from nodes
        nodes.RemoveAt(nodes.Count - 1);

        //if end was attached to something, attach the new end to it
        if (connectedBody)
        {
            attachEndTo(connectedBody, connectedAnchor);
        }
    }

    private void configureJoint(HingeJoint joint)
    {
        //set the anchors to the centers of the rope nodes
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector3.down*maxNodeSeparation;
        joint.anchor = Vector3.zero;
        /*
        //set joint constants
        joint.spring = spring;
        joint.damper = damper;

        //set the range where force will not be applied
        joint.maxDistance = minNodeSeparation;
        joint.minDistance = maxNodeSeparation;*/
    }

    private GameObject createNode(Vector3 position)
    {
        GameObject newNode = Instantiate(ropeNodePrefab, position, Quaternion.identity);
        newNode.transform.parent = transform;
        Rigidbody rb = newNode.GetComponent<Rigidbody>();
        rb.drag = nodeDrag;
        rb.angularDrag = nodeAngularDrag;
        rb.mass = nodeMass;
        return newNode;
    }

    private GameObject getRoot()
    {
        return (GameObject)nodes[0];
    }

    private GameObject getEnd()
    {
        return (GameObject)nodes[nodes.Count - 1];
    }

    public void fixRoot()
    {
        getRoot().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
    }
    public void fixEnd()
    {
        getEnd().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
    }
    public void freeRoot()
    {
        getRoot().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
    public void freeEnd()
    {
        getEnd().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    public void attachRootTo(Rigidbody r, Vector3 connectedAnchor)
    {
        freeRoot();

        FixedJoint joint = getRoot().AddComponent<FixedJoint>();

        joint.connectedBody = r;

        //anchors
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = connectedAnchor;
        joint.anchor = Vector3.zero;
    }

    public void attachEndTo(Rigidbody r, Vector3 connectedAnchor)
    {
        freeEnd();

        FixedJoint joint = getEnd().AddComponent<FixedJoint>();

        joint.connectedBody = r;

        //anchors
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = connectedAnchor;
        joint.anchor = Vector3.zero;
    }

    public void detachRoot()
    {
        Destroy(getRoot().GetComponent<FixedJoint>());
    }

    public void detachEnd()
    {
        Destroy(getEnd().GetComponent<FixedJoint>());
    }

}
