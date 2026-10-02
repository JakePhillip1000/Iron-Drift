using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float time = 300f; 
    public TMP_Text timeTxt; 
    private bool Paused = false;

    void Start()
    {
        TimerTxtUp();
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1f);
            if (Paused)
            {
                continue;
            }

            time--;
            TimerTxtUp();
        }

        PauseGame();
    }

    void TimerTxtUp()
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeTxt.text = $"{minutes:D2}:{seconds:D2}";
    }

    void PauseGame()
    {
        Paused = true;
        Time.timeScale = 0; 
        timeTxt.text = "00:00";
        Debug.Log("Game ended");
    }
}