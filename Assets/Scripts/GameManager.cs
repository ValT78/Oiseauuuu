using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<BlockGenerator> blockList = new();

    public List<ShopCard> shopCards = new();

    [Header("LifeCounter")]
    [HideInInspector] public int life;
    public int maxLife;
    public float invicibilityTime;
    private float invicibilityTimer;

    [Header("Block Generation")]
    public int numberOfCubesInBlock;
    public float baseHeigth;
    public float blockSpawnOffset;
    public float buildPositionX;

    [Header("Ressources")]
    public int wood;
    public int wool;
    public int compost;
    [HideInInspector] public int woodProduction;
    [HideInInspector] public int woolProduction;
    [HideInInspector] public int compostProduction;
    [HideInInspector] public int population;
    [HideInInspector] public int food;
    [HideInInspector] public int feededPopulation;

    [Header("Compute coast")]
    public float coastOffset;
    public float coastPerCube;
    public float variance;
    public float repartitionVariance;

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

    private void Start()
    {
        life = maxLife;
        invicibilityTimer = 0;
        PauseMenu.SetActive(false);
        RessourceDisplay.Instance.RessourceUpdate(wool, wood, compost, population, food);
    }

    private void Update()
    {
        invicibilityTimer -= Time.deltaTime;
    }

    public float GetSpawnBlockHeight()
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
        this.woodProduction = 0;
        this.woolProduction = 0;
        this.compostProduction = 0;
        foreach (BlockGenerator block in blockList)
        {
            if (block.isPlaced)
            {
                block.ProduceRessouces();
            }
        }
        feededPopulation = math.min(population, food);
        int totalProduction = woodProduction + woolProduction + compostProduction;

        RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.FOOD, false);
        RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.POPULATION, false);
        if (totalProduction == 0)
        {
        }
        else if(totalProduction <= feededPopulation)
        {
            wood += woodProduction;
            wool += woolProduction;
            compost += compostProduction;
        }
        else
        {
            if (totalProduction >= food) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.FOOD, true);
            if (totalProduction >= population) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.POPULATION, true);

            wood += (int)(woodProduction * feededPopulation / totalProduction);
            wool += (int)(woolProduction * feededPopulation / totalProduction);
            compost += (int)(compostProduction * feededPopulation / totalProduction);
        }

        RessourceDisplay.Instance.RessourceUpdate(wool, wood, compost, population, food);

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

    public void LoseLife(BlockGenerator blockGenerator)
    {
        blockList.Remove(blockGenerator);
        if (invicibilityTimer <= 0)
        {
            life--;
            invicibilityTimer = invicibilityTime;
            if (life <= 0)
            {
                GameOver();
            }
        }
    }   

    private void GameOver()
    {

    }

}
