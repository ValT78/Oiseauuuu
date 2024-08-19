using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiantSound : MonoBehaviour
{

    // Play wave is inside start of GameManager.cs

    private float nextSoundTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        nextSoundTimer -= Time.deltaTime;
        if (nextSoundTimer <= 0)
        {
            nextSoundTimer = Random.Range(5, 15);
            PlayRandomnly();
        }
    }


    void PlayRandomnly()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                AudioManager.Instance.PlaySeagullLow();
                break;
            case 1:
                AudioManager.Instance.PlaySeagullMedium();
                break;
            case 2:
                AudioManager.Instance.PlaySeagullHigh();
                break;
        }
    }
}
