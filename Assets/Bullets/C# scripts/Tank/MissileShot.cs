using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShot : MonoBehaviour // the missile will follow mouse position (Simple to sheridan missle in WOT)
{
    public GameObject trailPrefab;
    public Transform shootingPoint;
    public float trailSpeed = 20f;
    public float firingError = 0.25f;
    public float trailLifetime = 2f;
    public ParticleSystem shootEffect;
    public float reloadTime = 3.5f;

    private bool canShoot = true;

    void Update()
    {
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
            }
            trail.transform.SetParent(shootingPoint); // Make the missile move with the mouse and not left behind

            Destroy(trail, trailLifetime);

            if (shootEffect != null)
            {
                ParticleSystem effect = Instantiate(shootEffect, shootingPoint.position, shootingPoint.rotation);
                effect.transform.SetParent(shootingPoint); // Make the particles system not left behind
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
    }

    IEnumerator Reload() // realoading, cannot shooot
    {
        canShoot = false;

        yield return new WaitForSeconds(reloadTime);

        canShoot = true;
    }
}