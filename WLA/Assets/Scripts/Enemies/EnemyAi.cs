using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    // Public
    [Range(1f, 20f)]
    public float moveSpeed = 1f;
    [Range(0f, 5f)]
    public float minDistanceToPlayer = 2f;
    [Range(5f, 10f)]
    public float maxDistanceToPlayer = 5f;
    [Range(1, 10)]
    public int damage = 1;
    public float receiveDamageFactor = 1f;

    Vector2 movement;
    public float moveSpeedStore;

    public float stunTime = 0;

    // Internal
    private PlayerController player;
    private Animator animator;
    private Rigidbody2D rb;

    protected void Awake()
    {
        GameReferences.AddEnemy(this);

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        animator = GetComponentInChildren<Animator>();

        moveSpeedStore = moveSpeed;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        player = GameReferences.player;
        movement = Vector2.zero;
    }

    // Update is called once per frame
    protected void Update()
    {
        movement = GetMovement();

        animator?.SetFloat("Speed", movement.sqrMagnitude);
        animator?.SetFloat("Horizontal", movement.x);
        animator?.SetFloat("Vertical", movement.y);
    }

    public int GetDamage()
    {
        return damage;
    }

    protected void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void DoDamage(int damage, Transform tOrigin)
    {
        Debug.LogFormat("{0} receive damge {1}", name, damage);
        DoPush(transform.position - tOrigin.position);
        stunTime += GameReferences.stunTime;
        animator?.SetTrigger("Hit");
    }

    void DoPush(Vector3 pushDir)
    {
        Vector3 knockback = pushDir.normalized * GameReferences.knockbackFactor;
        transform.position = transform.position + knockback;
    }

    private Vector2 GetMovement()
    {
        if (moveSpeed == 0)
            return Vector2.zero;

        float distance = Vector2.Distance (transform.position, player.transform.position);
        if (distance > minDistanceToPlayer && distance < maxDistanceToPlayer)
        {
            return (Vector2)(player.transform.position - transform.position).normalized;
        }
        else
        {
            return Vector2.zero;
        }
    }

    public void StartStun()
    {
        Debug.Log("StartStun");
        moveSpeed = 0;
        receiveDamageFactor = 0.5f;
    }

    public void StopStun()
    {
        Debug.Log("StopStun");
        moveSpeed = moveSpeedStore;
        receiveDamageFactor = 1f;
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

}
