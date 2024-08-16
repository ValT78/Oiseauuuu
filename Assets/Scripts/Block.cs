using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab; // Le cube qui sert à générer les blocs

    [SerializeField] private Sprite[] sprites = new Sprite[15]; // Tableau de 15 couleurs


    private float angle;
    private int numberOfCubesInBlock;

    private Vector2 initialPosition;

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
        if (!blockParent.TryGetComponent<Rigidbody2D>(out var rb))
        {
            blockParent.AddComponent<Rigidbody2D>();
        }
        CompositeCollider2D compositeCollider = blockParent.GetComponent<CompositeCollider2D>();
        if (compositeCollider == null)
        {
            compositeCollider = blockParent.AddComponent<CompositeCollider2D>();
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

            // Vérifier les voisins
            int neighborCode = 0;
            if (positions.Contains(position + Vector2.up)) neighborCode |= 1;    // Haut
            if (positions.Contains(position + Vector2.down)) neighborCode |= 2;  // Bas
            if (positions.Contains(position + Vector2.left)) neighborCode |= 4;  // Gauche
            if (positions.Contains(position + Vector2.right)) neighborCode |= 8; // Droite

            // Attribuer la couleur appropriée
            if (!cube.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer = cube.AddComponent<SpriteRenderer>();
            }
            spriteRenderer.sprite = sprites[neighborCode - 1]; // Utiliser neighborCode comme index
        }

    }


    private Vector2 GetRandomAdjacentPosition(List<Vector2> existingPositions)
    {
        List<Vector2> possiblePositions = new List<Vector2>();

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
