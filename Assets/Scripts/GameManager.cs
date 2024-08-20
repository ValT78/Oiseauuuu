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

    [HideInInspector] public int totalBlockPlaced;
    public int population { 
        get { return _population; } 
        set {
            if (value > recordMaxPopulation) recordMaxPopulation = value;
            _population = value;
        } 
    }


    [Header("Compute coast")]
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
        totalBlockPlaced++;
        numberOfCubesInBlock = 4 + (int)math.floor(totalBlockPlaced / 10);
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
        int actualWorkers = population==0 ? 0 : (population - 1) / 5 + 1;
        feededPopulation = math.min(actualWorkers, food);
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
            compost += compostProduction;
            wool += woolProduction;
            tempWood = woodProduction;
            tempCompost = compostProduction;
            tempWool = woolProduction;
        }
        else
        {
            if (totalProduction >= food) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.FOOD, true);
            if (totalProduction >= population) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.POPULATION, true);

            while(feededPopulation > 0)
            {
                if(woodProduction > 0)
                {
                    wood += 1;
                    tempWood += 1;
                    woodProduction -= 1;
                    feededPopulation--;
                }
                if (compostProduction > 0 && feededPopulation > 0)
                {
                    compost += 1;
                    tempCompost += 1;
                    compostProduction -= 1;
                    feededPopulation--;
                }
                if (woolProduction > 0 && feededPopulation > 0)
                {
                    wool += 1;
                    tempWool += 1;
                    woolProduction -= 1;
                    feededPopulation--;
                }
                
            }
        }

        foreach (BlockGenerator block in blockList)
        {
            if (block.isPlaced)
            {
                switch (block.buildingType)
                {
                    case BlockGenerator.BuildingType.WoodFactory:
                        if(tempWood > 0)
                        {
                            block.SummonIndicator(1);
                            tempWood -= 1;
                        }
                        break;
                    case BlockGenerator.BuildingType.WoolFactory:
                        if (tempWool > 0)
                        {
                            block.SummonIndicator(1);
                            tempWool -= 1;
                        }
                        break;
                    case BlockGenerator.BuildingType.Composter:
                        if (tempCompost > 0)
                        {
                            block.SummonIndicator(1);
                            tempCompost -= 1;
                        }
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
