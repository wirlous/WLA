using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType
{
    HEALTH,
    MANA,
    ARROW,
    KEY,
    BOW,
    SWORD,
    MAGIC
};

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
public class PickUpManager : MonoBehaviour
{
    public PickUpType type;
    public int value;
    public Animator animator;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRender;
    private CircleCollider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;

        spriteRender = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetAnimator();
    }

    void SetAnimator()
    {
        if (animator == null)
            return;

        // animator.SetFloat("value", value);
        switch (type)
        {
        case PickUpType.HEALTH:
            animator.SetTrigger("Health");
            break;
        case PickUpType.MANA:
            animator.SetTrigger("Mana");
            break;
        case PickUpType.ARROW:
            animator.SetTrigger("Arrow");
            break;
        case PickUpType.KEY:
            animator.SetTrigger("Key");
            break;
        case PickUpType.SWORD:
            animator.SetTrigger("Sword");
            break;
        case PickUpType.BOW:
            animator.SetTrigger("Bow");
            break;
        case PickUpType.MAGIC:
            animator.SetTrigger("Magic");
            break;
        default:
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // SetAnimator();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.tag;
        if (tag.Equals("Player"))
        {
            // Debug.LogFormat("Pick up {0} with value {1}", type, value);
            bool gotten = GameReferences.inventoryManager.PickUp(type, value);
            if (gotten)
                Destroy(gameObject);
        }
    }
    
}
