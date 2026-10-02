using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour // inheritance from the main class
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 60f;
    public float stationaryRotationSpeed = 45f;
    public float acceleration = 2f;
    public float deceleration = 2f;

    public Transform turret;
    public float turretRotationSpeed = 5f;
    public float turretPitchSpeed = 2f;

    public Camera tankCamera;
    public AudioSource acceleratingSound;
    public AudioSource stationarySound;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private float currentRotation = 0f;

    private Vector3 targetTurretDirection;
    private float targetTurretPitch;

    [SerializeField] private TankPointer tankpoint;
    [SerializeField] private AllyHealthbar allyHealthbar;

    public bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (tankCamera == null)
        {
            tankCamera = Camera.main;
        }

        targetTurretDirection = turret.forward;
        targetTurretPitch = turret.localEulerAngles.x;

        acceleratingSound.loop = false;
        acceleratingSound.Stop();
        stationarySound.loop = false;
        stationarySound.Stop();

        Cursor.visible = false;
    }

    void Update()
    {
        if (allyHealthbar != null && allyHealthbar.currentHealth <= 0)
        {
            isDead = true;
        }

        if (!isDead)
        {
            HandleInput();
        }

        UpdateTurretTarget();
        UpdateAudio();

        if (tankpoint != null)
        {
            Vector3 aimPosition = AimPoint();
            tankpoint.UpdateAimPoint(aimPosition);
        }

        ShowCursor();
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            MoveTank();
            RotateTank();
            RotateTurret();
        }
    }

    void ShowCursor()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void HandleInput()
    {
        if (isDead)
        {
            return;
        }

        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (verticalInput > 0)
        {
            targetSpeed = moveSpeed;
        }
        else if (verticalInput < 0)
        {
            targetSpeed = -moveSpeed;
        }
        else
        {
            targetSpeed = 0;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, (targetSpeed == 0 ? deceleration : acceleration) * Time.deltaTime);

        if (currentSpeed != 0)
        {
            if (currentSpeed < 0)
            {
                if (horizontalInput > 0)
                    currentRotation = rotationSpeed * Time.deltaTime;
                else if (horizontalInput < 0)
                    currentRotation = -rotationSpeed * Time.deltaTime;
                else
                    currentRotation = 0;
            }
            else
            {
                currentRotation = horizontalInput * rotationSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (horizontalInput > 0)
                currentRotation = stationaryRotationSpeed * Time.deltaTime;
            else if (horizontalInput < 0)
                currentRotation = -stationaryRotationSpeed * Time.deltaTime;
            else
                currentRotation = 0f;
        }
    }

    private void UpdateAudio()
    {
        if (currentSpeed != 0)
        {
            if (!acceleratingSound.isPlaying)
            {
                acceleratingSound.Play();
                acceleratingSound.loop = true; 
            }

            if (stationarySound.isPlaying)
            {
                stationarySound.Stop();
            }
        }
        else
        {
            if (acceleratingSound.isPlaying)
            {
                acceleratingSound.Stop();
            }

            if (!stationarySound.isPlaying)
            {
                stationarySound.PlayOneShot(stationarySound.clip); 
            }
        }
    }

    private void MoveTank()
    {
        if (isDead)
        {
            return;
        }

        Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void RotateTank()
    {
        Quaternion rotation = Quaternion.Euler(0, currentRotation, 0);
        rb.MoveRotation(rb.rotation * rotation);
    }

    public void UpdateTurretTarget()
    {
        if (turret == null || tankCamera == null)
        {
            return;
        }

        Ray ray = tankCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 directionToMouse = hit.point - turret.position;
            directionToMouse.y = 0;

            targetTurretDirection = directionToMouse.normalized;

            float mouseY = Input.GetAxis("Mouse Y");
            targetTurretPitch -= mouseY * turretPitchSpeed;
            targetTurretPitch = Mathf.Clamp(targetTurretPitch, -17f, 17f);
        }
    }

    public void RotateTurret()
    {
        if (turret == null)
        {
            return;
        }

        Vector3 currentDirection = turret.forward;
        Vector3 newDirection = Vector3.RotateTowards(currentDirection, targetTurretDirection, turretRotationSpeed * Time.deltaTime, 0.0f);
        Quaternion horizontalRotation = Quaternion.LookRotation(newDirection);

        Vector3 localEuler = horizontalRotation.eulerAngles;
        localEuler.x = targetTurretPitch;
        turret.rotation = Quaternion.Euler(localEuler);
    }

    private void OnCollisionEnter(Collision collision) // Built in functions (OnCollisionEnter)
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        currentSpeed = 0f;
    }

    public void TurrRotCam() // This method will be use in another class, not this class
    {
        UpdateTurretTarget();
        RotateTurret();
    }

    public Vector3 AimPoint()
    {
        Ray ray = new Ray(turret.position, turret.forward); // Ray from the turret in its forward direction
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point; 
        }
        else
        {
            return ray.GetPoint(1f); 
        }
    }
}

