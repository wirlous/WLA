using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileController : MonoBehaviour
{

    [SerializeField] private float maxDistance;
    [SerializeField] private int damage;

    private Vector3 originPosition;
    private int wallLayer;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        originPosition = transform.position;
        wallLayer = LayerMask.NameToLayer("Wall");
    }

    void Update()
    {
        float distance = Vector3.Distance(originPosition, transform.position);
        if (distance >= maxDistance)
        {
            Delete();
        }
    }

    public void SetMaxDistance(float distance)
    {
        maxDistance = distance;
    }
    
    public void SetDamage(int d)
    {
        damage = d;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
        Destroy(gameObject);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.tag;
        if (tag.Equals("Terrain") || (other.gameObject.layer == wallLayer))
        {
            Delete();
        }
    }

}
