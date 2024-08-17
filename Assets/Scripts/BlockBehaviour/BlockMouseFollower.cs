using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script pour suivre la souris et g�rer les clics
public class BlockMouseFollower : MonoBehaviour
{
    private bool isFollowing = false;
    private bool isFalling;
    private int clickCount = 0;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private BlockGenerator blockGenerator;

    [Header("Block Settings")]
    private float blockHeigth;

    [Header("Block Movement")]
    [SerializeField] private float moveUnit;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float fallSpeedIncreaseFactor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();
        blockGenerator = GetComponent<BlockGenerator>();
        blockHeigth = GameManager.Instance.towerHeigth + GameManager.Instance.blockSpawnOffset;
    }

    void Update()
    {
        if(isFollowing || isFalling)
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                moveUnit *= 2;
            }
            else if(Input.GetKeyUp(KeyCode.Z))
            {
                moveUnit /= 2;
            }
            if(Input.GetKeyDown(KeyCode.Q))
            {
                transform.position = new Vector2(transform.position.x - moveUnit, transform.position.y);
            }
            else if(Input.GetKeyDown(KeyCode.D))
            {
                transform.position = new Vector2(transform.position.x + moveUnit, transform.position.y);
            }
        }
        if (isFollowing)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                RotateBlock(90);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateBlock(-90);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                isFollowing = false;
                isFalling = true;
                rb.isKinematic = true; // D�sactiver la physique pour le mouvement manuel
                rb.gravityScale = 0; // D�sactiver la gravit�
                foreach (var collider in colliders)
                {
                    collider.isTrigger = true; // D�sactiver les collisions physiques
                }
            }
        }
        if (isFalling)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - fallSpeed * Time.deltaTime);

            if(Input.GetKeyDown(KeyCode.S))
            {
                fallSpeed *= fallSpeedIncreaseFactor;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                fallSpeed /= fallSpeedIncreaseFactor;
            }
        }
    }

    void OnMouseDown()
    {
        if (clickCount == 0)
        {
            if (blockGenerator.woodCost > GameManager.Instance.wood || blockGenerator.woolCost > GameManager.Instance.wool || blockGenerator.compostCost > GameManager.Instance.compost)
            {
                Debug.Log("Not enough resources");
                return;
            }
            else
            {
                GameManager.Instance.wood -= blockGenerator.woodCost;
                GameManager.Instance.wool -= blockGenerator.woolCost;
                GameManager.Instance.compost -= blockGenerator.compostCost;
                clickCount++;
                isFollowing = true;

                // Position de spawn fixe
                transform.position = new Vector2(GameManager.Instance.buildPositionX, blockHeigth);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling)
        {
            isFalling = false;
            rb.isKinematic = false; // Activer la physique
            rb.gravityScale = 1; // Activer la gravit�
            foreach (var collider in colliders)
            {
                collider.isTrigger = false; // Activer les collisions physiques
            }
            transform.position = new Vector2(transform.position.x, Mathf.Round(transform.position.y) + moveUnit);
        }
    }

    private void RotateBlock(float angle)
    {
        transform.RotateAround(transform.position, Vector3.forward, angle);
        blockGenerator.UpdateSprites();
    }
}

