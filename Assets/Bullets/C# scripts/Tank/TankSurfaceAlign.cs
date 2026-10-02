using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankSurfaceAlign : MonoBehaviour
{
    public Transform tankBody;
    public float raycastLength = 5f;
    public float raycastSpread = 2f;
    public float rotationSmoothing = 10f;
    public float positionSmoothing = 5f;
    public float additionalGravity = 20f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        Surface(); 
    }

    public void Surface()
    {
        Vector3 averageNormal = Vector3.zero;
        int hitCount = 0;

        Vector3[] rayOffsets = {
            Vector3.zero,
            Vector3.forward * raycastSpread, Vector3.back * raycastSpread,
            Vector3.left * raycastSpread, Vector3.right * raycastSpread,
            (Vector3.forward + Vector3.left) * raycastSpread,
            (Vector3.forward + Vector3.right) * raycastSpread,
            (Vector3.back + Vector3.left) * raycastSpread,
            (Vector3.back + Vector3.right) * raycastSpread
        };

        foreach (var offset in rayOffsets)
        {
            Vector3 rayOrigin = transform.position + offset;
            Ray ray = new Ray(rayOrigin, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength))
            {
                averageNormal += hit.normal;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            averageNormal /= hitCount;
            averageNormal.Normalize();

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;
            tankBody.rotation = Quaternion.Slerp(tankBody.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
        }
        rb.AddForce(Vector3.down * additionalGravity, ForceMode.Acceleration);
    }
}