using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FarmBlockGenerator : BlockGenerator
{
    // Juste pour ajouter du blé en haut des blocks de farm
    [SerializeField] private GameObject cropsPrefab;

    public void AddCrops()
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (GameObject cube in cubes)
        {
            positions.Add(cube.transform.position);
        }

        foreach (GameObject cube in cubes)
        {
            Vector2 position = cube.transform.position;
            float tolerance = 0.1f; // Tolérance pour la comparaison des positions

            if (positions.All(p => Vector2.Distance(p, position + Vector2.up) > tolerance))
            {
                Vector2 cropsPosition = position + Vector2.up;
                GameObject crops = Instantiate(cropsPrefab, cropsPosition, Quaternion.identity, cube.transform);
            }
            
        }
    }

    /*public void RemoveCrops()
    {
        foreach (GameObject cube in cubes)
        {
            if (cube.tag == "Crops")
            {
                Destroy(cube);
            }
        }
    }*/

    public override void GenerateBlock(int numberOfCubes)
    {
        base.GenerateBlock(numberOfCubes);
        AddCrops();
    }


}