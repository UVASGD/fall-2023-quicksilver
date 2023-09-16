using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    [Header("Cam Settings")]
    public float fov = 80f;

    [Header("References")]
    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //Get Mouse Input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Prevents looking too far up or down

        //Rotate cam and orientation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFOV(float endvalue)
    {
        GetComponent<Camera>().DOFieldOfView(endvalue, 0.25f);
    }

    public void DoTilt(float tilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, tilt), 0.25f);
    }
}
