using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    // Public
    [Header("Movement")]
    [Range(0f, 20f)] public float moveSpeed = 1f;
    [Header("Damage")]
    [Range(1, 10)] public int damage = 1;
    [Header("AI")]
    [Range(0f, 5f)] public float minDistanceToPlayer = 2f;
    [Range(5f, 10f)] public float maxDistanceToPlayer = 5f;

    [Header("Debug")]
    [SerializeField] Vector2 movement;
    [SerializeField] float moveSpeedStore;
    [SerializeField] Vector2 knockbackDistance;
    [SerializeField] bool isKnockback;

    // Internal
    private PlayerController player;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col2D;
    private HealthSystem health;



    protected void Awake()
    {
        GameReferences.AddEnemy(this);

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        col2D = GetComponent<Collider2D>();

        animator = GetComponentInChildren<Animator>();

        health = GetComponent<HealthSystem>();
        moveSpeedStore = moveSpeed;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        player = GameReferences.player;
        movement = Vector2.zero;

        knockbackDistance = Vector2.zero;
        isKnockback = false;
    }

    // Update is called once per frame
    protected void Update()
    {
        movement = GetMovement();

        animator?.SetFloat("Speed", movement.sqrMagnitude);
        animator?.SetFloat("Horizontal", movement.x);
        animator?.SetFloat("Vertical", movement.y);
    }

    private void OnEnable()
    {
        health.OnDeath += Die;
    }

    private void OnDisable()
    {
        health.OnDeath -= Die;
    }


    public int GetDamage()
    {
        return damage;
    }

    protected void FixedUpdate()
    {
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
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void ReceiveHit(Vector3 pOrigin)
    {
        Knockback2D(Freya.Mathfs.Dir(pOrigin, transform.position));
        animator?.SetTrigger("Hit");
    }

    void Knockback2D(Vector2 pushDir)
    {
        knockbackDistance = pushDir * GameReferences.knockbackFactor;
        isKnockback = true;
    }

    private Vector2 GetMovement()
    {
        if (moveSpeed == 0)
            return Vector2.zero;

        float distance = Vector2.Distance (transform.position, player.transform.position);
        if (distance > minDistanceToPlayer && distance < maxDistanceToPlayer)
        {
            return Freya.Mathfs.Dir(transform.position, player.transform.position);
        }
        else
        {
            return Vector2.zero;
        }
    }

    public void StartStun()
    {
        moveSpeed = 0;
    }

    public void StopStun()
    {
        moveSpeed = moveSpeedStore;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.tag;
        if (tag.Equals("Player"))
        {
            ReceiveHit(other.transform.position);

            health.ReceiveDamage(GameReferences.player.GetDamage());
        }
        else if (tag.Equals("Arrow"))
        {
            ReceiveHit(other.transform.position);
            ProyectileController proyectileController = other.gameObject.GetComponent<ProyectileController>();
            if (proyectileController != null)
            {
                health.ReceiveDamage(proyectileController.GetDamage());
                proyectileController.Hit();

            }
        }
        else if (tag.Equals("Magic"))
        {
            ReceiveHit(other.transform.position);
            ProyectileController proyectileController = other.gameObject.GetComponent<ProyectileController>();
            if (proyectileController != null)
            {
                health.ReceiveDamage(proyectileController.GetDamage());
                proyectileController.Hit();

            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawCircleSphere(minDistanceToPlayer, Color.blue);
        DrawCircleSphere(maxDistanceToPlayer, Color.red);
    }

    protected void DrawCircleSphere(float radius, Color color, float deltaTheta = 0.1f)
    {
        Gizmos.color = color;

        float theta = 0f;
        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);

        Vector3 center = transform.position;

        Vector3 offsetCircle = new Vector3(x, y, 0);
        Vector3 pos = center + offsetCircle;
        Vector3 newPos;
        Vector3 lastPos = pos;

        for (theta = deltaTheta; theta < Mathf.PI * 2; theta += deltaTheta)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);
            offsetCircle = new Vector3(x, y, 0);
            newPos = center + offsetCircle;
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);
    }

    private void Die()
    {
        Debug.LogFormat("Enemy {0} dies", name);
        moveSpeed = 0;
        col2D.enabled = false;
        animator?.SetTrigger("Die");
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

}
