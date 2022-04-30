using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("Json data")]
    public TextAsset swordJson;
    public TextAsset bowJson;
    public TextAsset magicJson;
    public TextAsset pickUpJson;

    public int points;
    public float time;

    public bool degug;

    [SerializeField] private SwordData swordData;
    [SerializeField] private BowData bowData;
    [SerializeField] private MagicData magicData;
    [SerializeField] private PickUpData pickUpData;

    [SerializeField] PlayerInput playerInput;

    void Awake()
    {
        GameReferences.gameManager = this;

        GameObject[] cmvcam = GameObject.FindGameObjectsWithTag("cmvcam");
        GameReferences.cmvcam = cmvcam[0].GetComponent<CinemachineVirtualCamera>();

        string swordString = swordJson.text;
        swordData = JsonUtility.FromJson<SwordData>(swordString);

        string bowString = bowJson.text;
        bowData = JsonUtility.FromJson<BowData>(bowString);

        string magicString = magicJson.text;
        magicData = JsonUtility.FromJson<MagicData>(magicString);

        playerInput = new PlayerInput();
        playerInput.Control.GameQuit.performed += ctx => QuitApplication();
        playerInput.Control.ShowDebug.performed += ctx => ToggleDebug();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameReferences.enemies.Count == 0 && !degug)
        {
            Win();
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
        StartCoroutine( RestartLevelCorrutine(2) );
    }

    public void Win()
    {
        GameReferences.canvasManager.ShowMessage("You win!");
        GameReferences.canvasManager.StopTime();
        StartCoroutine( RestartLevelCorrutine(2) );
    }

    IEnumerator RestartLevelCorrutine(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        RestartLevel();
    }
    

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitApplication()
    {
        Application.Quit();
    }

    private void ToggleDebug()
    {
        GameReferences.canvasManager.ToggleDebug();
    }

}
