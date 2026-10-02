using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float minDamage = 68f;
    public float maxDamage = 113f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ally"))
        {
            Destroy(gameObject); 

            float damage = Random.Range(minDamage, maxDamage);

            AllyHealthbar healthScript = other.GetComponent<AllyHealthbar>(); 
            if (healthScript != null)
            {
                healthScript.TakeDamage(damage);
            }
        }
    }
}