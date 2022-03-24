﻿using System;
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
    Vector2 moveDir;
    [SerializeField]
    float moveFactor;

    // Internal
    PlayerInput playerInput;
    Rigidbody2D rb;
    Timer moveTimer;


    // public Timer.TimerDelegate moveTimerCallback;
    
    void Awake()
    {
        // moveTimerCallback += MoveTimerDone;

        rb = GetComponent<Rigidbody2D>();
        
        moveTimer = GetComponent<Timer>();
        moveTimer.Init(moveSpeedUpTime);
    
        playerInput = new PlayerInput();
        

        // Register Attack
        playerInput.Gameplay.Attack.performed += ctx => Attack();
        
        // Register Move
        playerInput.Gameplay.Move.performed += ctx => moveDir = ctx.ReadValue<Vector2>();
        playerInput.Gameplay.Move.performed += ctx => StartMove();
        playerInput.Gameplay.Move.canceled += ctx => StopMove();
    }

    void StopMove()
    {
        moveDir = Vector2.zero;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

        float t = moveTimer.GetTimeNormalize();
        float moveFactor = moveSpeedLerp.Evaluate(t);

        // Movement
        rb.MovePosition(rb.position + moveDir * moveSpeed * moveFactor * Time.fixedDeltaTime);

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
