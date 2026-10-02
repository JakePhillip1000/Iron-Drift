using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TankSpawner : MonoBehaviour
{
    public List<GameObject> EnemyPref;  
    public int maxEnemies = 5;
    public float SpRad = 10f;
    public float minDistance = 5f;
    public Transform playerTransform;
    public TextMeshProUGUI tankCountText;
    private AllyHealthbar allyhealth;
    public int tankCount = 1; // (the reason that I use 1 because I include the player)  
    public GameObject defeatUI; // The defeated UI
    private List<Vector3> lsSpawnPos = new List<Vector3>();

    void Start() // Spawn the tank at the start
    {
        allyhealth = GetComponent<AllyHealthbar>();
        StartCoroutine(SpawnTanksCoroutine());
    }

    public IEnumerator SpawnTanksCoroutine()
    {
        int counter = 0;

        while (counter < maxEnemies)
        {
            Vector3 SpawnPOS;
            int attempts = 0;
            bool validPosition = false;

            do
            {
                SpawnPOS = GetRandomPosition();
                validPosition = CheckDis(SpawnPOS) && ValidSpawnLoc(SpawnPOS);
                attempts++;
            }
            while (!validPosition && attempts < 5);

            if (validPosition)
            {
                GameObject tankPrefab = EnemyPref[Random.Range(0, EnemyPref.Count)];
                GameObject enemy = Instantiate(tankPrefab, SpawnPOS, Quaternion.identity);
                lsSpawnPos.Add(SpawnPOS);
                CheckComp(enemy);
                FacePlayer(enemy);
                tankCount++;  
                UpdateTankUI(); 
                counter++;
            }
            else
            {
                Debug.LogWarning("Retry spawning");
            }
            yield return null;
        }
    }

    public Vector3 GetRandomPosition()
    {
        return transform.position + new Vector3(
            Random.Range(-SpRad, SpRad), 0f,
            Random.Range(-SpRad, SpRad)
        );
    }

    public bool CheckDis(Vector3 position) 
    {
        foreach (Vector3 spawnedPos in lsSpawnPos)
        {
            if (Vector3.Distance(position, spawnedPos) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    public bool ValidSpawnLoc(Vector3 spawnPosition)
    {
        RaycastHit hit;
        Vector3 rayOrigin = spawnPosition + Vector3.up * 5f;
        Debug.DrawRay(rayOrigin, Vector3.down * 10f, Color.red, 2f);

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10f))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            if (hit.collider.CompareTag("tree") || hit.collider.CompareTag("Player"))
            {
                return false;
            }
        }

        return true;
    }

    public void CheckComp(GameObject tank)
    {
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

    public void FacePlayer(GameObject tank)
    {
        if (playerTransform != null)
        {
            Vector3 dir = playerTransform.position - tank.transform.position;
            dir.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            tank.transform.rotation = lookRotation;
        }
        else
        {
            Debug.LogWarning("Player transform is not assigned.");
        }
    }

    private void UpdateTankUI()
    {
        if (tankCountText != null)
        {
            tankCountText.text = tankCount.ToString();
        }

        if (tankCount == 0)
        {
            ShowDefeatScreen();
        }
    }

    public void AllyDestroyed()
    {
        tankCount--;
        UpdateTankUI();
    }

    private void ShowDefeatScreen()
    {
        defeatUI.SetActive(true);
        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadSceneAsync(0);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}