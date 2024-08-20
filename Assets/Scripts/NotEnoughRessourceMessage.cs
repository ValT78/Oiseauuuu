using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnoughRessourceMessage : MonoBehaviour
{
    public static NotEnoughRessourceMessage Instance { get; private set; }
    [SerializeField] private float timeBeforeDissapear = 1f;
    private float timer;

    private bool isOnAwake = false;

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
        isOnAwake = true;
        timer = 0;
        
    }

    private void OnEnable()
    {
        timer = isOnAwake ? 0 : timeBeforeDissapear;
        //Debug.Log("Timer set to " + timer);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("IsOnAwake: " + isOnAwake);
        //Debug.Log("Timer: " + timer);
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false);
        }
        isOnAwake = false;

    }
}
