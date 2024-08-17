using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab; // Le cube qui sert à générer les blocs

    [Tooltip("Commence par ceux qui ont 1 voisin, pour finir avec celui qui en a 4.\nOn ajoute dans cet ordre : Top, Bottom, Left, Right")]
    [SerializeField] private Sprite[] sprites = new Sprite[15] ; // Tableau de 15 sprites nommés


    private float angle;
    private int surfaceArea;
    private int numberOfCubesInBlock;

    public int woolCost;
    public int woodCost;
    public int compostCost;

    private Vector2 initialPosition;
    private List<GameObject> cubes = new();

    private void Start()
    {
        numberOfCubesInBlock = GameManager.Instance.numberOfCubesInBlock;
        GenerateBlock(numberOfCubesInBlock);
    }

    private void Update()
    {
        angle = CalculateAngle(initialPosition);
    }

    public void GenerateBlock(int numberOfCubes)
    {
        GameObject blockParent = this.gameObject; // Utiliser l'objet actuel comme parent
        if (!blockParent.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb = blockParent.AddComponent<Rigidbody2D>();
        }
        rb.isKinematic = true; // Désactiver la physique
        rb.gravityScale = 0; // Désactiver la gravité

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
            PolygonCollider2D polyCollider = cube.AddComponent<PolygonCollider2D>();
            polyCollider.usedByComposite = true;
            polyCollider.isTrigger = true; // Désactiver les collisions physiques

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
            float tolerance = 0.1f; // Tolérance pour la comparaison des positions

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
            print(cube.transform.position);
            print(neighborCode);
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

        // Filtrer les positions déjà occupées
        possiblePositions.RemoveAll(pos => existingPositions.Contains(pos));

        // Choisir une position aléatoire parmi les positions possibles
        return possiblePositions[Random.Range(0, possiblePositions.Count)];
    }

    private float CalculateAngle(Vector2 initialPosition)
    {
        Vector2 direction = initialPosition - (Vector2)transform.position;
        return Vector2.Angle(Vector2.up, direction);
    }

}
