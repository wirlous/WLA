using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WLA;


public class SwordController : MonoBehaviour
{
    [SerializeField] SwordDataStructure swordData;
    [SerializeField] private Dictionary<Direction, BoxCollider2D> collidersDict = new Dictionary<Direction, BoxCollider2D>();

    void Awake()
    {
        collidersDict.Add(Direction.UP, gameObject.AddComponent<BoxCollider2D>());
        collidersDict.Add(Direction.DOWN, gameObject.AddComponent<BoxCollider2D>());
        collidersDict.Add(Direction.RIGHT, gameObject.AddComponent<BoxCollider2D>());
        collidersDict.Add(Direction.LEFT, gameObject.AddComponent<BoxCollider2D>());
    }

    // Start is called before the first frame update
    void Start()
    {
        swordData = GameReferences.gameManager.GetSwordData(0);
        ApplyDataToBoxColliders();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeWeapon(int index)
    {
        swordData = GameReferences.gameManager.GetSwordData(index);
        ApplyDataToBoxColliders();
    }

    void ApplyDataToBoxColliders()
    {
        collidersDict[Direction.UP].offset = swordData.upCollider.GetOffset();
        collidersDict[Direction.UP].size = swordData.upCollider.GetSize();
        collidersDict[Direction.UP].isTrigger = true;
        collidersDict[Direction.UP].enabled = false;

        collidersDict[Direction.DOWN].offset = swordData.downCollider.GetOffset();
        collidersDict[Direction.DOWN].size = swordData.downCollider.GetSize();
        collidersDict[Direction.DOWN].isTrigger = true;
        collidersDict[Direction.DOWN].enabled = false;

        collidersDict[Direction.RIGHT].offset = swordData.rightCollider.GetOffset();
        collidersDict[Direction.RIGHT].size = swordData.rightCollider.GetSize();
        collidersDict[Direction.RIGHT].isTrigger = true;
        collidersDict[Direction.RIGHT].enabled = false;

        collidersDict[Direction.LEFT].offset = swordData.leftCollider.GetOffset();
        collidersDict[Direction.LEFT].size = swordData.leftCollider.GetSize();
        collidersDict[Direction.LEFT].isTrigger = true;
        collidersDict[Direction.LEFT].enabled = false;
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
        collidersDict[dir].enabled = true;
    }

    private void DisableAllColliders()
    {
        collidersDict[Direction.UP].enabled = false;
        collidersDict[Direction.DOWN].enabled = false;
        collidersDict[Direction.RIGHT].enabled = false;
        collidersDict[Direction.LEFT].enabled = false;
    }

    public int GetDamage()
    {
        return swordData.damage;
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
    //     enemy?.ReceiveHit(transform.position);

    //     HealthSystem otherHealth = other.gameObject.GetComponent<HealthSystem>();
    //     otherHealth?.ReceiveDamage(swordData.damage);
    // }
}
