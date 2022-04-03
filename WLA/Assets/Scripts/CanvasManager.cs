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

    // Time info
    private float timePassed;
    private float initialTime;

    // Weapons info
    [SerializeField] WeaponType weaponUsed = WeaponType.NONE;
    string swordName = "";
    string bowName = "";
    string magicName = "";

    void Awake()
    {
        GameReferences.canvasManager = this;
    }


    void Start()
    {
        initialTime = Time.time;
        timePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed = Time.time - initialTime;

        ShowTime();
        ShowWeapons();
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

}
