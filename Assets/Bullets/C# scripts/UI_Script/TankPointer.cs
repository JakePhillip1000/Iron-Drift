using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPointer : MonoBehaviour
{
    public RectTransform aimPointUI;
    public Camera cam;

    public void UpdateAimPoint(Vector3 worldPosition)
    {
        if (aimPointUI != null && cam != null)
        {
            Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);

            if (screenPosition.z > 0)
            {
                aimPointUI.position = screenPosition;
            }
            else
            {
                aimPointUI.position = new Vector3(-1000, -1000, 0);
            }
        }
    }

    public void HideAimPoint()
    {
        if (aimPointUI != null)
        {
            aimPointUI.gameObject.SetActive(false);
        }
    }

    public void ShowAimPoint() 
    {
        if (aimPointUI != null)
        {
            aimPointUI.gameObject.SetActive(true);
        }
    }
}