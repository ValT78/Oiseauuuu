using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheat : MonoBehaviour
{
    [HideInInspector] public bool isTrampled;
    [SerializeField] SpriteRenderer spriteRenderer;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        isTrampled = true;
        spriteRenderer.enabled = false;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        isTrampled = false;
        spriteRenderer.enabled = true;
    }
}
