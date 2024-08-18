using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GreenHouseBlockGenerator : BlockGenerator
{
    // Juste pour ajouter les arbres et la porte
    [SerializeField] private GameObject tree1;
    [SerializeField] private GameObject tree2;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private List<GameObject> detailsList;

    public void AddDetails()
    {
        // Delete all existing details
        foreach (GameObject detail in detailsList)
        {
            Destroy(detail);
        }

        Boolean door_added = false;
        List<Vector2> positions = new List<Vector2>();
        foreach (GameObject cube in cubes)
        {
            positions.Add(cube.transform.position);
        }

        foreach (GameObject cube in cubes)
        {
            Vector2 position = cube.transform.position;
            float tolerance = 0.1f; // Tolérance pour la comparaison des positions

            if (positions.All(p => Vector2.Distance(p, position + Vector2.down) > tolerance))
            {
                

                // Add door with 50% chance, only one door per house
                if (UnityEngine.Random.Range(0, 2) == 0 && !door_added)
                {
                    GameObject door = Instantiate(doorPrefab, position, Quaternion.identity);
                    door.transform.parent = transform;
                    door_added = true;
                    detailsList.Add(door);
                    continue; // On ne peut pas avoir de fenetre et de porte sur le meme cube
                }

            }

            // Add tree1 with 25% chance
            if (UnityEngine.Random.Range(0, 4) == 0)
            {
                GameObject detail = Instantiate(tree1, position, Quaternion.identity);
                detail.transform.parent = transform;
                detailsList.Add(detail);
                continue;
            }

            // Add tree2 with 25% chance
            if (UnityEngine.Random.Range(0, 4) == 0)
            {
                GameObject detail = Instantiate(tree2, position, Quaternion.identity);
                detail.transform.parent = transform;
                detailsList.Add(detail);
                continue;
            }

        }
    }

    public override void UpdateSprites()
    {
        base.UpdateSprites();
        AddDetails();
    }


}