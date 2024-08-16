using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopCard : MonoBehaviour
{
    [SerializeField] private TMP_Text text_title;
    [SerializeField] private TMP_Text text_wool;
    [SerializeField] private TMP_Text text_wood;
    [SerializeField] private TMP_Text text_compost;
    // Start is called before the first frame update
    void Start()
    {
        SetUpCard("Bla",1,2,3); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpCard(string name, int woolCount, int woodCount, int compostCount)
    {
        text_title.text = name;
        text_wool.text = woolCount.ToString();
        text_wood.text = woodCount.ToString();
        text_compost.text = compostCount.ToString();
    }
}
