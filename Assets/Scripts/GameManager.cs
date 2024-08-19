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
    [SerializeField] private GameObject heart;
    [SerializeField] private Transform canvas;
    private List<GameObject> heartList = new List<GameObject>();

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
        AudioManager.Instance.PlayWave();
        for(int i = 0; i < maxLife; i++)
        {
            heartList.Add(Instantiate(heart,canvas));
            RectTransform rectTrans = heartList[i].GetComponent<RectTransform>();
            rectTrans.Translate(new Vector3(75*maxLife - i*150,0,0));
        }
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
        float tempWool = 0;
        float tempWood = 0;
        float tempCompost = 0;

        if (totalProduction == 0)
        {
        }
        else if(totalProduction <= feededPopulation)
        {
            wood += woodProduction;
            wool += woolProduction;
            compost += compostProduction;
            tempWool = woodProduction;
            tempWood = woolProduction;
            tempCompost = compostProduction;
        }
        else
        {
            if (totalProduction >= food) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.FOOD, true);
            if (totalProduction >= population) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.POPULATION, true);

            wood += woodProduction * feededPopulation / totalProduction;
            wool += woolProduction * feededPopulation / totalProduction;
            compost += compostProduction * feededPopulation / totalProduction;
            tempWool = woolProduction * feededPopulation / totalProduction;
            tempWood = woodProduction * feededPopulation / totalProduction;
            tempCompost = compostProduction * feededPopulation / totalProduction;
        }

        foreach (BlockGenerator block in blockList)
        {
            if (block.isPlaced)
            {
                switch (block.buildingType)
                {
                    case BlockGenerator.BuildingType.WoodFactory:
                        block.SummonIndicator((int)math.max(0,math.min(block.numberOfCubesInBlock, tempWood)));
                        tempWood -= block.numberOfCubesInBlock;
                        break;
                    case BlockGenerator.BuildingType.WoolFactory:
                        block.SummonIndicator((int)math.max(0,math.min(block.numberOfCubesInBlock, tempWool)));
                        tempWool -= block.numberOfCubesInBlock;
                        break;
                    case BlockGenerator.BuildingType.CompostFactory:
                        block.SummonIndicator((int)math.max(0, math.min(block.numberOfCubesInBlock, tempWool)));
                        tempCompost -= block.numberOfCubesInBlock;
                        break;
                }
            }
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
        PauseMenuScript.Instance.returnToGame();
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
            heartList[life].transform.GetChild(1).gameObject.SetActive(false);
            Camera.main.GetComponent<CamMouvement>().StartShake(0.4f, 0.8f);
            invicibilityTimer = invicibilityTime;
            if (life <= 0)
            {
                GameOver();
            }
        }
    }
    
    public void PublishScore()
    {
        LeaderBoardManagerScript.Instance.PublishScore(GameManager.Instance.population);
    }

    private void GameOver()
    {
        PublishScore();
    }

}
