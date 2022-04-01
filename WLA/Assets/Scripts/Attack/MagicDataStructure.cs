using UnityEngine;

[System.Serializable]
public class MagicData
{
    public string version;
    public MagicDataStructure[] spells;
}

[System.Serializable]
public class MagicDataStructure
{
    public string name;
    public int code;
    public int damage;
    public int maxMana;
    public int manaCost;
    public SpellDefinition spell;

    public MagicDataStructure(string name, int code, int damage, int maxMana, int manaCost, SpellDefinition spell)
    {
        this.name = name;
        this.code = code;
        this.damage = damage;
        this.maxMana = maxMana;
        this.manaCost = manaCost;
        this.spell = spell;
    }

    public MagicDataStructure()
    {
        this.name = "";
        this.code = -1;
        this.damage = 0;
        this.maxMana = 0;
        this.manaCost = 0;
        this.spell = null;
    }

}

[System.Serializable]
public class SpellDefinition
{
    public float radius;
    public float height;
    public float speed;
    public float maxDistance;

    public SpellDefinition(float radius, float height, float speed, float maxDistance)
    {
        this.radius         = radius;
        this.height         = height;
        this.speed          = speed;
        this.maxDistance    = maxDistance;
    }

    public SpellDefinition()
    {
        this.radius         = 0.5f;
        this.height         = 1f;
        this.speed          = 1f;
        this.maxDistance    = 1f;
    }

}
