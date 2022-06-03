using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Json data")]
    public TextAsset swordJson;
    public TextAsset bowJson;
    public TextAsset magicJson;
    public TextAsset pickUpJson;

    [Header("Dungeon")]
    [Range(0,5)] public int numDungeons;
    [Range(4,100)]public int initialNumberRooms;
    [Range(1,5)]public int incrementNumberRooms;
    [SerializeField] private List<DungeonGenerator> dungeons = new List<DungeonGenerator>();
    [SerializeField] private int dungeonIndex;
    public GameObject dungeonPrefab;

    public bool degug;

    [SerializeField] private SwordData swordData;
    [SerializeField] private BowData bowData;
    [SerializeField] private MagicData magicData;
    [SerializeField] private PickUpData pickUpData;

    [SerializeField] PlayerInput playerInput;

    void Awake()
    {
        GameReferences.gameManager = this;

        string swordString = swordJson.text;
        swordData = JsonUtility.FromJson<SwordData>(swordString);

        string bowString = bowJson.text;
        bowData = JsonUtility.FromJson<BowData>(bowString);

        string magicString = magicJson.text;
        magicData = JsonUtility.FromJson<MagicData>(magicString);

        playerInput = new PlayerInput();
        playerInput.Control.GameQuit.performed += ctx => QuitApplication();
        playerInput.Control.ShowDebug.performed += ctx => ToggleDebug();
        playerInput.Control.ResetGame.performed += ctx => RestartLevel();
    }

    void Start()
    {
        for (int i = 0; i < numDungeons; i++)
        {
            GameObject dungeonGo = Instantiate(dungeonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DungeonGenerator dungeon = dungeonGo.GetComponent<DungeonGenerator>();

            dungeon.roomCount = initialNumberRooms + (i*incrementNumberRooms);
            dungeon.level = i;
            if (GameReferences.initialSeed.Equals(""))
            {
                // Debug.Log("Initial seed empty");
                dungeon.useRandomSeed = true;
                dungeon.seed = "Dungeon" + i;
            }
            else
            {
                // Debug.Log("Initial seed not empty");
                dungeon.useRandomSeed = false;
                dungeon.seed = GameReferences.initialSeed + i;
            }

            dungeon.StartDungeon();

            dungeons.Add(dungeon);

            if (i!=0)
                dungeonGo.SetActive(false);
        }

        if (numDungeons > 0)
            PlacePlayer();
    }

    public void PlacePlayer()
    {
        dungeonIndex = 0;
        GameReferences.player.transform.position = dungeons[dungeonIndex].InitialPosition;
    }

    public void ExitReached()
    {
        if (dungeonIndex == (dungeons.Count-1))
        {
            GameReferences.timePassed = (int)(Time.time - GameReferences.canvasManager.initialTime);
            Win();
        }
        else
        {
            AddPoints(100);
            dungeons[dungeonIndex].gameObject.SetActive(false);
            dungeonIndex = (dungeonIndex+1)%dungeons.Count;
            dungeons[dungeonIndex].gameObject.SetActive(true);
            GameReferences.player.transform.position = dungeons[dungeonIndex].InitialPosition;
        }
    }

    void OnEnable()
    {
        playerInput.Control.Enable();

    }

    void OnDisable()
    {
        playerInput.Control.Disable();
    }

    public SwordDataStructure GetSwordData(ref int index)
    {
        int size = swordData.swords.Length;
        index = Freya.Mathfs.Mod(index, size);
        // Debug.LogFormat("Size = {0}, index = {1}", size, index);
        return swordData.swords[index];
    }

    public BowDataStructure GetBowData(ref int index)
    {
        int size = bowData.bows.Length;
        index = Freya.Mathfs.Mod(index, size);
        // Debug.LogFormat("Size = {0}, index = {1}", size, index);
        return bowData.bows[index];
    }

    public MagicDataStructure GetMagicData(ref int index)
    {
        int size = magicData.spells.Length;
        index = Freya.Mathfs.Mod(index, size);
        // Debug.LogFormat("Size = {0}, index = {1}", size, index);
        return magicData.spells[index];
    }

    public void Lose()
    {
        GameReferences.canvasManager.ShowMessage("You lose!");
        GameReferences.canvasManager.StopTime();
        foreach (var enemy in GameReferences.enemies)
        {
            enemy.StartStun();
        }
        GameReferences.player.moveSpeed = 0;
        StartCoroutine( QuitApplicationCorrutine(2) );
    }

    public void Win()
    {
        GameReferences.canvasManager.ShowMessage("You win!");
        GameReferences.canvasManager.StopTime();

        int currentBestTime = PlayerPrefs.GetInt("BestTime", 0);
        int currentHightScore = PlayerPrefs.GetInt("HighScore", 0);

        if ((currentBestTime == 0) || (currentBestTime > GameReferences.timePassed))
            PlayerPrefs.SetInt("BestTime", GameReferences.timePassed);

        if ((currentHightScore == 0) || (currentHightScore < GameReferences.score))
            PlayerPrefs.SetInt("HighScore", GameReferences.score);
        
        
        StartCoroutine( QuitApplicationCorrutine(2) );
    }

    IEnumerator QuitApplicationCorrutine(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        QuitApplication();
    }
    

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitApplication()
    {
        SceneManager.LoadScene(0);
        // Application.Quit();
    }

    private void ToggleDebug()
    {
        GameReferences.canvasManager.ToggleDebug();
    }


    public void AddPoints(int points)
    {
        GameReferences.score += points;
    }

    public void SubstractPoints(int points)
    {
        GameReferences.score -= points;
        if (GameReferences.score < 0)
            GameReferences.score = 0;
    }

}
