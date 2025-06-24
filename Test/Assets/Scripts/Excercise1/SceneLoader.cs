using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    public Button m_startButton;
    [SerializeField]
    public Slider m_loadingBar;


    private void Start()
    {
        m_startButton.onClick.AddListener(StartNextSceneLoad);
    }

    public void StartNextSceneLoad()
    {
        m_startButton.gameObject.SetActive(false);
        m_loadingBar.gameObject.SetActive(true);
        StartCoroutine(LoadNextScene());
    }

    public IEnumerator LoadNextScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Scenes/Exercise1/JumpingBox", LoadSceneMode.Single);
        loadOperation.allowSceneActivation = true;
        
        while (loadOperation.progress < 1)
        {
            m_loadingBar.value = loadOperation.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
