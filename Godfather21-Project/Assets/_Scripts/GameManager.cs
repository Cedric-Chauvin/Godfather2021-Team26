using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return _instance; } }
    static GameManager _instance;
    [SerializeField] GameObject pauseMenu;
    Player player0;
    Player player1;
    public bool gamePaused = false;

    private void Start()
    {
        _instance = this;
        player0 = Rewired.ReInput.players.GetPlayer(0);
        player1 = Rewired.ReInput.players.GetPlayer(1);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" && player0.GetButtonDown("Pause") || player1.GetButtonDown("Pause"))
        {
            if (!gamePaused)
                PauseGame();
            else
                UnPauseGame();
        }
    }


    public void AddScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void LoadScene(string sceneName)
    {

        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }
     
    public void PauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }
    
    public void UnPauseGame()
    {
        gamePaused = false;
        Time.timeScale =1;
        pauseMenu.SetActive(false);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
