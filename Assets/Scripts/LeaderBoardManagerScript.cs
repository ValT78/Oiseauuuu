using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardManagerScript : MonoBehaviour
{
    public static LeaderBoardManagerScript Instance { get; private set; }
    // Start is called before the first frame update


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


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
