using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject control;
    [SerializeField] private GameObject explaination;
    [SerializeField] private string sceneName = "Main Game";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void returnToMenu()
    {
        control.SetActive(false);
        explaination.SetActive(false);
        main.SetActive(true);
    }

    public void ToExplaination()
    {
        control.SetActive(false);
        main.SetActive(false);
        explaination.SetActive(true);
    }
    public void ToControl()
    {
        main.SetActive(false);
        explaination.SetActive(false);
        control.SetActive(true);
    }

    public void StartGame()
    {
        // Leaderboard canvas is set to inactive, we don't need to see it when we start the game
        // Will be reactivated when we go back to the main menu, in the QuitGame function of PauseMenuScript
        LeaderBoardManagerScript.Instance.leaderBoardCanvas.gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
