using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


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
    
    // Private
    [SerializeField]
    public Vector2 facingDirection;
    public Vector2 movement;
    [SerializeField]
    float moveFactor;

    // Internal
    PlayerInput playerInput;
    Animator animator;
    Rigidbody2D rb;
    Timer moveTimer;

    void Awake()
    {
        GameReferences.player = this;

        facingDirection = new Vector2(0,-1);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        
        moveTimer = GetComponent<Timer>();
        moveTimer.Init(moveSpeedUpTime);
    
        playerInput = new PlayerInput();
        
        // Register Attack
        playerInput.Gameplay.Attack.performed += ctx => Attack();
        
        // Register Move
        playerInput.Gameplay.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        playerInput.Gameplay.Move.performed += ctx => StartMove();
        playerInput.Gameplay.Move.canceled += ctx => StopMove();
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
        Debug.Log("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        float speed = movement.sqrMagnitude;
        if (speed > 0.1f)
        {
            facingDirection = movement;
        }
        animator?.SetFloat("Speed", movement.sqrMagnitude);
        animator?.SetFloat("Horizontal", movement.x);
        animator?.SetFloat("Vertical", movement.y);
        animator?.SetFloat("FacingHorizontal", facingDirection.x);
        animator?.SetFloat("FacingVertical", facingDirection.y);
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
}
