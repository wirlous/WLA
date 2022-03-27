using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    UP,
    DOWN,
    RIGHT,
    LEFT
};

public enum WeaponType
{
    NONE,
    SWORD,
    ARC,
    MAGIC
};

public static class GameReferences
{
    public static PlayerController player;
    public static List<EnemyAI> enemies = new List<EnemyAI>();

    public static float stunTime = 0.5f;
    public static float knockbackFactor = 0.25f;


    public static void AddEnemy(EnemyAI obj)
    {
        enemies.Add(obj);
    }

    public static void RemoveEnemy(EnemyAI obj)
    {
        enemies.Remove(obj);
    }

}
