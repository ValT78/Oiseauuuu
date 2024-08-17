using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab; // Le cube qui sert � g�n�rer les blocs

    [Tooltip("Commence par ceux qui ont 1 voisin, pour finir avec celui qui en a 4.\nOn ajoute dans cet ordre : Top, Bottom, Left, Right")]
    [SerializeField] private Sprite[] sprites = new Sprite[15] ; // Tableau de 15 sprites nomm�s

    enum BuildingType
    {
        House = 0,
        CropFields = 1,
        WoolFactory = 2,
        WoodFactory = 3,
        CompostFactory = 4,
        Wall = 5

    }

    private BuildingType buildingType;

    private float angle;
    private int surfaceArea;
    private int numberOfCubesInBlock;
    [HideInInspector] public bool isPlaced;

    [HideInInspector] public int woolCost;
    [HideInInspector] public int woodCost;
    [HideInInspector] public int compostCost;

    private Vector2 initialPosition;
    protected List<GameObject> cubes = new();

    private void Start()
    {
        GameManager.Instance.blockList.Add(this);
        numberOfCubesInBlock = GameManager.Instance.numberOfCubesInBlock;

        woodCost = GetNewPrice();
        woolCost = GetNewPrice();
        compostCost = GetNewPrice();

        GenerateBlock(numberOfCubesInBlock);
    }

    private void Update()
    {
        angle = CalculateAngle(initialPosition);
    }

    float GetRandomFloat(float min, float max)
    {
        System.Random random = new System.Random();
        return (float)(random.NextDouble() * (max - min) + min);
    }
    int GetNewPrice()
    {
        return (int)math.max(0, math.round(numberOfCubesInBlock * GameManager.Instance.coastPerCube * GetRandomFloat(GameManager.Instance.variance, 1 / GameManager.Instance.variance) - GameManager.Instance.coastOffset)); ;
    }
<<<<<<< HEAD

=======
     
>>>>>>> 332dce634bc2f80d3feaf7efb9705f52e3a59cf8
    public virtual void GenerateBlock(int numberOfCubes)
    {
        GameObject blockParent = this.gameObject; // Utiliser l'objet actuel comme parent
        if (!blockParent.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb = blockParent.AddComponent<Rigidbody2D>();
        }
        rb.isKinematic = true; // D�sactiver la physique
        rb.gravityScale = 0; // D�sactiver la gravit�

        if (!blockParent.TryGetComponent<CompositeCollider2D>(out _))
        {
            CompositeCollider2D compositeCollider = blockParent.AddComponent<CompositeCollider2D>();
            compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        }

        List<Vector2> positions = new()
        {
            transform.position
        };

        for (int i = 1; i < numberOfCubes; i++)
        {
            Vector2 newPosition = GetRandomAdjacentPosition(positions);
            positions.Add(newPosition);
        }

        foreach (Vector2 position in positions)
        {
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, blockParent.transform);

            if (!cube.TryGetComponent<PolygonCollider2D>(out _))
            {
                PolygonCollider2D polyCollider = cube.AddComponent<PolygonCollider2D>();
                polyCollider.usedByComposite = true;
                polyCollider.isTrigger = true; // D�sactiver les collisions physiques
            }
            cubes.Add(cube);
        }

        UpdateSprites();
    }

    public void UpdateSprites()
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (GameObject cube in cubes)
        {
            positions.Add(cube.transform.position);
        }

        foreach (GameObject cube in cubes)
        {
            Vector2 position = cube.transform.position;
            int neighborCode = 0;
            float tolerance = 0.1f; // Tol�rance pour la comparaison des positions

            if (positions.Any(p => Vector2.Distance(p, position + Vector2.up) < tolerance))
            {
                neighborCode |= 1;    // Haut
                surfaceArea++;
            }
            if (positions.Any(p => Vector2.Distance(p, position + Vector2.down) < tolerance)) neighborCode |= 2;  // Bas
            if (positions.Any(p => Vector2.Distance(p, position + Vector2.left) < tolerance)) neighborCode |= 4;  // Gauche
            if (positions.Any(p => Vector2.Distance(p, position + Vector2.right) < tolerance)) neighborCode |= 8; // Droite

            if (!cube.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer = cube.AddComponent<SpriteRenderer>();
            }
            spriteRenderer.sprite = sprites[neighborCode - 1]; // Utiliser neighborCode comme index
        }
    }



    private Vector2 GetRandomAdjacentPosition(List<Vector2> existingPositions)
    {
        List<Vector2> possiblePositions = new();

        foreach (Vector2 pos in existingPositions)
        {
            possiblePositions.Add(pos + Vector2.up);
            possiblePositions.Add(pos + Vector2.down);
            possiblePositions.Add(pos + Vector2.left);
            possiblePositions.Add(pos + Vector2.right);
        }

        // Filtrer les positions d�j� occup�es
        possiblePositions.RemoveAll(pos => existingPositions.Contains(pos));

        // Choisir une position al�atoire parmi les positions possibles
        return possiblePositions[Random.Range(0, possiblePositions.Count)];
    }

    private float CalculateAngle(Vector2 initialPosition)
    {
        Vector2 direction = initialPosition - (Vector2)transform.position;
        return Vector2.Angle(Vector2.up, direction);
    }

    public float GetHighestObjectHeight()
    {
        float highestY = float.MinValue;

        foreach (Transform child in transform)
        {
            if (child.position.y > highestY)
            {
                highestY = child.position.y;
            }
        }

        return highestY;
    }

    public void SetInitialPosition(Vector2 position)
    {
        initialPosition = position;
    }

    public void ProduceRessouces()
    {
        switch (buildingType)
        {
            case BuildingType.House:
                GameManager.Instance.population += numberOfCubesInBlock;
                break;
            case BuildingType.CropFields:
                GameManager.Instance.food += surfaceArea;
                break;
            case BuildingType.WoolFactory:
                GameManager.Instance.wool += woolCost;
                break;
            case BuildingType.WoodFactory:
                GameManager.Instance.wood += woodCost;
                break;
            case BuildingType.CompostFactory:
                GameManager.Instance.compost += compostCost;
                break;
            case BuildingType.Wall:
                break;
        }
    }
}
