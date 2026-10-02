using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomRide : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 targetDestination;
    private HealthBarDecrease healthScript;

    public float walkRadius = 5f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    [HideInInspector] public bool isDestroyed = false;

    public GameObject tankHead;
    public GameObject headPrefab;
    public float explosionForce = 5000f;

    public float rayDis = 3f; 
    public float colliderRad = 5f; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        healthScript = GetComponent<HealthBarDecrease>();
        SetRandomDestination();
    }

    void Update()
    {
        if (healthScript != null && healthScript.currentHealth <= 0 && !isDestroyed)
        {
            StopMoving();
            isDestroyed = true;
            ExplodeHead();
            return;
        }

        if (CheckCollision())
        {
            SetRandomDestination();
        }

        if (!agent.pathPending && agent.remainingDistance < 0.4f)
        {
            SetRandomDestination();
        }
    }

    public bool CheckCollision()
    {
        RaycastHit rays;
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, forward, out rays, rayDis))
        {
            if (rays.collider != null)
            {
                if (rays.collider.CompareTag("Player")) // ignore player collision
                {
                    return false;
                }

                if (rays.collider.GetComponent<BoxCollider>() != null || rays.collider.GetComponent<MeshCollider>() != null)
                {
                    if (Vector3.Distance(transform.position, rays.collider.transform.position) < colliderRad)
                    {
                        return true; 
                    }
                }
            }
        }
        return false; 
    }

    public void SetRandomDestination()
    {
        if (healthScript != null && healthScript.currentHealth <= 0)
            return;

        // Check if there is a waypoint assign on the enemy
        Wapoints waypointScript = GetComponent<Wapoints>();
        if (waypointScript != null && waypointScript.currWP != null)
        {
            targetDestination = waypointScript.currWP.position;
            targetDestination.y = transform.position.y;  
            agent.SetDestination(targetDestination);
            agent.speed = Random.Range(minSpeed, maxSpeed); 
        }
        else // When the enemy has not assign the waypoint
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += transform.position;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, walkRadius, NavMesh.AllAreas))
            {
                targetDestination = navHit.position;
                targetDestination.y = transform.position.y;
                agent.SetDestination(targetDestination);
                agent.speed = Random.Range(minSpeed, maxSpeed);
            }
        }
    }

    public void StopMoving()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void ExplodeHead()
    {
        if (tankHead != null && headPrefab != null)
        {
            tankHead.SetActive(false);
            GameObject newHead = Instantiate(headPrefab, tankHead.transform.position, tankHead.transform.rotation);

            Rigidbody headRb = newHead.GetComponent<Rigidbody>();
            if (headRb != null)
            {
                headRb.useGravity = true;
                headRb.isKinematic = false;

                headRb.AddForce(Vector3.up * 100000f, ForceMode.Impulse);
                headRb.AddExplosionForce(explosionForce, newHead.transform.position, 10f, 0f, ForceMode.Impulse);

                headRb.drag = 0f;
                headRb.angularDrag = 0f;
                headRb.mass = 0.1f;
            }

            BoxCollider box = newHead.GetComponent<BoxCollider>();
            if (box == null)
            {
                box = newHead.AddComponent<BoxCollider>();
            }
            newHead.SetActive(true);
        }
    }
}