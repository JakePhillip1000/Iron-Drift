using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameCountDown : MonoBehaviour
{
    public TextMeshProUGUI countdownText; 
    public AudioSource coundownSFX;  

    void Start()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        countdownText.gameObject.SetActive(true);
        Time.timeScale = 0;

        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            coundownSFX.Play();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}