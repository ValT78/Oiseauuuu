using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Block Generation")]
    public int numberOfCubesInBlock;
    public float towerHeigth;
    public float blockSpawnOffset;
    public float buildPositionX;

    [Header("Ressources")]
    public int population;
    public int food;
    public int wood;
    public int wool;
    public int compost;




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

}
