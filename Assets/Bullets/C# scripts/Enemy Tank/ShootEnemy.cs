using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootEnemy : MonoBehaviour
{
    public GameObject trailPrefab;
    public Transform shootingPoint;
    public Transform turret; 
    public float trailSpeed = 600f;
    public float firingError = 0.1f;
    public float trailLifetime = 5f;
    public ParticleSystem shootEffect;
    public float reloadTime = 3.5f;
    public AudioSource shootSFX;

    private bool canShoot = true;
    private Transform currentTarget;

    private RandomRide enemyScript;

    private float fireThreshold;

    void Start()
    {
        enemyScript = GetComponent<RandomRide>();
        /*
         randomize between 0-1 (even though the tank finish reload, it wont shoot)
        Determine the aggressiveness of the shooting
         */
        fireThreshold = Random.Range(0.3f, 0.8f); 
    }

    void Update()
    {
        if (enemyScript != null && enemyScript.isDestroyed)
        {
            canShoot = false;
            return;
        }

        FindClosestTarget();
        if (currentTarget != null)
        {
            if (canShoot && AIDecide())
            {
                ShootTrail();
                StartCoroutine(Reload());
            }
        }
    }

    void FindClosestTarget()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("ally");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (GameObject obj in allies)
            EvaluateTarget(obj, ref closestDistance);

        foreach (GameObject obj in players)
            EvaluateTarget(obj, ref closestDistance);
    }

    void EvaluateTarget(GameObject obj, ref float closestDistance)
    {
        float distance = Vector3.Distance(shootingPoint.position, obj.transform.position);
        if (distance < closestDistance)
        {
            closestDistance = distance;
            currentTarget = obj.transform;
        }
    }

    bool AIDecide()
    {
        float decisionValue = Random.Range(0f, 1f);
        return decisionValue > fireThreshold;
    }

    public void ShootTrail()
    {
        if (trailPrefab != null && shootingPoint != null)
        {
            GameObject trail = Instantiate(trailPrefab, shootingPoint.position, shootingPoint.rotation);
            trail.SetActive(true);

            float errorX = Random.Range(-firingError, firingError);
            float errorY = Random.Range(-firingError, firingError);
            Quaternion errorRotation = Quaternion.Euler(errorX, errorY, 0);

            Vector3 shootDirection = errorRotation * shootingPoint.forward; 

            Rigidbody rb = trail.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = shootDirection * trailSpeed;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }

            Destroy(trail, trailLifetime);

            RaycastHit hit;
            if (Physics.Raycast(shootingPoint.position, shootDirection, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("ally"))
                {
                    EnemyBullet bullet = trail.GetComponent<EnemyBullet>();
                    if (bullet != null)
                    {
                        float damage = Random.Range(bullet.minDamage, bullet.maxDamage);
                        AllyHealthbar healthScript = hit.collider.GetComponent<AllyHealthbar>();
                        if (healthScript != null)
                        {
                            healthScript.TakeDamage(damage);
                        }
                    }
                }
            }

            if (shootSFX != null) shootSFX.Play();

            if (shootEffect != null)
            {
                ParticleSystem effect = Instantiate(shootEffect, shootingPoint.position, shootingPoint.rotation);
                effect.transform.SetParent(shootingPoint);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
    }

    IEnumerator Reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        canShoot = true;
    }
}