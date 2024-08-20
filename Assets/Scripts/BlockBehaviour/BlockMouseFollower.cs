using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;

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

    private bool isPlaced = false;
    private Coroutine moveCoroutine;
    [SerializeField] private LayerMask layerMaskToHit;
    [SerializeField] private LayerMask layerMaskNoToHit;

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
        if (blockGenerator.woodCost > GameManager.Instance.wood) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.WOOD, false);
        if (blockGenerator.woolCost > GameManager.Instance.wool) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.WOOL, false);
        if (blockGenerator.compostCost > GameManager.Instance.compost) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.COMPOSTE, false);

        if (blockGenerator.woodCost > GameManager.Instance.wood || blockGenerator.woolCost > GameManager.Instance.wool || blockGenerator.compostCost > GameManager.Instance.compost)
        {
            if (blockGenerator.woodCost > GameManager.Instance.wood) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.WOOD, true);
            if (blockGenerator.woolCost > GameManager.Instance.wool) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.WOOL, true);
            if (blockGenerator.compostCost > GameManager.Instance.compost) RessourceDisplay.Instance.ToggleWarning(RessourceDisplay.RessourceType.COMPOSTE, true);
            
            Debug.Log("Not enough resources");
            NotEnoughRessourceMessage.Instance.gameObject.SetActive(true);
            return false;
        }
        else
        {
            GameManager.Instance.wood -= blockGenerator.woodCost;
            GameManager.Instance.wool -= blockGenerator.woolCost;
            GameManager.Instance.compost -= blockGenerator.compostCost;
            isFollowing = true;

            // Position de spawn fixe
            return true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling && !isPlaced)
        {
            if(collision.gameObject.TryGetComponent<BlockGenerator>(out _))
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.transform.position = new Vector2(Mathf.Round(collision.transform.position.x * 2), Mathf.Round(collision.transform.position.y *2)) / 2;
            }
            foreach (var collider in colliders)
            {
                collider.points = new Vector2[] {
                    new(-0.5f, 0.5f),
                    new(0.5f, 0.5f),
                    new(0.5f, -0.5f),
                    new(-0.5f, -0.5f)
                };
                collider.gameObject.layer = layerMaskToHit;
            }
            isPlaced = true;
            AudioManager.Instance.PlayPlacement();
            isFalling = false;
            rb.isKinematic = false; // Activer la physique
            rb.gravityScale = 1; // Activer la gravit�
            rb.velocity = Vector2.zero;
            blockGenerator.isPlaced = true;

            Vector2 temp_pos = new Vector2(Mathf.Round(transform.position.x * 2), Mathf.Round(transform.position.y * 2)) / 2;

            if (blockGenerator.buildingType == BlockGenerator.BuildingType.GlueBlock)
            {
                

                foreach (Transform child in transform)
                {
                    Vector2 childPosition = child.position;
                    RaycastHit2D hitR = Physics2D.Raycast(childPosition, Vector2.right, currentMoveUnit * 1.2f, layerMaskToHit);
                    RaycastHit2D hitL = Physics2D.Raycast(childPosition, Vector2.left, currentMoveUnit * 1.2f, layerMaskToHit);

                    if (hitR.collider != null && hitR.collider.gameObject.layer != layerMaskNoToHit)
                    {
                        temp_pos = new Vector2(hitR.point.x - 0.5f, Mathf.Round(transform.position.y * 2)/2);
                        break;
                        
                    }
                    else if (hitL.collider != null && hitL.collider.gameObject.layer != layerMaskNoToHit)
                    {
                        temp_pos = new Vector2(hitL.point.x + 0.5f, Mathf.Round(transform.position.y * 2) / 2);
                        break;
                    }
                    
                }


                blockGenerator.Stick();

            }
            else
            {
                rb.constraints = RigidbodyConstraints2D.None;
            }
            transform.position = temp_pos;
            

        blockGenerator.GetPlaced();
            Destroy(this);
        }
    }

    private void RotateBlock(float angle)
    {
        if (GameManager.Instance.isPaused) return;
        transform.RotateAround(transform.position, -Vector3.forward, angle);
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
        Vector2 currentPosition = transform.position;
        float direction = Mathf.Sign(context.ReadValue<float>());

        float moveDistance = currentMoveUnit;
        


        foreach (Transform child in transform)
        {
            Vector2 childPosition = child.position;
            Vector2 rayOrigin = new Vector2(childPosition.x + 0.5f * direction, childPosition.y);
            // Effectuer un Raycast pour vérifier les collisions
            RaycastHit2D hit = Physics2D.Raycast(childPosition, Vector2.right * direction, currentMoveUnit, layerMaskToHit); 

            if (hit.collider != null && hit.collider.gameObject.layer != layerMaskNoToHit)
            {
                // Si un collider est détecté, ajuster la distance de déplacement
                moveDistance = Mathf.Min(moveDistance, hit.distance);
            }
        }

        // Déplacer le bloc de la distance calculée
        transform.position = new Vector2(currentPosition.x + moveDistance * direction, currentPosition.y);
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
        rb.isKinematic = true; // Activer la physique
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        rb.gravityScale = 0;
        foreach (var collider in colliders)
        {
            collider.isTrigger = false;
        }
        blockGenerator.SetInitialPosition(transform.position);
        HelperCanvasScript.Instance.MovingExplanation();
    }
    public void ToggleFastDrop(InputAction.CallbackContext context)
    {
        if (!isFalling) return;
        if (context.phase == InputActionPhase.Started) fallSpeed *= fallSpeedIncreaseFactor;
        else if (context.phase == InputActionPhase.Canceled) fallSpeed = baseFallSpeed;
    }

}

