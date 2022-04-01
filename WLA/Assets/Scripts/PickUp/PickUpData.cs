using UnityEngine;

[System.Serializable]
public class PickUpData
{
    public string version;
    public PickUpDataStructure[] pickUps;
}

[System.Serializable]
public class PickUpDataStructure
{
    public string type;
    public string spriteReference;
    public int minValue;
    public int maxValue;

    public PickUpDataStructure(string type, string spriteReference, int minValue, int maxValue)
    {
        this.type = type;
        this.spriteReference = spriteReference;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public PickUpDataStructure()
    {
        this.type = "";
        this.spriteReference = "";
        this.minValue = 0;
        this.maxValue = 0;
    }
}
