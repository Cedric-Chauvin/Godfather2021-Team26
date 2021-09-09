using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   
    public void AddScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void LoadScene(string sceneName)
    {

        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
