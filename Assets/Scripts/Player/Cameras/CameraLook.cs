using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public bool hideCursor = true; // Is the cursor hidden?
    public Vector2 speed = new Vector2(120f, 120f); 
    public float yMinLimit = -20f, yMaxLimit = 80f; 
    
    private float x, y;                         // Y degrees of rotation

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
        Vector3 angles = transform.eulerAngles;
        // Set X and Y degrees to current camera rotation
        x = angles.y;
        y = angles.x;
    }

    void Update()
    {
        // Cursor mode
        Cursor.lockState = hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = hideCursor ? false : true;
        
        // Rotate the camera based on Mouse X and Mouse Y inputs
        x += Input.GetAxis("Mouse X") * speed.x * Time.deltaTime;
        y -= Input.GetAxis("Mouse Y") * speed.y * Time.deltaTime;

        // Clamp the angle using a custom 'ClampAngle' function defined in this script
        y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

        // Rotate the transform using euler angles (y for X rotation and x for Y rotation)
        transform.parent.rotation = Quaternion.Euler(0, x, 0);
        transform.localRotation = Quaternion.Euler(y, 0, 0);

    }
}