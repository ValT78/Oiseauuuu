using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject control;
    [SerializeField] private GameObject explaination;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private string mainMenuName = "Main Menu";

    public static PauseMenuScript Instance { get; private set; }

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


    public void ToExplaination()
    {
        Debug.Log("ToExplaination");
        pauseMenu.SetActive(false);
        control.SetActive(false);
        explaination.SetActive(true);
    }
    public void ToControl()
    {
        pauseMenu.SetActive(false);
        explaination.SetActive(false);
        control.SetActive(true);
    }

    public void returnToMenu()
    {
        control.SetActive(false);
        explaination.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void returnToGame()
    {
        control.SetActive(false);
        explaination.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuName);
    }

}
