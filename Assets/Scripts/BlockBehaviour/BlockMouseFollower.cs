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
    List<PolygonCollider2D> colliders = new();
    private BlockGenerator blockGenerator;

    [Header("Block Settings")]

    [Header("Block Movement")]
    [SerializeField] private float moveUnit;
    private float currentMoveUnit;
    private float fallSpeed;
    [SerializeField] private float fallSpeedIncreaseFactor;
    [SerializeField] private float baseFallSpeed;
    private float holdDelay = 0.5f; // Temps avant que le mouvement continu commence
    private float repeatRate = 0.1f; // Intervalle entre chaque mouvement continu

    private bool isMoving = false;
    private Coroutine moveCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<PolygonCollider2D>(out var collider))
            {
                colliders.Add(collider);
            }
        }
        blockGenerator = GetComponent<BlockGenerator>();
        fallSpeed = baseFallSpeed;
        currentMoveUnit = moveUnit;
/*        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Cube"), LayerMask.NameToLayer("Cube"), true);
*/    }

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
            transform.position = new Vector2(GameManager.Instance.buildPositionX, GameManager.Instance.GetSpawnBlockHeight());
            return true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling)
        {
            if(collision.gameObject.TryGetComponent<BlockGenerator>(out _))
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.transform.position = new Vector2(Mathf.Round(collision.transform.position.x / currentMoveUnit), Mathf.Round(collision.transform.position.y / currentMoveUnit)) * currentMoveUnit;
            }
            foreach (var collider in colliders)
            {
                collider.points = new Vector2[] {
                    new(-0.5f, 0.5f),
                    new(0.5f, 0.5f),
                    new(0.5f, -0.5f),
                    new(-0.5f, -0.5f)
                };
                /*FixedJoint2D joint = collider.gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = rb;*/
            }
            isFalling = false;
            rb.isKinematic = false; // Activer la physique
            rb.gravityScale = 1; // Activer la gravit�
            rb.velocity = Vector2.zero;
            blockGenerator.isPlaced = true;
            if (blockGenerator.buildingType == BlockGenerator.BuildingType.Wall) blockGenerator.Stick();
            transform.position = new Vector2(Mathf.Round(transform.position.x/ currentMoveUnit), Mathf.Round(transform.position.y/currentMoveUnit))*currentMoveUnit;
            GameManager.Instance.UpdateRessources();
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
        if (!isFalling && !isFollowing) return;

        if (context.phase == InputActionPhase.Started)
        {
            MoveOnce(context);
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(HoldMove(context));
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
        }
    }

    private void MoveOnce(InputAction.CallbackContext context)
    {
        transform.position = new Vector2(transform.position.x + Mathf.Sign(context.ReadValue<float>()) * currentMoveUnit, transform.position.y);
    }

    private IEnumerator HoldMove(InputAction.CallbackContext context)
    {
        yield return new WaitForSeconds(holdDelay);
        while (true)
        {
            MoveOnce(context);
            yield return new WaitForSeconds(repeatRate);
        }
    }

    public void ToggleFastMove(InputAction.CallbackContext context)
    {
        if (!isFalling && !isFollowing) return;
        if (context.phase == InputActionPhase.Started) currentMoveUnit *= 2;
        else if (context.phase == InputActionPhase.Canceled) currentMoveUnit = moveUnit;
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
        else if (context.phase == InputActionPhase.Canceled) fallSpeed = baseFallSpeed;
    }

}

