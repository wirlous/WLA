using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;
using WLA;


public class ArcController : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform centerPlayer;
    [SerializeField] ArcDataStructure arcData;
    [SerializeField] int ammunition; 

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeWeapon(int index)
    {
        arcData = GameReferences.gameManager.GetArcData(index);
        ammunition = arcData.maxAmmunition;
    }

    public void Attack(Direction dir)
    {
        if (ammunition > 0)
        {
            // Debug.Log("Attacking with " + arcData.name + " in direction " + dir);
            float arrowAngle = GameReferences.GetAngleFromDir(dir);
            Vector3 arrowDir = GameReferences.GetDirVector(dir);
            Vector3 arrowPos = centerPlayer.position + arrowDir * 0.5f;
            
            GameObject arrow = Instantiate(arrowPrefab, arrowPos, Quaternion.Euler(0, 0, arrowAngle));
            arrow.transform.parent = null;

            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            arrowRb.velocity = arrowDir * arcData.proyectile.speed;

            ArrowController arrowController = arrow.GetComponent<ArrowController>();
            arrowController.SetDamage(arcData.damage);
            arrowController.SetMaxDistance(arcData.proyectile.maxDistance);
            
            arrow.tag = "Arrow";
            arrow.layer = LayerMask.NameToLayer("Arrow");

            ammunition--;
        }
    }

    public void EndAttack()
    {

    }

    public void IncreaseAmmo(int ammo)
    {
        ammunition = Mathf.Max(ammunition+ammo, arcData.maxAmmunition);
    }

    
    public int GetDamage()
    {
        return arcData.damage;
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
    //     enemy?.ReceiveHit(transform.position);

    //     HealthSystem otherHealth = other.gameObject.GetComponent<HealthSystem>();
    //     otherHealth?.ReceiveDamage(swordData.damage);
    // }
}
