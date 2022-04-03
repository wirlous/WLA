using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CanvasManager : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI timeTMP;
    public TextMeshProUGUI weaponsTMP;
    public TextMeshProUGUI statsTMP;

    // Time info
    private float timePassed;
    private float initialTime;

    // Weapons info
    WeaponType weaponUsed = WeaponType.NONE;
    string swordName = "";
    string bowName = "";
    string magicName = "";

    // Stats info
    HealthSystem playerHealth;
    BowController bowController;
    MagicController magicController;

    void Awake()
    {
        GameReferences.canvasManager = this;
    }


    void Start()
    {
        initialTime = Time.time;
        timePassed = 0;

        playerHealth = GameReferences.player.GetComponent<HealthSystem>();
        bowController = GameReferences.player.GetComponent<BowController>();
        magicController = GameReferences.player.GetComponent<MagicController>();
    }

    // Update is called once per frame
    void Update()
    {
        timePassed = Time.time - initialTime;

        ShowTime();
        ShowWeapons();
        ShowStats();
    }

    private void ShowTime()
    {
        var timeSpan = System.TimeSpan.FromSeconds(timePassed);
        int hour = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        int milliseconds = timeSpan.Milliseconds;

        if (hour == 0)
        {
            timeTMP.text = System.String.Format ("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }
        else
        {
            timeTMP.text = System.String.Format ("{0:00}:{1:00}:{2:00}.{3:000}", hour, minutes, seconds, milliseconds);

        }
    }

    internal void SetWeaponName(WeaponType weapon, string name)
    {
        switch (weapon)
        {
        case WeaponType.SWORD:
            swordName = name;
            break;
        case WeaponType.BOW:
            bowName = name;
            break;
        case WeaponType.MAGIC:
            magicName = name;
            break;
        default:
            break;
        }
    }

    internal void SetUseWeapon(WeaponType weaponType)
    {
        weaponUsed = weaponType;
    }

    private void ShowWeapons()
    {
        string weaponStr = "";
        if (swordName != "")
        {
            weaponStr += System.String.Format("{0} {1}\n", (weaponUsed==WeaponType.SWORD)?"[X]":"[ ]", swordName);
        }

        if (bowName != "")
        {
            weaponStr += System.String.Format("{0} {1}\n", (weaponUsed==WeaponType.BOW)?"[X]":"[ ]", bowName);
        }

        if (magicName != "")
        {
            weaponStr += System.String.Format("{0} {1}\n", (weaponUsed==WeaponType.MAGIC)?"[X]":"[ ]", magicName);
        }
        
        weaponsTMP.text = weaponStr;
    }

    private void ShowStats()
    {
        string statsStr = "";
        statsStr += System.String.Format("Health: {0}/{1}\n", playerHealth.Health, playerHealth.maxHealth);

        if (bowName != "")
        {
            statsStr += System.String.Format("Arrows: {0}\n", bowController.Ammunition);
        }

        if (magicName != "")
        {
            statsStr += System.String.Format("Mana: {0}/{1}\n", magicController.Mana, magicController.MaxMana);
        }
        
        statsTMP.text = statsStr;
    }

}
