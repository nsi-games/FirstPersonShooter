using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float resolveSpeed = 10f;
    public bool hideCursor = true; // Is the cursor hidden?
    public Vector2 speed = new Vector2(120f, 120f); 
    public float minYAngle = -20f, maxYAngle = 80f; 
    
    private Vector3 euler, offset;                         // Y degrees of rotation
    private Vector3 targetOffset;

    // Use this for initialization
    void Start()
    {
        // Is the cursor supposed to be hidden?
        if (hideCursor)
        {
            // Lock...
            Cursor.lockState = CursorLockMode.Locked;
            // ... and hide the cursor
            Cursor.visible = false;
        }
        // Get current camera rotation
        euler = transform.eulerAngles;
    }



    void Update()
    {
        targetOffset = Vector3.Lerp(targetOffset, Vector3.zero, resolveSpeed * Time.deltaTime);
        offset = Vector3.MoveTowards(offset, targetOffset, resolveSpeed * Time.deltaTime);

        // Rotate the camera based on Mouse X and Mouse Y inputs
        euler.y += Input.GetAxis("Mouse X") * speed.x * Time.deltaTime;
        euler.x -= Input.GetAxis("Mouse Y") * speed.y * Time.deltaTime;
        // Clamp the angle using a custom 'ClampAngle' function defined in this script
        euler.x = Mathf.Clamp(euler.x, minYAngle, maxYAngle);
        // Rotate the transform using euler angles (y for X rotation and x for Y rotation)
        transform.parent.localEulerAngles = new Vector3(0, euler.y + offset.x, 0);
        transform.localEulerAngles = new Vector3(euler.x - offset.y, 0, 0);
    }

    public void ApplyOffset(Vector3 offset)
    {
        targetOffset = offset;
    }
}