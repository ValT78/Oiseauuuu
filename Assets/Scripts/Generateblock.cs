using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public GameObject cubePrefab; // Assigne le prefab du cube dans l'inspecteur

    public void GenerateBlock(int numberOfCubes)
    {
        GameObject blockParent = new("BlockParent");
        _ = blockParent.AddComponent<Rigidbody2D>();
        CompositeCollider2D compositeCollider = blockParent.AddComponent<CompositeCollider2D>();
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;

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

    private void Start()
    {
        GenerateBlock(GameManager.Instance.numberOfCubesInBlock);
    }
}
