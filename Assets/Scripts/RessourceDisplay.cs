using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RessourceDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Text text_woolCount;
    [SerializeField] private TMP_Text text_woodCount;
    [SerializeField] private TMP_Text text_compostCount;
    [SerializeField] private TMP_Text text_populationCount;
    [SerializeField] private TMP_Text text_foodCount;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text_woolCount.text =  GameManager.Instance.wool.ToString();
        text_woodCount.text =  GameManager.Instance.wood.ToString();
        text_compostCount.text =  GameManager.Instance.compost.ToString();
        text_populationCount.text =  GameManager.Instance.population.ToString();
        text_foodCount.text =  GameManager.Instance.food.ToString();
    }
}
