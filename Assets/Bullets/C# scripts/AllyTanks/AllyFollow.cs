using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyFollow : MonoBehaviour
{
    public float detectionRadius = 1000f;  
    public float shootZoneRadius = 35f;  
    public float stoppingDistance = 10f; 
    public float turretRotationSpeed = 2f; 
    public Transform turret; 
    public Transform shootingPoint;  

    private NavMeshAgent agent;
    [HideInInspector] public Transform currentTarget;
    private AllyShoot shootScript;
    private AllyRandom randomWalk;

    [HideInInspector] public bool ISWALK = false;
    [HideInInspector] public bool targetIsDead = false;

    private AllyHealthbar allyHealth;
    private HealthBarDecrease enemyHealth;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootScript = GetComponent<AllyShoot>();
        randomWalk = GetComponent<AllyRandom>();

        allyHealth = GetComponent<AllyHealthbar>();
        if (allyHealth == null)
        {
            Debug.Log($"Ally heathbar not found on {gameObject.name}");
        }

        enemyHealth = GetComponent<HealthBarDecrease>();
        if (enemyHealth == null)
        {
            Debug.Log($"Enemy healthbar not found on {gameObject.name}");
        }
    }

    void Update()
    {
        if (allyHealth != null && allyHealth.currentHealth <= 0)
        {
            StopEnemy();
            return;
        }

        DetectTarget();

        if (!ISWALK)
        {
            BaysianAggressive();
            MoveToTarget();
            RotateTurret();
        }
    }

    public void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius); 
        float closestDistance = Mathf.Infinity;
        currentTarget = null;
        targetIsDead = false;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                HealthBarDecrease healthScript = col.GetComponent<HealthBarDecrease>();
                if (healthScript != null && healthScript.currentHealth > 0)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, col.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        currentTarget = col.transform;
                    }
                }
            }
        }

        if (currentTarget == null)
        {
            targetIsDead = true;
            StartRandomWalk();
            shootScript.enabled = false;
        }
        else
        {
            ISWALK = false;
            StopRandomWalk();

            if (Vector3.Distance(transform.position, currentTarget.position) <= shootZoneRadius)
            {
                shootScript.enabled = true;  
            }
            else
            {
                shootScript.enabled = false;  
            }
        }
    }

    public void BaysianAggressive()
    {
        if (enemyHealth != null)
        {
            float enemyHealthProb = Mathf.InverseLerp(0, enemyHealth.maxHealth, enemyHealth.currentHealth);  // 0 - 1 scale
            float allyHealthProb = Mathf.InverseLerp(0, allyHealth.maxHealth, allyHealth.currentHealth);  // 0 - 1 scale

            float aggression = allyHealthProb; // High ally health, high aggression
            float escape = 1 - enemyHealthProb; // Low enemy health, high escape

            if (enemyHealthProb < 0.3f)
            {
                aggression = 1.0f;
                shootScript.enabled = true;
            }
            else if (allyHealthProb < 0.3f)
            {
                aggression = 0.0f;
                shootScript.enabled = false;

                randomWalk.SetRandomDestination();  // Start random walk if enemy is weak
            }
            else
            {
                aggression = Mathf.Clamp(aggression, 0.5f, 1.0f);
                shootScript.enabled = true;
            }

            if (aggression == 0.0f && currentTarget != null)
            {
                RotateTurret();
            }
        }
    }

    public void MoveToTarget()
    {
        if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.position) > stoppingDistance)
            {
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                agent.ResetPath();
            }
        }
    }

    public void RotateTurret()
    {
        if (currentTarget != null)
        {
            Vector3 directionToTarget = currentTarget.position - turret.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            turret.rotation = Quaternion.Slerp(turret.rotation, targetRotation, Time.deltaTime * turretRotationSpeed);
        }
    }

    public void StartRandomWalk()
    {
        if (!ISWALK && randomWalk != null)
        {
            ISWALK = true;
            randomWalk.enabled = true;
            agent.isStopped = false;
            randomWalk.SetRandomDestination();
        }
    }

    public void StopRandomWalk()
    {
        if (randomWalk != null)
        {
            ISWALK = false;
            randomWalk.enabled = false;
        }
    }

    public void StopEnemy()
    {
        agent.isStopped = true;
        shootScript.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue; 
        Gizmos.DrawWireSphere(transform.position, shootZoneRadius);
    }
}