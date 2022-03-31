using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WLA;

public class SwordController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] SwordDataStructure swordData;

#if (UNITY_EDITOR)
    // Show Dictionary in editor
    [System.Serializable] private class DirectionBoxCollider2D : KeyValuePair<Direction, BoxCollider2D> { }; 
    [SerializeField] private DirectionBoxCollider2D[] colliders = new DirectionBoxCollider2D[4];
#endif
    [SerializeField] private Dictionary<Direction, BoxCollider2D> collidersInternal = new Dictionary<Direction, BoxCollider2D>();

    void Awake()
    {
        collidersInternal.Add(Direction.UP, gameObject.AddComponent<BoxCollider2D>());
        collidersInternal.Add(Direction.DOWN, gameObject.AddComponent<BoxCollider2D>());
        collidersInternal.Add(Direction.RIGHT, gameObject.AddComponent<BoxCollider2D>());
        collidersInternal.Add(Direction.LEFT, gameObject.AddComponent<BoxCollider2D>());
    }

#if (UNITY_EDITOR)
    void OnGUI()
    {
        colliders[0].key = Direction.UP;
        colliders[0].value = collidersInternal[Direction.UP];

        colliders[1].key = Direction.DOWN;
        colliders[1].value = collidersInternal[Direction.DOWN];

        colliders[2].key = Direction.RIGHT;
        colliders[2].value = collidersInternal[Direction.RIGHT];

        colliders[3].key = Direction.LEFT;
        colliders[3].value = collidersInternal[Direction.LEFT];
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        int initialIndex = 0;
        ChangeWeapon(ref initialIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeWeapon(ref int index)
    {
        swordData = GameReferences.gameManager.GetSwordData(ref index);
        ApplyDataToBoxColliders();
    }

    void ApplyDataToBoxColliders()
    {
        collidersInternal[Direction.UP].offset = swordData.upCollider.GetOffset();
        collidersInternal[Direction.UP].size = swordData.upCollider.GetSize();
        collidersInternal[Direction.UP].isTrigger = true;
        collidersInternal[Direction.UP].enabled = false;

        collidersInternal[Direction.DOWN].offset = swordData.downCollider.GetOffset();
        collidersInternal[Direction.DOWN].size = swordData.downCollider.GetSize();
        collidersInternal[Direction.DOWN].isTrigger = true;
        collidersInternal[Direction.DOWN].enabled = false;

        collidersInternal[Direction.RIGHT].offset = swordData.rightCollider.GetOffset();
        collidersInternal[Direction.RIGHT].size = swordData.rightCollider.GetSize();
        collidersInternal[Direction.RIGHT].isTrigger = true;
        collidersInternal[Direction.RIGHT].enabled = false;

        collidersInternal[Direction.LEFT].offset = swordData.leftCollider.GetOffset();
        collidersInternal[Direction.LEFT].size = swordData.leftCollider.GetSize();
        collidersInternal[Direction.LEFT].isTrigger = true;
        collidersInternal[Direction.LEFT].enabled = false;
    }

    public void Attack(Direction dir)
    {
        // DisableAllColliders();
        EnableCollider(dir);
        // Debug.Log("Attacking with " + swordData.name + " in direction " + dir);
    }

    public void EndAttack()
    {
        DisableAllColliders();
    }

    private void EnableCollider(Direction dir)
    {
        collidersInternal[dir].enabled = true;
    }

    private void DisableAllColliders()
    {
        collidersInternal[Direction.UP].enabled = false;
        collidersInternal[Direction.DOWN].enabled = false;
        collidersInternal[Direction.RIGHT].enabled = false;
        collidersInternal[Direction.LEFT].enabled = false;
    }

    public int GetDamage()
    {
        return swordData.damage;
    }

    private void OnDrawGizmos()
    {
        if (collidersInternal.Count != 0)
        {
            DrawRectangle(transform.position, collidersInternal[Direction.UP].offset, collidersInternal[Direction.UP].size, Color.red);
            DrawRectangle(transform.position, collidersInternal[Direction.DOWN].offset, collidersInternal[Direction.DOWN].size, Color.red);
            DrawRectangle(transform.position, collidersInternal[Direction.RIGHT].offset, collidersInternal[Direction.RIGHT].size, Color.red);
            DrawRectangle(transform.position, collidersInternal[Direction.LEFT].offset, collidersInternal[Direction.LEFT].size, Color.red);
        }
    }

    protected void DrawRectangle(Vector2 position, Vector2 offset, Vector2 size, Color color)
    {
        Gizmos.color = color;

        Vector2 center = position + offset;
        
        Vector2 p0 = new Vector2(center.x + 0.5f*size.x, center.y + 0.5f*size.y);
        Vector2 p1 = new Vector2(center.x + 0.5f*size.x, center.y - 0.5f*size.y);
        Vector2 p2 = new Vector2(center.x - 0.5f*size.x, center.y - 0.5f*size.y);
        Vector2 p3 = new Vector2(center.x - 0.5f*size.x, center.y + 0.5f*size.y);
        
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
    //     enemy?.ReceiveHit(transform.position);

    //     HealthSystem otherHealth = other.gameObject.GetComponent<HealthSystem>();
    //     otherHealth?.ReceiveDamage(swordData.damage);
    // }
}
