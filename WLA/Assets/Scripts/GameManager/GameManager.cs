using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WLA;

public class GameManager : MonoBehaviour
{
    [Header("Json data")]
    public TextAsset swordJson;
    public TextAsset bowJson;
    public TextAsset magicJson;
    public TextAsset pickUpJson;

    public int points;
    public float time;

    [SerializeField] private SwordData swordData;
    [SerializeField] private BowData bowData;
    [SerializeField] private MagicData magicData;
    [SerializeField] private PickUpData pickUpData;

    void Awake()
    {
        GameReferences.gameManager = this;

        string swordString = swordJson.text;
        swordData = JsonUtility.FromJson<SwordData>(swordString);

        string bowString = bowJson.text;
        bowData = JsonUtility.FromJson<BowData>(bowString);

        string magicString = magicJson.text;
        magicData = JsonUtility.FromJson<MagicData>(magicString);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
