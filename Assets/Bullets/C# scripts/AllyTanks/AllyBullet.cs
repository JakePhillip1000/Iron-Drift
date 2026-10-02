using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBullet : MonoBehaviour
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
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);

            float damage = Random.Range(minDamage, maxDamage);

            HealthBarDecrease healthScript = other.GetComponent<HealthBarDecrease>();
            if (healthScript != null)
            {
                healthScript.TakeDamage(damage);
            }
        }
    }
}