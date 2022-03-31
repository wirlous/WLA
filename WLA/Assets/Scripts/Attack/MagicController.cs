using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WLA;

public class MagicController : MonoBehaviour
{
    [Header("Magic")]
    public GameObject magicPrefab;
    public Transform centerPlayer;
    
    [Header("Debug")]
    [SerializeField] MagicDataStructure magicData;
    [SerializeField] int mana;
    [SerializeField] int maxMana; // TODO: Extract to ManaSystem (Similar to HealthSystem)
    [SerializeField] int manaCost;

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
        magicData = GameReferences.gameManager.GetMagicData(ref index);
        maxMana = magicData.maxMana;
        manaCost = magicData.manaCost;
        mana = maxMana;
    }

    public void Attack(Direction dir)
    {
        if ((mana - manaCost) > 0)
        {
            // Debug.Log("Attacking with " + magicData.name + " in direction " + dir);
            float magicAngle = GameReferences.GetAngleFromDir(dir);
            Vector3 magicDir = GameReferences.GetDirVector(dir);
            Vector3 magicPos = centerPlayer.position + magicDir * GameReferences.shootOffset;
            
            GameObject magic = Instantiate(magicPrefab, magicPos, Quaternion.Euler(0, 0, magicAngle));
            magic.transform.parent = null;

            Rigidbody2D magicRb = magic.GetComponent<Rigidbody2D>();
            magicRb.velocity = magicDir * magicData.spell.speed;

            ProyectileController proyectileController = magic.GetComponent<ProyectileController>();
            proyectileController.SetDamage(magicData.damage);
            proyectileController.SetMaxDistance(magicData.spell.maxDistance);
            
            magic.tag = "Magic";
            magic.layer = LayerMask.NameToLayer("Magic");

            mana -= manaCost;
        }
    }

    public void EndAttack()
    {

    }

    public bool IncreaseMana(int addMana)
    {
        if (mana == maxMana)
            return false;
        
        mana = Mathf.Min(maxMana, mana + addMana);
        return true;
    }

    
    public int GetDamage()
    {
        return magicData.damage;
    }

    private void OnDrawGizmos()
    {
        DrawCircleSphere(magicData.spell.maxDistance + GameReferences.shootOffset, Color.blue);
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
