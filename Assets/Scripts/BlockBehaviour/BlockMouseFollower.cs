using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

// Script pour suivre la souris et g�rer les clics
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
        if (isFalling)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - fallSpeed * Time.deltaTime);
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
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            isFalling = false;
            rb.isKinematic = false; // Activer la physique
            rb.gravityScale = 1; // Activer la gravit�
            rb.velocity = Vector2.zero;
            blockGenerator.isPlaced = true;
            if (blockGenerator.buildingType == BlockGenerator.BuildingType.Wall) blockGenerator.Stick();
            transform.position = new Vector2(Mathf.Round(transform.position.x/moveUnit), Mathf.Round(transform.position.y/moveUnit))*moveUnit;

            ShopManager.Instance.InitializeShop();
            Destroy(this);

        }
    }

    private void RotateBlock(float angle)
    {
        if (GameManager.Instance.isPaused) return;
        transform.RotateAround(transform.position, Vector3.forward, angle);
        blockGenerator.UpdateSprites();
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.isPaused) return;
        if (context.phase != InputActionPhase.Started) return;
        if (!isFalling && !isFollowing) return;
        transform.position = new Vector2(transform.position.x + Mathf.Sign(context.ReadValue<float>()) * moveUnit, transform.position.y);
    }
    
    public void ToggleFastMove(InputAction.CallbackContext context)
    {
        if (!isFalling && !isFollowing) return;
        if (context.phase == InputActionPhase.Started) moveUnit *= 2;
        else if (context.phase == InputActionPhase.Canceled) moveUnit /= 2;
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.isPaused) return;
        if (context.phase != InputActionPhase.Started) return;
        if (!isFollowing) return;
        RotateBlock(Mathf.Sign(context.ReadValue<float>()) * 90);
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.isPaused) return;
        if (context.phase != InputActionPhase.Started) return;
        if (!isFollowing) return;
        isFollowing = false;
        isFalling = true;
        rb.isKinematic = true; // D�sactiver la physique pour le mouvement manuel
        rb.gravityScale = 0; // D�sactiver la gravit�
        foreach (var collider in colliders)
        {
            collider.isTrigger = false; // D�sactiver les collisions physiques
        }
        blockGenerator.SetInitialPosition(transform.position);
    }
    public void ToggleFastDrop(InputAction.CallbackContext context)
    {
        if (!isFalling) return;
        if (context.phase == InputActionPhase.Started) fallSpeed *= fallSpeedIncreaseFactor;
        else if (context.phase == InputActionPhase.Canceled) fallSpeed /= fallSpeedIncreaseFactor;
    }

}

