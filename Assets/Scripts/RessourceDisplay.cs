using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RessourceDisplay : MonoBehaviour
{
    public static RessourceDisplay Instance { get; private set; }

    // Start is called before the first frame update
    [SerializeField] private TMP_Text text_woolCount;
    [SerializeField] private TMP_Text text_woodCount;
    [SerializeField] private TMP_Text text_compostCount;
    [SerializeField] private TMP_Text text_populationCount;
    [SerializeField] private TMP_Text text_foodCount;

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
}
