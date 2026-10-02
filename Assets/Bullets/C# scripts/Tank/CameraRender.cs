using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRender : MonoBehaviour
{
    [SerializeField] private LayerMask renderLayer;  
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float cullingAngle = 90f; 

    private Camera cameraComponent;

    void Start()
    {
        // Get the Camera component
        cameraComponent = GetComponent<Camera>();

        if (cameraComponent == null)
        {
            Debug.LogError("No Camera found");
        }
    }

    void Update()
    {
        Rendering();
    }

    private void Rendering()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance, renderLayer);

        foreach (Collider collider in colliders)
        {
            Vector3 directionToObject = collider.transform.position - transform.position;
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, renderLayer))
            {
                float dotProduct = Vector3.Dot(transform.forward, directionToObject.normalized);

                if (dotProduct > Mathf.Cos(cullingAngle * Mathf.Deg2Rad))
                {
                    collider.gameObject.SetActive(true); 
                }
                else
                {
                    collider.gameObject.SetActive(false); 
                }
            }
            else
            {
                collider.gameObject.SetActive(false); 
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}