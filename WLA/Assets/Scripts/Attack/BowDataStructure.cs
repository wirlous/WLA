using UnityEngine;

[System.Serializable]
public class BowData
{
    public string version;
    public BowDataStructure[] bows;
}

[System.Serializable]
public class BowDataStructure
{
    public string name;
    public int code;
    public int damage;
    public int maxAmmunition;
    public ArrowDefinition arrow;

    public BowDataStructure(string name, int code, int damage, int maxAmmunition, ArrowDefinition arrow)
    {
        this.name = name;
        this.code = code;
        this.damage = damage;
        this.maxAmmunition = maxAmmunition;
        this.arrow = arrow;
    }

    public BowDataStructure()
    {
        this.name = "";
        this.code = -1;
        this.damage = 0;
        this.maxAmmunition = 0;
        this.arrow = null;
    }

}

[System.Serializable]
public class ArrowDefinition
{
    public float radius;
    public float height;
    public float speed;
    public float maxDistance;

    public ArrowDefinition(float radius, float height, float speed, float maxDistance)
    {
        this.radius         = radius;
        this.height         = height;
        this.speed          = speed;
        this.maxDistance    = maxDistance;
    }

    public ArrowDefinition()
    {
        this.radius         = 0.5f;
        this.height         = 1f;
        this.speed          = 1f;
        this.maxDistance    = 1f;
    }

}
