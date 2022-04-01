using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Potions")] // TODO: Don't auto use potions in ground
    public int healtPotions;
    public int manaPotions;
    
    [Header("Debug")]
    [SerializeField] int keys;

    [Header("Weapons")]
    [SerializeField] bool hasSword;
    [SerializeField] bool hasBow;
    [SerializeField] bool hasMagic;

    SwordController swordController;
    BowController bowController;
    MagicController magicController;
    HealthSystem playerHealth;


    void Awake()
    {
        GameReferences.inventoryManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameReferences.player.GetComponent<HealthSystem>();
        swordController = GameReferences.player.GetComponent<SwordController>();
        bowController = GameReferences.player.GetComponent<BowController>();
        magicController = GameReferences.player.GetComponent<MagicController>();
    }

    public int GetKeys()
    {
        return keys;
    }

    public bool UseKeys(int useKeys)
    {
        if (keys >= useKeys)
        {
            keys -= useKeys;
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool PickUp(PickUpType type, int value)
    {
        switch (type)
        {
        case PickUpType.HEALTH:
            return playerHealth.Heal(value);
        case PickUpType.MANA:
            if (!hasMagic) return false;
            return magicController.IncreaseMana(value);
        case PickUpType.ARROW:
            if (!hasBow) return false;
            return bowController.IncreaseAmmo(value);
        case PickUpType.KEY:
            keys++;
            return true;
        case PickUpType.SWORD:
            hasSword = true;
            PickUpWeapon(WeaponType.SWORD, value);
            // swordController.ChangeWeapon(ref value);
            return true;
        case PickUpType.BOW:
            hasBow = true;
            PickUpWeapon(WeaponType.BOW, value);
            // bowController.ChangeWeapon(ref value);
            return true;
        case PickUpType.MAGIC:
            hasMagic = true;
            PickUpWeapon(WeaponType.MAGIC, value);
            // magicController.ChangeWeapon(ref value);
            return true;
        default:
            break;
        }
        return false;
    }

    public bool PickUpWeapon(WeaponType weaponType, int index)
    {
        return GameReferences.player.ChangeWeapon(weaponType, index);
    }

    public bool HasWeapon(WeaponType weaponType)
    {
        switch (weaponType)
        {
        case WeaponType.SWORD:
            if (hasSword) return true;
            break;
        case WeaponType.BOW:
            if (hasBow) return true;
            break;
        case WeaponType.MAGIC:
            if (hasMagic) return true;
            break;
        default:
            break;
        }
        return false;
    }

    public WeaponType GetNextWeapon(WeaponType weaponType = WeaponType.NONE)
    {
        if (weaponType == WeaponType.NONE)
        {
            if (hasSword) return WeaponType.SWORD;
            if (hasBow) return WeaponType.BOW;
            if (hasMagic) return WeaponType.MAGIC;
            return WeaponType.NONE;
        }

        if (weaponType == WeaponType.SWORD)
        {
            if (hasBow) return WeaponType.BOW;
            if (hasMagic) return WeaponType.MAGIC;
            if (hasSword) return WeaponType.SWORD;
            return WeaponType.NONE;
        }

        if (weaponType == WeaponType.BOW)
        {
            if (hasMagic) return WeaponType.MAGIC;
            if (hasSword) return WeaponType.SWORD;
            if (hasBow) return WeaponType.BOW;
            return WeaponType.NONE;
        }

        if (weaponType == WeaponType.MAGIC)
        {
            if (hasSword) return WeaponType.SWORD;
            if (hasBow) return WeaponType.BOW;
            if (hasMagic) return WeaponType.MAGIC;
            return WeaponType.NONE;
        }

        return WeaponType.NONE;
    }

}
