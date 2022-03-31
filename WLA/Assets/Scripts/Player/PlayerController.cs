﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(SwordController))]
[RequireComponent(typeof(BowController))]
public class PlayerController : MonoBehaviour
{
    // Public
    [Header("Movement")]
    [Range(1f, 20f)] public float moveSpeed = 5f;
    public float moveSpeedUpTime = 2f;
    public float moveSpeedDownTime = 1f;
    public AnimationCurve moveSpeedLerp;

    [Header("Invincibility")]
    [Range(1, 10)] public float invincibilityTime = 2f;

    [Header("Animation")]
    public Animator playerAnimator;
    public Animator atttackAnimator;

    [Header("Debug")]
    [SerializeField] int swordIndex = 0;
    [SerializeField] int bowIndex = 0;
    [SerializeField] int spellIndex = 0;

    // Private
    [SerializeField] Vector2 facingDirection;
    [SerializeField] Vector2 movement;
    [SerializeField] Vector2 knockbackDistance;
    [SerializeField] bool isKnockback;
    [SerializeField] WeaponType weapon = WeaponType.NONE;
    float moveFactor;
    bool isHitable;
    int invincibilityFlashes = 5;

    // Internal
    PlayerInput playerInput;
    Rigidbody2D rb;
    SwordController swordController;
    BowController bowController;
    MagicController magicController;
    HealthSystem health;
    TimeCounter moveTimeCounter;
    SpriteRenderer[] sprites;

    void Awake()
    {
        // Save reference
        GameReferences.player = this;

        // Cache components
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthSystem>();
        swordController = GetComponent<SwordController>();
        bowController = GetComponent<BowController>();
        magicController = GetComponent<MagicController>();

        // Timer for movement
        moveTimeCounter = new TimeCounter(moveSpeedUpTime);

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

        health.SetMinMaxHealth(20, 20);
        health.ResetHealth();

        sprites = GetComponentsInChildren<SpriteRenderer>();

        isHitable = true;
        
        knockbackDistance = Vector2.zero;
        isKnockback = false;

        weapon = GameReferences.inventoryManager.GetNextWeapon();
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
        case WeaponType.BOW:
            atttackAnimator?.SetTrigger("AttackBow");
            break;
        case WeaponType.MAGIC:
            // TODO: Add AttackMagic trigger
            atttackAnimator?.SetTrigger("AttackBow");
            break;
        default:
            break;
        }
    }

    void TogleWeapon()
    {
        weapon = GameReferences.inventoryManager.GetNextWeapon(weapon);
        // switch (weapon)
        // {
        // case WeaponType.SWORD:
        //     weapon = WeaponType.BOW;
        //     break;
        // case WeaponType.BOW:
        //     weapon = WeaponType.MAGIC;
        //     break;
        // case WeaponType.MAGIC:
        //     weapon = WeaponType.SWORD;
        //     break;
        // default:
        //     break;
        // }
    }

    void WeaponUp()
    {
        switch (weapon)
        {
        case WeaponType.SWORD:
            swordIndex++;
            swordController?.ChangeWeapon(ref swordIndex);
            break;
        case WeaponType.BOW:
            bowIndex++;
            bowController?.ChangeWeapon(ref bowIndex);
            break;
        case WeaponType.MAGIC:
            spellIndex++;
            bowController?.ChangeWeapon(ref bowIndex);
            break;
        default:
            break;
        }
    }

    public bool ChangeWeapon(WeaponType weaponType, int index)
    {
        bool hasWeapon = GameReferences.inventoryManager.HasWeapon(weaponType);
        if (!hasWeapon)
            return false;
            
        switch (weaponType)
        {
        case WeaponType.SWORD:
            swordController?.ChangeWeapon(ref index);
            swordIndex = index;
            return true;
        case WeaponType.BOW:
            bowController?.ChangeWeapon(ref index);
            bowIndex = index;
            return true;
        case WeaponType.MAGIC:
            bowController?.ChangeWeapon(ref index);
            spellIndex = index;
            return true;
        default:
            break;
        }
        return false;
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
        moveTimeCounter.Tick(Time.fixedDeltaTime);
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
        case WeaponType.BOW:
            bowController.Attack(dir);
            break;
        case WeaponType.MAGIC:
            magicController.Attack(dir);
            break;
        default:
            Debug.Log("Don't have any weapon");
            break;
        }
    }

    public void EndDamage()
    {
        switch (weapon)
        {
        case WeaponType.SWORD:
            swordController.EndAttack();
            break;
        case WeaponType.BOW:
            break;
        case WeaponType.MAGIC:
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
  