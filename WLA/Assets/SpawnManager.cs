using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public GameObject pickupPrefab;

    private DungeonGenerator dungeonGenerator;

    public void SetDungeonGenerator(DungeonGenerator dungeon)
    {
        dungeonGenerator = dungeon;
    }

    public GameObject GetEnemy(Vector2 pos)
    {
        int idx = dungeonGenerator.GenerateRandomInt(0, enemyPrefabs.Count);
        GameObject prefab = enemyPrefabs[idx];

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        return go;
    }


    public GameObject GetWeapon(Vector2 pos)
    {
        GameObject go = Instantiate(pickupPrefab, pos, Quaternion.identity);
        PickUpManager pickUpManager = go.GetComponent<PickUpManager>();
        int option = dungeonGenerator.GenerateRandomInt(0, 3);

        PickUpType type;
        int value;

        switch (option)
        {
            case 0:
                type = PickUpType.SWORD;
                value = dungeonGenerator.GenerateRandomInt(0, 3);
                break;
            case 1:
                type = PickUpType.BOW;
                value = dungeonGenerator.GenerateRandomInt(0, 3);
            break;
            case 2:
                type = PickUpType.MAGIC;
                value = dungeonGenerator.GenerateRandomInt(0, 3);
            break;
            default:
                type = PickUpType.SWORD;
                value = dungeonGenerator.GenerateRandomInt(0, 3);
                break;
        }

        pickUpManager.type = type;
        pickUpManager.value = value;
        return go;
    }

    public GameObject GetPickup(Vector2 pos)
    {
        GameObject go = Instantiate(pickupPrefab, pos, Quaternion.identity);
        PickUpManager pickUpManager = go.GetComponent<PickUpManager>();
        int option = dungeonGenerator.GenerateRandomInt(0, 3);

        PickUpType type;
        int value;

        switch (option)
        {
            case 0:
                type = PickUpType.HEALTH;
                value = dungeonGenerator.GenerateRandomInt(1, 10);
                value *= 10;
                break;
            case 1:
                type = PickUpType.MANA;
                value = dungeonGenerator.GenerateRandomInt(1, 10);
                value *= 10;
            break;
            case 2:
                type = PickUpType.ARROW;
                value = dungeonGenerator.GenerateRandomInt(1, 10);
                value *= 10;
            break;
            default:
                type = PickUpType.HEALTH;
                value = 0;
                break;
        }

        pickUpManager.type = type;
        pickUpManager.value = value;
        return go;
    }

}
