using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotate : MonoBehaviour
{
    public Transform cam;
    public Transform cam2;

    void LateUpdate()
    {
        transform.LookAt(cam.position + cam.forward);
        transform.LookAt(cam2.position + cam2.forward);
    }
}
