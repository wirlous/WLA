using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct DirectionBoxCollider2D
{
     public Direction direction;
     public BoxCollider2D boxCollider;
 }


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Timer))]
public class PlayerController : MonoBehaviour
{
    // Public
    [Range(1f, 20f)]
    public float moveSpeed = 5f;
    public float moveSpeedUpTime = 2f;
    public float moveSpeedDownTime = 1f;

    public AnimationCurve moveSpeedLerp;
    public Animator playerAnimator;
    public Animator atttackAnimator;

    public List<DirectionBoxCollider2D> swordCollider = new List<DirectionBoxCollider2D>();
    [SerializeField]
    private Dictionary<Direction, BoxCollider2D> swordColliderDict = new Dictionary<Direction, BoxCollider2D>();
    
    // Private
    [SerializeField]
    Vector2 facingDirection;
    [SerializeField]
    Vector2 movement;
    [SerializeField]
    WeaponType weapon = WeaponType.SWORD;
    float moveFactor;

    // Internal
    PlayerInput playerInput;
    Rigidbody2D rb;
    Timer moveTimer;

    void Awake()
    {
        // Save reference
        GameReferences.player = this;

        // Cache components
        rb = GetComponent<Rigidbody2D>();
        moveTimer = GetComponent<Timer>();

        // Player Input
        playerInput = new PlayerInput();
        
        // Register Attack
        playerInput.Gameplay.Attack.performed += ctx => Attack();
        
        // Register Move
        playerInput.Gameplay.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        playerInput.Gameplay.Move.performed += ctx => StartMove();
        playerInput.Gameplay.Move.canceled += ctx => StopMove();
    }

    void Start()
    {
        facingDirection = new Vector2(0,-1);

        SwordColliderArrayToDictionary();
        DisableAllSwordColliders();

        moveTimer.Init(moveSpeedUpTime);
    }

    void SwordColliderArrayToDictionary()
    {
        foreach (DirectionBoxCollider2D dbc in swordCollider)
        {
            dbc.boxCollider.isTrigger = true;
            swordColliderDict.Add(dbc.direction, dbc.boxCollider);
        }
    }

    void StopMove()
    {
        movement = Vector2.zero;
        moveTimer.SetDown(moveSpeedDownTime);
    }

    void StartMove()
    {
        moveTimer.SetUp(moveSpeedUpTime);
    }

    void Attack()
    {
        if (weapon == WeaponType.SWORD)
            atttackAnimator?.SetTrigger("AttackSword");
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

        float t = moveTimer.GetTimeNormalize();
        moveFactor = moveSpeedLerp.Evaluate(t);

        // Movement
        rb.MovePosition(rb.position + movement * moveSpeed * moveFactor * Time.fixedDeltaTime);

    }

    void OnEnable()
    {
        playerInput.Gameplay.Enable();
    }

    void OnDisable()
    {
        playerInput.Gameplay.Disable();
    }

    public void DoDamage(WeaponType weaponType, Direction dir)
    {
        DisableAllSwordColliders();
        EnableSwordCollider(dir);
        // Debug.LogFormat("Do damage player. Weapon: {0}, Direction: {1}", weaponType, dir);
    }

    public void EndDamageSword()
    {
        DisableAllSwordColliders();
    }

    void DisableAllSwordColliders()
    {
        foreach (var kvp in swordColliderDict)
        {
            kvp.Value.enabled = false;
        }
    }

    void EnableSwordCollider(Direction direction)
    {
        swordColliderDict[direction].enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if (enemy != null)
            Debug.LogFormat("Receive damage from {0}", enemy.gameObject.name);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.LogFormat("Collision exit detected: {0}", collision.gameObject.name);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Debug.LogFormat("Collision stay detected: {0}", collision.gameObject.name);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
        if (enemy != null)
            Debug.LogFormat("Attack {0}", enemy.gameObject.name);
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
  