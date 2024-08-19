using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab; // Le cube qui sert � g�n�rer les blocs
    [SerializeField] private Rigidbody2D rb; // Le cube qui sert � g�n�rer les blocs

    [Tooltip("Commence par ceux qui ont 1 voisin, pour finir avec celui qui en a 4.\nOn ajoute dans cet ordre : Top, Bottom, Left, Right")]
    [SerializeField] private Sprite[] sprites = new Sprite[15] ; // Tableau de 15 sprites nomm�s

    public enum BuildingType
    {
        House = 0,
        CropFields = 1,
        WoolFactory = 2,
        WoodFactory = 3,
        CompostFactory = 4,
        Wall = 5,
        Dirt = 6
    }

    public BuildingType buildingType;
    public bool canBeDestroyed = true;

    private float angle;
    private int surfaceArea;
    private int trampledWheat;
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

        Vector3Int prices = new Vector3Int();
        switch (buildingType)
        {
            case BuildingType.House:
                prices = GetNewPrices(1, 3, 1);
                break;
            case BuildingType.CropFields:
                prices = GetNewPrices(1, 1, 3);
                break;
            case BuildingType.WoolFactory:
                prices = GetNewPrices(3, 1, 1);
                break;
            case BuildingType.WoodFactory:
                prices = GetNewPrices(3, 1, 1);
                break;
            case BuildingType.CompostFactory:
                prices = GetNewPrices(3, 1, 1);
                break;
            case BuildingType.Wall:
                prices = GetNewPrices(1, 1, 1);
                break;
        }   
        woodCost = prices.x;
        woolCost = prices.y;
        compostCost = prices.z;

        GenerateBlock(numberOfCubesInBlock);
    }



    private void Update()
    {
        if(transform.position.y < -20 && canBeDestroyed)
        {
            GameManager.Instance.LoseLife(this);
            Destroy(gameObject);
        }
    }

    float GetRandomFloat(float min, float max)
    {
        System.Random random = new();
        return (float)(random.NextDouble() * (max - min) + min);
    }
    public Vector3Int GetNewPrices(int weightWood, int weightWool, int weightCompost)
    {
        int total = (int)math.max(0, math.round((numberOfCubesInBlock * GameManager.Instance.coastPerCube - GameManager.Instance.coastOffset) * GetRandomFloat(GameManager.Instance.variance, 1 / GameManager.Instance.variance))); ;
        int totalWeight = weightWood + weightWool + weightCompost;

        // Calculer les parts proportionnelles
        float part1 = (float)weightWood / totalWeight;
        float part2 = (float)weightWool / totalWeight;
        float part3 = (float)weightCompost / totalWeight;

        // Générer des valeurs aléatoires basées sur les parts
        int value1 = Mathf.RoundToInt(total * part1 * Random.Range(1- GameManager.Instance.repartitionVariance, 1+ GameManager.Instance.repartitionVariance));
        int value2 = Mathf.RoundToInt(total * part2 * Random.Range(1 - GameManager.Instance.repartitionVariance, 1 + GameManager.Instance.repartitionVariance));
        int value3 = total - value1 - value2;

        // Ajuster les valeurs pour s'assurer que la somme est égale au total
        int sum = value1 + value2 + value3;

        // Si la somme n'est pas égale au total, ajuster la dernière valeur
        if (sum != total)
        {
            value3 += total - sum;
        }
        return new Vector3Int(value1, value2, value3);
    }

    public virtual void GenerateBlock(int numberOfCubes)
    {
        GameObject blockParent = this.gameObject; // Utiliser l'objet actuel comme parent
        if (!blockParent.TryGetComponent(out Rigidbody2D rb))
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

    public virtual void UpdateSprites()
    {
        List<Vector2> localPositions = new();
        foreach (GameObject cube in cubes)
        {
            localPositions.Add(cube.transform.position);
            // Tourne les petits cubes pour qu'ils aient toujours la tête en haut (-rotation du parent)
            // Rotation toujours un multiple de 90 degres, Unity est bizzare donc on doit arrondir
            cube.transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(-transform.rotation.z/90)*90);

        }

        foreach (GameObject cube in cubes)
        {
            Vector2 localPosition = cube.transform.position;
            int neighborCode = 0;
            float tolerance = 0.1f; // Tolérance pour la comparaison des positions


            if (localPositions.Any(p => Vector2.Distance(p, localPosition +  (Vector2) cube.transform.up) < tolerance))
            {
                neighborCode |= 1;    // Haut
            }
            else
            {
                surfaceArea++;
            }
            if (localPositions.Any(p => Vector2.Distance(p, localPosition + (Vector2) (-cube.transform.up)) < tolerance)) neighborCode |= 2;  // Bas
            if (localPositions.Any(p => Vector2.Distance(p, localPosition + (Vector2) (-cube.transform.right)) < tolerance)) neighborCode |= 4;  // Gauche
            if (localPositions.Any(p => Vector2.Distance(p, localPosition + (Vector2) (cube.transform.right)) < tolerance)) neighborCode |= 8; // Droite

            if (!cube.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer = cube.AddComponent<SpriteRenderer>();
            }
            if(sprites.Length > neighborCode-1)
                spriteRenderer.sprite = sprites[neighborCode - 1];
            else
            {
                spriteRenderer.sprite = sprites[14];
            }
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
                foreach (Transform child in transform)
                {
                    // Appel récursif pour les enfants de l'enfant actuel
                    if (child.childCount > 0 && child.GetChild(0).TryGetComponent(out Wheat wheatScript))
                    {
                        if (wheatScript.isTrampled)
                        {
                            trampledWheat++;
                        }
                    }
                }
                GameManager.Instance.food += (surfaceArea-trampledWheat)*3;
                break;
            case BuildingType.WoolFactory:
                GameManager.Instance.woolProduction += numberOfCubesInBlock;
                break;
            case BuildingType.WoodFactory:
                GameManager.Instance.woodProduction += numberOfCubesInBlock;
                break;
            case BuildingType.CompostFactory:
                GameManager.Instance.compostProduction += numberOfCubesInBlock;
                break;
            case BuildingType.Wall:
                break;
        }
    }

    public void ToleranceMovement()
    {
        if (isPlaced && rb.velocity.magnitude == 0)
        {
            Vector3 position = transform.position;
            float distanceToHalfX = Mathf.Abs(position.x - Mathf.Round(position.x * 2) / 2);
            float distanceToHalfY = Mathf.Abs(position.y - Mathf.Round(position.y * 2) / 2);

            if (distanceToHalfX < 0.05f)
            {
                position.x = Mathf.Round(position.x * 2) / 2;
            }

            if (distanceToHalfY < 0.05f)
            {
                position.y = Mathf.Round(position.y * 2) / 2;
            }

            transform.position = position;
        }
    }
    public void Stick()
    {
        foreach (GameObject cube in cubes)
        {
            Collider2D[] neighbours = Physics2D.OverlapCircleAll(cube.transform.position, 1);
            foreach (Collider2D neighbour in neighbours)
            {
                if (!neighbour.transform.parent) continue ;
                if (!neighbour.transform.parent.TryGetComponent<BlockGenerator>(out _)) continue ;
                GameObject parent = neighbour.transform.parent.gameObject;
                if (!parent.TryGetComponent<Rigidbody2D>(out Rigidbody2D component)) continue;
                var joint = gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = component;
            }
        }
    }

    public void GetPlaced()
    {
        StartCoroutine(GetPlacedCoroutine());
    }

    private IEnumerator GetPlacedCoroutine()
    {
        Camera.main.GetComponent<CamMouvement>().StartShake(0.2f, 0.7f);
        yield return new WaitForSeconds(0.2f);
        Camera.main.GetComponent<CamMouvement>().StartZoom(5, transform.position, 0.3f, 0.5f, true);
        yield return new WaitForSeconds(1.1f);
        GameManager.Instance.UpdateRessources();
        ShopManager.Instance.InitializeShop();
    }   
}
