using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SingleSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public List<GameObject> EnemyPref;
    private EnemyManager enemyManager;

    void Start()
    {
        EnemyManager enemyManager = GetComponent<EnemyManager>();
        if (spawnPoint == null)
        {
            Debug.Log("There is no spawnpoint");
            return;
        }

        Spawning();
    }
    public void Spawning()
    {
        if (EnemyPref.Count == 0)
        {
            Debug.Log("No enemy spawning");
            return;
        }

        int idx = Random.Range(0, EnemyPref.Count);
        GameObject objSpawn = EnemyPref[idx];
        GameObject spawnEne = Instantiate(objSpawn, spawnPoint.position, spawnPoint.rotation);
        EnemyManager.Instance.RegisterEnemy(spawnEne);

        CheckComp(spawnEne);
    }

    public void CheckComp(GameObject tank)
    {
        if (tank == null)
        {
            return;
        }

        tank.SetActive(true);

        if (tank.CompareTag("Player"))
        {
            return;
        }

        Collider col = tank.GetComponent<Collider>();
        if (col == null)
        {
            col = tank.AddComponent<BoxCollider>();
        }
        col.enabled = true;

        Rigidbody rb = tank.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = tank.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }
        rb.isKinematic = false;

        MeshRenderer mesher = tank.GetComponent<MeshRenderer>();
        if (mesher == null)
        {
            Debug.Log("The tank is not rendered");
        }
        else
        {
            mesher.enabled = true;
        }
    }
}