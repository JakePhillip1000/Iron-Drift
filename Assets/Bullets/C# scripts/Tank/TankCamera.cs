using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCamera : MonoBehaviour
{
    // Attach this to camera tank, create empty and specify transform to empty
    // set the terrain layer in layermask to 'terrain' (any name you want)
    float x, y;
    public float sensitivity = 3;
    public float distance = 6;
    public float minDistance = 3; 
    public float maxDistance = 15; 
    public Vector2 xminmax = new Vector2(-10, 60);
    public Transform target;
    public LayerMask terrain; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor in the center
        Cursor.visible = false; // Hide the cursor
    }

    public void Update()
    {
        CameraRot();
    }

    public void CameraRot()
    {
        x += Input.GetAxis("Mouse Y") * sensitivity * -1;
        y += Input.GetAxis("Mouse X") * sensitivity;

        x = Mathf.Clamp(x, xminmax.x, xminmax.y);

        // Scroll to zoom in and out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * 5f;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(x, y + 180, 0);
        Vector3 despos = target.position - rotation * Vector3.forward * distance;

        // use raycast method to detect terrain height
        // When collide with the terrain, player won't be able to view under terrain
        if (Physics.Raycast(target.position, despos - target.position, out RaycastHit hit, distance, terrain))
        {
            despos = hit.point + hit.normal * 0.7f;
        }

        transform.eulerAngles = new Vector3(x, y + 180, 0);
        transform.position = despos;
    }
}