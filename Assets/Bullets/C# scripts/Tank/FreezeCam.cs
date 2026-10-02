using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCam : MonoBehaviour
{
    private bool isPaused = false;

    public Camera tankCamera;
    public TankCamera tankCamScript;
    public TankAim aimCamScript;

    public KeyCode freezeKey = KeyCode.LeftAlt;

    void Update()
    {
        if (Input.GetKeyDown(freezeKey))
        {
            FreezeCamera();
        }
        else if (Input.GetKeyUp(freezeKey))
        {
            UnfreezeCamera();
        }
    }

    void FreezeCamera()
    {
        if (tankCamScript != null)
        {
            tankCamScript.enabled = false;
        }
        if (aimCamScript != null)
        {
            aimCamScript.enabled = false;
        }
    }

    void UnfreezeCamera()
    {
        if (tankCamScript != null)
        {
            tankCamScript.enabled = true;
        }
        if (aimCamScript != null)
        {
            aimCamScript.enabled = true;
        }
    }
}