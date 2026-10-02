using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 - The enemy will determine the waypoint to ally base
 - enemy determine which point is close to ally base
 - If no waypoint close to base, it will choose random waypoint
 */

// [System.Serializable] // (use this for data storing)
public class Wapoints : MonoBehaviour
{
    public Transform[] waypoints;  // list of waypoints
    public Transform allyBase;
    private NavMeshAgent agent;
    [HideInInspector] public Transform currWP; // the waypoint that the enemy choose to go
    private bool isRandom = false;

    public float waypointRadius = 15f;
    private EnemyFollow enemyFollowScript;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyFollowScript = GetComponent<EnemyFollow>(); // get the component from enemy follow

        ChoosePoint();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, allyBase.position) < waypointRadius) // when enemy is near ally base
        {
            IncreaseDetZone();
        }
        else
        {
            ResetDetZone();
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isRandom)
        {
            ChoosePoint();
        }
    }

    void ChoosePoint()
    {
        if (waypoints.Length == 0)
        {
            return;
        }

        List<Transform> chooseWay = new List<Transform>();

        foreach (Transform wp in waypoints)
        {
            if (Vector3.Distance(wp.position, allyBase.position) < Vector3.Distance(transform.position, allyBase.position))
            {
                chooseWay.Add(wp);
            }
        }

        if (chooseWay.Count > 0)
        {
            currWP = chooseWay[Random.Range(0, chooseWay.Count)];
        }
        else
        {
            currWP = waypoints[Random.Range(0, waypoints.Length)];
        }

        agent.SetDestination(currWP.position);
    }

    public void IncreaseDetZone() // increase detection zone (when the at the ally base)
    {
        if (enemyFollowScript != null)
        {
            enemyFollowScript.detectionRadius = enemyFollowScript.detectionRadius * 3;
        }
    }

    public void ResetDetZone() // reset teh detection zone to normal
    {
        if (enemyFollowScript != null)
        {
            enemyFollowScript.detectionRadius = enemyFollowScript.detectionRadius * 1;
        }
    }
}