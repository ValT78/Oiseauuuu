using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<BlockGenerator> blockList = new();

    [Header("Block Generation")]
    public int numberOfCubesInBlock;
    public float baseHeigth;
    public float blockSpawnOffset;
    public float buildPositionX;

    [Header("Ressources")]
    public int population;
    public int food;
    public int wood;
    public int wool;
    public int compost;

    [Header("Pause")]
    [SerializeField] private GameObject PauseMenu;
    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetSPawnBlockHeight()
    {
        float towerHeigth =  baseHeigth;
        foreach (BlockGenerator block in blockList)
        {
            if (block.isPlaced)
            {
                float blockHeight = block.GetHighestObjectHeight();
                print(blockHeight);
                if (blockHeight > towerHeigth)
                    towerHeigth = blockHeight;
            }
        }
        return towerHeigth + blockSpawnOffset;
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
        Debug.Log("set Pause true");
    }

    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
        Debug.Log("set Pause false");
    }

    public void SwitchPauseState(InputAction.CallbackContext context)
    {
        //Debug.Log(context.phase);
        
        if(context.phase == InputActionPhase.Started)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
        
    }

}
