using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WLA;

public class GameManager : MonoBehaviour
{
    public TextAsset arcJson;
    public TextAsset swordJson;

    [SerializeField] private ArcData arcData;
    [SerializeField] private SwordData swordData;

    void Awake()
    {
        GameReferences.gameManager = this;

        string arcString = arcJson.text;
        arcData = JsonUtility.FromJson<ArcData>(arcString);

        string swordString = swordJson.text;
        swordData = JsonUtility.FromJson<SwordData>(swordString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SwordDataStructure GetSwordData(int index)
    {
        int size = swordData.swords.Length;
        index = Freya.Mathfs.Mod(index, size);
        Debug.LogFormat("Size = {0}, index = {1}", size, index);
        return swordData.swords[index];
    }

    public ArcDataStructure GetArcData(int index)
    {
        int size = arcData.arcs.Length;
        index = Freya.Mathfs.Mod(index, size);
        Debug.LogFormat("Size = {0}, index = {1}", size, index);
        return arcData.arcs[index];
    }
}
