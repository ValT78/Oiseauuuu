using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WoolBlockGenerator : BlockGenerator
{
    // Juste pour ajouter les arbres et la porte
    [SerializeField] private GameObject windowSheep;
    [SerializeField] private GameObject windowWool;
    [SerializeField] private GameObject doorPrefab;
    private List<GameObject> detailsList = new List<GameObject>();

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
            float tolerance = 0.1f; // Tol�rance pour la comparaison des positions

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

            // Add sheep with 25% chance
            if (UnityEngine.Random.Range(0, 4) == 0)
            {
                GameObject detail = Instantiate(windowSheep, position, Quaternion.identity);
                detail.transform.parent = transform;
                detailsList.Add(detail);
                continue;
            }

            // Add windowWool with 10% chance
            if (UnityEngine.Random.Range(0, 10) == 0)
            {
                GameObject detail = Instantiate(windowWool, position, Quaternion.identity);
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