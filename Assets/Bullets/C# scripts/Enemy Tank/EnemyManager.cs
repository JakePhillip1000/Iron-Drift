using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    /*
     For this script, I write it myself first and it seems like the code does not work (trying), 
     so I decided to use the chatbot to fix my code for me
     */
    public static EnemyManager Instance;

    public List<GameObject> enemies = new List<GameObject>();
    public TextMeshProUGUI enemyCountText;
    public GameObject victoryUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateEnemyCount();
    }

    public void RegisterEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        UpdateEnemyCount();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            UpdateEnemyCount();
        }
    }

    private void UpdateEnemyCount()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = enemies.Count.ToString();
        }

        if (enemies.Count == 0)
        {
            ShowVictoryScreen();
        }
    }

    private void ShowVictoryScreen()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
        }

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