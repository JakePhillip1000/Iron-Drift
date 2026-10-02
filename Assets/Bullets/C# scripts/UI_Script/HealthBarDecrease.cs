using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarDecrease : MonoBehaviour
{
    public Image healthBarFill;
    public Image easeHealth;
    public TextMeshProUGUI healthText;
    public float maxHealth = 800f;
    public float currentHealth;
    private float targetHealth;

    [SerializeField] float lerpSpeed = 5f;
    private bool takeDmg = false;

    // Audio and Effects
    public AudioSource explosionSound1;
    public AudioSource smokeSound;
    public GameObject explosionEffect;
    public GameObject smokeEffect;

    private bool isDestroyed = false;

    private void Start()
    {
        // Initialize health values
        currentHealth = maxHealth;
        targetHealth = currentHealth;
        UpdateHealth();
    }

    private void Update()
    {
        if (easeHealth != null && takeDmg && !isDestroyed)
        {
            // Start easing
            easeHealth.fillAmount = Mathf.Lerp(easeHealth.fillAmount, targetHealth / maxHealth, lerpSpeed * Time.deltaTime);

            // Stop easing  (the yellow bar still remain)
            if (Mathf.Abs(easeHealth.fillAmount - targetHealth / maxHealth) < 0.001f)
            {
                takeDmg = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                float damage = Random.Range(bullet.minDamage, bullet.maxDamage);
                TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        targetHealth = currentHealth;
        takeDmg = true;

        if (healthBarFill != null)
        {
            UpdateHealth();
        }

        if (currentHealth <= 0)
        {
            HandleDestruction();
        }
    }

    private void HandleDestruction()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;

            EnemyManager.Instance.RemoveEnemy(gameObject); // decrement enemy number

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; 
                rb.velocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero; 
            }

            PlayExplosionSound();

            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                explosion.transform.SetParent(transform);
            }

            if (smokeEffect != null)
            {
                GameObject smoke = Instantiate(smokeEffect, transform.position, Quaternion.identity);
                smoke.transform.SetParent(transform);

                ParticleSystem smokeParticles = smoke.GetComponent<ParticleSystem>();
                if (smokeParticles != null)
                {
                    var main = smokeParticles.main;
                    main.loop = true;
                    smokeParticles.Play();
                }
            }

            if (smokeSound != null && !smokeSound.isPlaying)
            {
                smokeSound.loop = true;
                smokeSound.Play();
            }
            if (healthBarFill != null && easeHealth != null && healthText != null)
            {
                Destroy(healthBarFill.transform.parent.gameObject);
            }
        }
    }

    private void PlayExplosionSound()
    {
        if (explosionSound1 != null && !explosionSound1.isPlaying)
        {
            AudioSource explosionSound = explosionSound1;

            if (explosionSound != null)
            {
                explosionSound.PlayOneShot(explosionSound.clip);
            }
        }
    }

    public void UpdateHealth()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth:F0}/{maxHealth}";
        }
    }
}