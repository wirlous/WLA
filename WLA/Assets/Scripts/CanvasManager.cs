using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class CanvasManager : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI timeTMP;
    public TextMeshProUGUI weaponsTMP;
    public TextMeshProUGUI statsTMP;
    public TextMeshProUGUI inventoryTMP;
    public TextMeshProUGUI messageTMP;
    public TextMeshProUGUI debugTMP;

    // Time info
    private float timePassed;
    private float initialTime;
    [SerializeField] private bool timeRunning;

    // Weapons info
    WeaponType weaponUsed = WeaponType.NONE;
    string swordName = "";
    string bowName = "";
    string magicName = "";

    // Stats info
    HealthSystem playerHealth;
    BowController bowController;
    MagicController magicController;

    // Inventory
    InventoryManager inventoryManager;

    // Message
    string message;

    // Debug
    [SerializeField] bool showDebug;
    float deltaTime;
    List<float> fpsList = new List<float>();
    [SerializeField] int fpsIndex = 0;
    const int fpsListSize = 500;

    void Awake()
    {
        GameReferences.canvasManager = this;
    }

    void Start()
    {
        timeRunning = true;
        initialTime = Time.time;
        timePassed = 0;

        playerHealth = GameReferences.player.GetComponent<HealthSystem>();
        bowController = GameReferences.player.GetComponent<BowController>();
        magicController = GameReferences.player.GetComponent<MagicController>();

        inventoryManager = GameReferences.player.GetComponent<InventoryManager>();
        
        showDebug = true;
        deltaTime = 0;

        ShowMessage("");
    }

    public void ToggleDebug()
    {
        showDebug = !showDebug;
    }

    // Update is called once per frame
    void Update()
    {
        ShowTime();
        ShowWeapons();
        ShowStats();
        ShowInventory();
        ShowDebug();
    }

    public void StopTime()
    {
        timeRunning = false;
    }

    internal void ShowMessage(string message)
    {
        messageTMP.text = message;
    }

    private void ShowTime()
    {
        if (timeRunning)
            timePassed = Time.time - initialTime;

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

    private void ShowInventory()
    {
        string inventoryStr = "";
        if (inventoryManager.Keys != 0)
        {
            inventoryStr += System.String.Format("Keys: {0}\n", inventoryManager.Keys);
        }
        
        inventoryTMP.text = inventoryStr;
    }

    private void ShowDebug()
    {
        string debugStr = "";
        if (showDebug)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            
            if (fpsList.Count < fpsListSize)
            {
                fpsList.Add(fps);
            }
            else
            {
                fpsList[fpsIndex] = fps;
                fpsIndex = (fpsIndex + 1) % fpsListSize;
            }
            
            debugStr = System.String.Format("FPS: {0}", Mathf.Ceil(fpsList.Average()));
        }

        debugTMP.text = debugStr;
    }

}
