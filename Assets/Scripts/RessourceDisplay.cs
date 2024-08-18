using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RessourceDisplay : MonoBehaviour
{
    public enum RessourceType
    {
        WOOL,
        WOOD,
        COMPOSTE,
        POPULATION,
        FOOD
    }

    public static RessourceDisplay Instance { get; private set; }

    // Start is called before the first frame update
    [SerializeField] private TMP_Text text_woolCount;
    [SerializeField] private TMP_Text text_woodCount;
    [SerializeField] private TMP_Text text_compostCount;
    [SerializeField] private TMP_Text text_populationCount;
    [SerializeField] private TMP_Text text_foodCount;

    [SerializeField] private GameObject img_woolWarning;
    [SerializeField] private GameObject img_woodWarning;
    [SerializeField] private GameObject img_compostWarning;
    [SerializeField] private GameObject img_populationWarning;
    [SerializeField] private GameObject img_foodWarning;

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

    public void RessourceUpdate(int wool, int wood, int compost, int population, int food)
    {
        text_woolCount.text = wool.ToString();
        text_woodCount.text = wood.ToString();
        text_compostCount.text = compost.ToString();
        text_populationCount.text = population.ToString();
        text_foodCount.text = food.ToString();
    }

    [System.Obsolete]
    public void ToggleWarning(RessourceType type)
    {
        switch (type)
        {
            case RessourceType.WOOL:
                img_woolWarning.SetActive(!img_woolWarning.active);
                break;
            case RessourceType.WOOD:
                img_woodWarning.SetActive(!img_woodWarning.active);
                break;
            case RessourceType.COMPOSTE:
                img_compostWarning.SetActive(!img_compostWarning.active);
                break;
            case RessourceType.POPULATION:
                img_populationWarning.SetActive(!img_populationWarning.active);
                break;
            case RessourceType.FOOD:
                img_foodWarning.SetActive(!img_foodWarning.active);
                break;
        }
    }
}
