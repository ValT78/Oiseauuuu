using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script pour suivre la souris et gérer les clics
public class BlockMouseFollower : MonoBehaviour
{
    private bool isFollowing = false;
    private bool isFalling;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private BlockGenerator blockGenerator;

    [Header("Block Settings")]

    [Header("Block Movement")]
    [SerializeField] private float moveUnit;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float fallSpeedIncreaseFactor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();
        blockGenerator = GetComponent<BlockGenerator>();
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
                rb.isKinematic = true; // Désactiver la physique pour le mouvement manuel
                rb.gravityScale = 0; // Désactiver la gravité
                foreach (var collider in colliders)
                {
                    collider.isTrigger = false; // Désactiver les collisions physiques
                }
                blockGenerator.SetInitialPosition(transform.position);

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

    public bool TryBuyCard()
    {
        if (blockGenerator.woodCost > GameManager.Instance.wood || blockGenerator.woolCost > GameManager.Instance.wool || blockGenerator.compostCost > GameManager.Instance.compost)
        {
            Debug.Log("Not enough resources");
            return false;
        }
        else
        {
            GameManager.Instance.wood -= blockGenerator.woodCost;
            GameManager.Instance.wool -= blockGenerator.woolCost;
            GameManager.Instance.compost -= blockGenerator.compostCost;
            isFollowing = true;

            // Position de spawn fixe
            transform.position = new Vector2(GameManager.Instance.buildPositionX, GameManager.Instance.GetSPawnBlockHeight());
            return true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling)
        {
            isFalling = false;
            rb.isKinematic = false; // Activer la physique
            rb.gravityScale = 1; // Activer la gravité
            blockGenerator.isPlaced = true;
            transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            ShopManager.Instance.InitializeShop();
            Destroy(this);

        }
    }

    private void RotateBlock(float angle)
    {
        transform.RotateAround(transform.position, Vector3.forward, angle);
        blockGenerator.UpdateSprites();
    }
}

