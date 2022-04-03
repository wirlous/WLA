using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowController : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform centerPlayer;
    
    [Header("Debug")]
    [SerializeField] BowDataStructure bowData;
    [SerializeField] int ammunition;

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

    public string GetWeaponName()
    {
        return bowData.name;
    }

    public void ChangeWeapon(ref int index)
    {
        bowData = GameReferences.gameManager.GetBowData(ref index);
        ammunition = bowData.maxAmmunition;
    }

    public void Attack(Direction dir)
    {
        if (ammunition > 0)
        {
            // Debug.Log("Attacking with " + bowData.name + " in direction " + dir);
            float arrowAngle = GameReferences.GetAngleFromDir(dir);
            Vector3 arrowDir = GameReferences.GetDirVector(dir);
            Vector3 arrowPos = centerPlayer.position + arrowDir * GameReferences.shootOffset;
            
            GameObject arrow = Instantiate(arrowPrefab, arrowPos, Quaternion.Euler(0, 0, arrowAngle));
            arrow.transform.parent = null;

            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            arrowRb.velocity = arrowDir * bowData.arrow.speed;

            ProyectileController proyectileController = arrow.GetComponent<ProyectileController>();
            proyectileController.SetDamage(bowData.damage);
            proyectileController.SetMaxDistance(bowData.arrow.maxDistance);
            
            arrow.tag = "Arrow";
            arrow.layer = LayerMask.NameToLayer("Arrow");

            ammunition--;
        }
    }

    public void EndAttack()
    {

    }

    public bool IncreaseAmmo(int ammo)
    {
        ammunition = Mathf.Max(ammunition+ammo, bowData.maxAmmunition);
        return true;
    }

    
    public int GetDamage()
    {
        return bowData.damage;
    }

    private void OnDrawGizmos()
    {
        DrawCircleSphere(bowData.arrow.maxDistance + GameReferences.shootOffset, Color.red);
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

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
    //     enemy?.ReceiveHit(transform.position);

    //     HealthSystem otherHealth = other.gameObject.GetComponent<HealthSystem>();
    //     otherHealth?.ReceiveDamage(swordData.damage);
    // }
}
