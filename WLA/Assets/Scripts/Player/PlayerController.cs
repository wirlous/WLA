using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TimeCounter))]
[RequireComponent(typeof(HealthSystem))]
public class PlayerController : MonoBehaviour
{
    // Public
    [Range(1f, 20f)] public float moveSpeed = 5f;
    public float moveSpeedUpTime = 2f;
    public float moveSpeedDownTime = 1f;
    [Range(1, 10)] public float invincibilityTime = 2f;
    [Range(1, 10)] public int damage = 2;
    public AnimationCurve moveSpeedLerp;
    public Animator playerAnimator;
    public Animator atttackAnimator;

    public int swordIndex = 0;
    public int arcIndex = 0;

    // Private
    [SerializeField] Vector2 facingDirection;
    [SerializeField] Vector2 movement;
    [SerializeField] Vector2 knockbackDistance;
    [SerializeField] bool isKnockback;
    [SerializeField] WeaponType weapon = WeaponType.SWORD;
    float moveFactor;
    bool isHitable;
    int invincibilityFlashes = 5;

    // Internal
    PlayerInput playerInput;
    Rigidbody2D rb;
    SwordController swordController;
    ArcController arcController;
    HealthSystem health;
    TimeCounter moveTimeCounter;
    SpriteRenderer[] sprites;

    void Awake()
    {
        // Save reference
        GameReferences.player = this;

        // Cache components
        rb = GetComponent<Rigidbody2D>();
        moveTimeCounter = GetComponent<TimeCounter>();
        health = GetComponent<HealthSystem>();
        swordController = GetComponent<SwordController>();
        arcController = GetComponent<ArcController>();

        // Player Input
        playerInput = new PlayerInput();
        
        // Register Attack
        playerInput.Gameplay.Attack.performed += ctx => Attack();
        
        // Register Move
        playerInput.Gameplay.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        playerInput.Gameplay.Move.performed += ctx => StartMove();
        playerInput.Gameplay.Move.canceled += ctx => StopMove();

        // Register TogleWeapon
        playerInput.Gameplay.ChangeWeaponType.performed += ctx => TogleWeapon();
        
        // Register WeaponUp
        playerInput.Gameplay.ChangeWeaponUp.performed += ctx => WeaponUp();
        
    }

    void Start()
    {
        facingDirection = new Vector2(0,-1);

        moveTimeCounter.Init(moveSpeedUpTime);

        health.SetMinMaxHealth(20, 20);
        health.ResetHealth();

        sprites = GetComponentsInChildren<SpriteRenderer>();

        isHitable = true;
        
        knockbackDistance = Vector2.zero;
        isKnockback = false;

    }

    void StopMove()
    {
        movement = Vector2.zero;
        moveTimeCounter.SetDown(moveSpeedDownTime);
    }

    void StartMove()
    {
        moveTimeCounter.SetUp(moveSpeedUpTime);
    }

    void Attack()
    {
        switch (weapon)
        {
        case WeaponType.SWORD:
            atttackAnimator?.SetTrigger("AttackSword");
            break;
        case WeaponType.ARC:
            atttackAnimator?.SetTrigger("AttackSword");
            break;
        default:
            break;
        }
    }

    void TogleWeapon()
    {
        switch (weapon)
        {
        case WeaponType.SWORD:
            weapon = WeaponType.ARC;
            break;
        case WeaponType.ARC:
            weapon = WeaponType.SWORD;
            break;
        default:
            break;
        }
    }

    void WeaponUp()
    {
        switch (weapon)
        {
        case WeaponType.SWORD:
            swordIndex++;
            swordController?.ChangeWeapon(swordIndex);
            break;
        case WeaponType.ARC:
            arcIndex++;
            arcController?.ChangeWeapon(arcIndex);
            break;
        default:
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float speed = movement.sqrMagnitude;
        if (speed > 0.2f)
        {
            facingDirection = movement;
        }
        playerAnimator?.SetFloat("Speed", movement.sqrMagnitude);
        playerAnimator?.SetFloat("Horizontal", movement.x);
        playerAnimator?.SetFloat("Vertical", movement.y);
        playerAnimator?.SetFloat("FacingHorizontal", facingDirection.x);
        playerAnimator?.SetFloat("FacingVertical", facingDirection.y);

        atttackAnimator?.SetFloat("FacingHorizontal", facingDirection.x);
        atttackAnimator?.SetFloat("FacingVertical", facingDirection.y);
    }

    void FixedUpdate()
    {
        float t = moveTimeCounter.GetT();
        moveFactor = moveSpeedLerp.Evaluate(t);

        // Movement
        if (isKnockback)
        {
            Vector2 partialKnockback = knockbackDistance.normalized * GameReferences.knockbackSpeed * Time.fixedDeltaTime;
            Vector2 originalKnockback = knockbackDistance;
            rb.MovePosition(rb.position + partialKnockback);
            knockbackDistance -= partialKnockback;
            float dotProduct = Vector2.Dot(knockbackDistance, originalKnockback);
            if (dotProduct < 0.01f)
            {
                knockbackDistance = Vector2.zero;
                isKnockback = false;
            }
        }
        else
        {
            rb.MovePosition(rb.position + movement * moveSpeed * moveFactor * Time.fixedDeltaTime);
        }

    }

    void OnEnable()
    {
        playerInput.Gameplay.Enable();
        health.OnDeath += Die;

    }

    void OnDisable()
    {
        playerInput.Gameplay.Disable();
        health.OnDeath -= Die;
    }

    void Die()
    {
        Debug.Log("Player dies");
    }

    public void DoDamage(WeaponType weaponType, Direction dir)
    {
        switch (weapon)
        {
        case WeaponType.SWORD:
            swordController.Attack(dir);
            break;
        case WeaponType.ARC:
            arcController.Attack(dir);
            break;
        default:
            Debug.Log("Don't have any weapon");
            break;
        }
    }

    public int GetDamage()
    {
        if (weapon == WeaponType.SWORD)
        {
            return swordController.GetDamage();
        }

        return 0;
    }

    public void ReceiveDamage(int damage, Vector3 pOrigin)
    {
        Debug.LogFormat("Player receive damage {0}", damage);
        Knockback2D(Freya.Mathfs.Dir(pOrigin, transform.position));
        health.ReceiveDamage(damage);
        StartCoroutine(FlashSprites(invincibilityFlashes, invincibilityTime/(2*invincibilityFlashes)));
    }


    void Knockback2D(Vector2 pushDir)
    {
        knockbackDistance = pushDir * GameReferences.knockbackFactor;
        isKnockback = true;
    }

    public void EndDamageSword()
    {
        swordController.EndAttack();
    }

    IEnumerator FlashSprites(int numTimes, float delay, bool disable = false)
    {
        isHitable = false;
        for (int loop = 0; loop < numTimes; loop++)
        {
            // Cycle through all sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                if (disable)
                {
                    // For disabling
                    sprites[i].enabled = false;
                }
                else
                {
                    // For changing the alpha
                    sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 0.25f);
                }
            }
 
            // Delay specified amount
            yield return new WaitForSeconds(delay);
 
            // Cycle through all sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                if (disable)
                {
                    // For disabling
                    sprites[i].enabled = true;
                }
                else
                {
                    // For changing the alpha
                    sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 1);
                }
            }
 
            // Delay specified amount
            yield return new WaitForSeconds(delay);
        }
        isHitable = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if ((enemy != null) && isHitable)
        {
            Debug.LogFormat("Receive damage from {0}", enemy.gameObject.name);
            ReceiveDamage(enemy.GetDamage(), enemy.transform.position);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.LogFormat("Collision exit detected: {0}", collision.gameObject.name);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if ((enemy != null) && isHitable)
        {
            Debug.LogFormat("Receive damage from {0}", enemy.gameObject.name);
            ReceiveDamage(enemy.GetDamage(), enemy.transform.position);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
        // enemy?.ReceiveHit(transform.position);

        // HealthSystem otherHealth = other.gameObject.GetComponent<HealthSystem>();
        // otherHealth?.ReceiveDamage(damage);
        
        // Debug.LogFormat("Trigger enter detected: {0}", other.gameObject.name);

    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Debug.LogFormat("Trigger exit detected: {0}", other.gameObject.name);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Debug.LogFormat("Trigger stay detected: {0}", other.gameObject.name);
    }
}
  