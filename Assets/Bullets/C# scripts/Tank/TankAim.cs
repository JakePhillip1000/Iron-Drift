using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAim : MonoBehaviour
{
    public Camera tankCamera;
    public Camera aimCamera;
    public float zoomSpeed = 10f;
    public float zoomMultiplier = 20f;
    public float maxZoom = 60f;

    private int count = 0;
    private Camera currentCamera;
    private TankController tankController;

    public GameObject hideTank;
    public Transform bullet;

    public GameObject aimPointerUI;
    public GameObject aimPointerUI2;  // This will move based on the gun position
    private TankPointer tankPointer;

    private Renderer[] tankRenderers;
    public Transform gunTransform;

    private Vector3 targetPosition;

    void Start()
    {
        tankCamera.enabled = true;
        aimCamera.enabled = false;
        currentCamera = tankCamera;

        tankController = GetComponent<TankController>();

        tankPointer = GetComponent<TankPointer>();

        tankRenderers = hideTank.GetComponentsInChildren<Renderer>();
        SetTankVisibility(true);

        if (aimPointerUI != null) aimPointerUI.SetActive(false);
        if (aimPointerUI2 != null) aimPointerUI2.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            count++;
            ToggleCameras();
        }

        if (currentCamera == aimCamera)
        {
            HandleZoom();
            AimCamRot();
        }
    }

    void ToggleCameras()
    {
        bool isAiming = count % 2 != 0;

        tankCamera.enabled = !isAiming;
        aimCamera.enabled = isAiming;

        currentCamera = isAiming ? aimCamera : tankCamera;

        SetTankVisibility(!isAiming);

        // Toggle UI pointers when aiming, active when aiming
        if (aimPointerUI != null) aimPointerUI.SetActive(isAiming);
        if (aimPointerUI2 != null) aimPointerUI2.SetActive(isAiming);

        // Hide the pointer when aiming
        if (tankPointer != null)
        {
            if (isAiming)
            {
                tankPointer.HideAimPoint();
            }
            else
            {
                tankPointer.ShowAimPoint();
            }
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newFOV = currentCamera.fieldOfView - scroll * zoomSpeed;
            currentCamera.fieldOfView = Mathf.Clamp(newFOV, zoomMultiplier, maxZoom);
        }
    }

    void AimCamRot()
    {
        tankController.TurrRotCam();
        tankController.UpdateTurretTarget();

        if (aimCamera != null && tankCamera != null)
        {
            aimCamera.transform.rotation = tankCamera.transform.rotation;
        }
    }

    void SetTankVisibility(bool isVisible)
    {
        foreach (Renderer x in tankRenderers)
        {
            if (x.gameObject == bullet.gameObject || x.transform.IsChildOf(bullet))
            {
                continue;
            }

            x.enabled = isVisible;
        }
    }
}