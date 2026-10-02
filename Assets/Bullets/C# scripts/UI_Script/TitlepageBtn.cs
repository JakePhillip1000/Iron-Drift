using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitlepageBtn : MonoBehaviour
{
    [SerializeField] public GameObject loadingPanel;
    public GameObject menuPanel;
    public Image loadingBar;
    public TextMeshProUGUI percentageText;
    public AudioSource soundtrack;
    public AudioSource battleMusic; 

    private void Start()
    {
        if (soundtrack != null)
        {
            if (!soundtrack.isPlaying)
            {
                soundtrack.Play();
                soundtrack.loop = true;
            }
        }
    }

    public void PlayGame()
    {
        AudioListener.pause = false;
        menuPanel.SetActive(false);
        loadingPanel.SetActive(true);

        if (soundtrack != null)
        {
            soundtrack.Stop(); 
        }

        if (battleMusic != null)
        {
            battleMusic.Play(); 
            battleMusic.loop = true;
        }

        StartCoroutine(LoadPanel());
        Debug.Log("Starting the game");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            Debug.Log("Exiting playmode in Unity");
        #else
            Application.Quit();
            Debug.Log("Exit the game");
        #endif
    }

    IEnumerator LoadPanel()
    {
        yield return null;
        StartCoroutine(LoadGameScene(1));
        Debug.Log("Starting the game");
    }

    IEnumerator LoadGameScene(int sceneIndex)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadOperation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (!loadOperation.isDone)
        {
            float targetProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);

            while (fakeProgress < targetProgress)
            {
                fakeProgress += Time.deltaTime * 0.2f;
                loadingBar.fillAmount = fakeProgress;
                percentageText.text = (fakeProgress * 100f).ToString("0") + "%";
                yield return null;
            }

            if (loadOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        StartCoroutine(GameAudStart(3f));
    }

    IEnumerator GameAudStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioListener.pause = false;
    }
}