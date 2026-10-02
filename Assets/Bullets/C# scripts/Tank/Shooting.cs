using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shooting : MonoBehaviour
{
    public GameObject trailPrefab;
    public Transform shootingPoint;
    public float trailSpeed = 600f;
    public float firingError = 0.1f;
    public float trailLifetime = 5f;
    public ParticleSystem shootEffect;
    public float reloadTime = 3.5f;
    public AudioSource ShootSFX;

    public TextMeshProUGUI reloadText;
    private HealthBarDecrease healthScript;

    private bool canShoot = false; 

    void Start()
    {
        healthScript = GetComponent<HealthBarDecrease>();
        StartCoroutine(Reload()); // Start the game with full reload time
    }

    void Update()
    {
        if (healthScript != null && healthScript.currentHealth <= 0)
        {
            canShoot = false; // (Disabled the shooting when the player is dead)
            return;
        }

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            ShootTrail();
            StartCoroutine(Reload());
        }
    }

    void ShootTrail()
    {
        if (trailPrefab != null && shootingPoint != null)
        {
            GameObject trail = Instantiate(trailPrefab, shootingPoint.position, shootingPoint.rotation);

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
                if (hit.collider.CompareTag("Enemy"))
                {
                    Bullet bullet = trail.GetComponent<Bullet>();

                    if (bullet != null)
                    {
                        float damage = Random.Range(bullet.minDamage, bullet.maxDamage);
                        HealthBarDecrease healthScript = hit.collider.GetComponent<HealthBarDecrease>();
                        if (healthScript != null)
                        {
                            healthScript.TakeDamage(damage);
                        }
                    }
                }
            }

            if (ShootSFX != null)
            {
                ShootSFX.Play();
            }

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

        if (reloadText != null)
        {
            reloadText.color = new Color(0.8f, 0f, 0f); // (r, g, b)
        }

        float timer = reloadTime;
        while (timer > 0)
        {
            if (reloadText != null)
            {
                reloadText.text = $"{timer:F2}s";
            }
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }

        if (reloadText != null)
        {
            reloadText.color = Color.green; 
            reloadText.text = $"{reloadTime:F2}s";  // show full reload time, but green after fully reload
        }

        if (healthScript != null && healthScript.currentHealth <= 0)
        {
            canShoot = false;
        }

        else
        {
            canShoot = true;
        }
    }
}