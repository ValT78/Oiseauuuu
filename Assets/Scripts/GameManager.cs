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

    [Header("Compute coast")]
    public float coastOffset;
    public float coastPerCube;
    public float variance;

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
                if (blockHeight > towerHeigth)
                    towerHeigth = blockHeight;
            }
        }
        return towerHeigth + blockSpawnOffset;
    }

    public void UpdateRessources()
    {
        this.population = 0;
        this.food = 0;
        foreach (BlockGenerator block in blockList)
        {
            if (block.isPlaced)
            {
                block.ProduceRessouces();
            }
        }
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
