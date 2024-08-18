using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FarmBlockGenerator : BlockGenerator
{
    // Juste pour ajouter du bl� en haut des blocks de farm
    [SerializeField] private GameObject cropsPrefab;
    private List<GameObject> detailsList = new List<GameObject>();

    public void AddDetails()
    {
        // Delete all existing details
        foreach (GameObject detail in detailsList)
        {
            Destroy(detail);
        }

        List<Vector2> positions = new List<Vector2>();
        foreach (GameObject cube in cubes)
        {
            positions.Add(cube.transform.position);
        }

        foreach (GameObject cube in cubes)
        {
            Vector2 position = cube.transform.position;
            float tolerance = 0.1f; // Tol�rance pour la comparaison des positions

            if (positions.All(p => Vector2.Distance(p, position + Vector2.up) > tolerance))
            {
                Vector2 cropsPosition = position + Vector2.up;
                GameObject crops = Instantiate(cropsPrefab, cropsPosition, Quaternion.identity, cube.transform);
                detailsList.Add(crops);
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

    public override void UpdateSprites()
    {
        base.UpdateSprites();
        AddDetails();
    }


}