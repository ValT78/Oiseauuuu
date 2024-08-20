using System;
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
    [SerializeField] private float distanceBetweenHearts;
    private List<GameObject> heartList = new List<GameObject>();

    [Header("Block Generation")]
    public int numberOfCubesInBlock;
    public float baseHeigth;
    public float blockSpawnOffset;
    public float buildPositionX;



    [Header("Ressources")]
    public int _wood;
    public int wood { get { return _wood;  } set { _wood = value > 0 ? value : 0; } }
    public int _wool;
    public int wool { get { return _wool; } set { _wool = value > 0 ? value : 0; } }

    public int _compost;
    public int compost { get { return _compost; } set { _compost = value > 0 ? value : 0; } }
    [HideInInspector] public int woodProduction;
    [HideInInspector] public int woolProduction;
    [HideInInspector] public int compostProduction;
    // food peut etre negatif, j'y touche pas jsp
    [HideInInspector] public int food;
    [HideInInspector] public int feededPopulation;

    // Servira comme score
    [HideInInspector] public int recordMaxPopulation;
    // Custom setter pour le recordMaxPopulation
    [HideInInspector] public int _population;
    public int population { 
        get { return _population; } 
        set {
            if (value > recordMaxPopulation) recordMaxPopulation = value;
            _population = value;
        } 
    }


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
            rectTrans.Translate(new Vector3(0.5f*distanceBetweenHearts * maxLife - i * distanceBetweenHearts, 0,0));
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
            tempWool = woolProduction;
            tempWood = woodProduction;
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
                        int production = (int)math.max(0, math.min(block.numberOfCubesInBlock, tempWood));
                        //print("Wood production: " + production + " numberOfCubes " + block.numberOfCubesInBlock + "  tempWood " + tempWood);
                        block.SummonIndicator((int)math.max(0,math.min(block.numberOfCubesInBlock, tempWood)));
                        tempWood -= block.numberOfCubesInBlock;
                        break;
                    case BlockGenerator.BuildingType.WoolFactory:
                        int productionWool = (int)math.max(0, math.min(block.numberOfCubesInBlock, tempWood));
                        //print("Wool production: " + productionWool + " numberOfCubes " + block.numberOfCubesInBlock + "  tempWool " + tempWool);
                        block.SummonIndicator((int)math.max(0,math.min(block.numberOfCubesInBlock, tempWool)));
                        tempWool -= block.numberOfCubesInBlock;
                        break;
                    case BlockGenerator.BuildingType.Composter:
                        int productionCompost = (int)math.max(0, math.min(block.numberOfCubesInBlock, tempCompost));
                        //print("Compost production: " + productionCompost + " numberOfCubes " + block.numberOfCubesInBlock + "  tempCompost " + tempCompost);
                        block.SummonIndicator((int)math.max(0, math.min(block.numberOfCubesInBlock, tempCompost)));
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
            ShopManager.Instance.InitializeShop();
            if (life <= 0)
            {
                GameOver();
            }
        }
    }
    
    public void PublishScore()
    {
        LeaderBoardManagerScript.Instance.PublishScore(recordMaxPopulation);
    }

    private void GameOver()
    {
        PublishScore();
        PauseMenuScript.Instance.QuitGame();
    }

}
