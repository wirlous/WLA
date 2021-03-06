using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public enum Direction
{
    UP,
    DOWN,
    RIGHT,
    LEFT,
};

[System.Serializable]
public enum WeaponType
{
    NONE,
    SWORD,
    BOW,
    MAGIC
};

#if (UNITY_EDITOR)
[System.Serializable]
public class KeyValuePair<K,V>
{
    public K key;
    public V value;
}
#endif

public static class GameReferences
{
    public static GameManager gameManager;
    public static CanvasManager canvasManager;
    public static InventoryManager inventoryManager;
    public static PlayerController player;
    public static List<EnemyAI> enemies = new List<EnemyAI>();
    public static AudioManager audioManager;

    public static int playerHealth = 50;
    public static float knockbackSpeed = 10f;
    public static float knockbackFactor = 1f;
    public static float shootOffset = 0.5f;

    public static string initialSeed = "";
    public static int timePassed = 0;
    public static int score = 0;


    public static void AddEnemy(EnemyAI obj)
    {
        enemies.Add(obj);
    }

    public static void RemoveEnemy(EnemyAI obj)
    {
        enemies.Remove(obj);
    }

    public static Vector3 GetDirVector(Direction dir)
    {
        switch (dir)
        {
        case Direction.UP:
            return Vector3.up;
        case Direction.DOWN:
            return Vector3.down;
        case Direction.LEFT:
            return Vector3.left;
        case Direction.RIGHT:
            return Vector3.right;
        default:
            return Vector3.zero;
        }
    }

    public static float GetAngleFromDir(Direction dir)
    {
        switch (dir)
        {
        case Direction.UP:
            return 0;
        case Direction.DOWN:
            return 180;
        case Direction.LEFT:
            return 90;
        case Direction.RIGHT:
            return -90;
        default:
            return 0;
        }
    }

}
