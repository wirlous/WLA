using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class ExitController : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite closeSprite;

    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = closeSprite;
        boxCollider2D.enabled = false;
    }

    internal void Open()
    {
        spriteRenderer.sprite = openSprite;
        boxCollider2D.enabled = true;
    }
}
