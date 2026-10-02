using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    public Camera tankCamera; 
    public TankCamera tankCamScript;
    public TankAim aimCamScript;

    public KeyCode freezeKey = KeyCode.LeftAlt; 

    void Start()
    {
        if (tankCamScript == null)
        {
            tankCamScript = tankCamera.GetComponent<TankCamera>();
        }
        if (aimCamScript == null)
        {
            aimCamScript = tankCamera.GetComponent<TankAim>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (Input.GetKey(freezeKey) || isPaused)
        {
            FreezeCamera();
        }
        else
        {
            UnfreezeCamera();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        FreezeCamera();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (tankCamScript != null)
        {
            tankCamScript.enabled = true;
        }
        if (aimCamScript != null)
        {
            aimCamScript.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Train_Station");
    }

    private void FreezeCamera()
    {
        if (tankCamScript != null)
        {
            tankCamScript.enabled = false;
        }
        if (aimCamScript != null)
        {
            aimCamScript.enabled = false;
        }
    }

    private void UnfreezeCamera()
    {
        if (tankCamScript != null)
        {
            tankCamScript.enabled = true;
        }
        if (aimCamScript != null)
        {
            aimCamScript.enabled = true;
        }
    }
}
