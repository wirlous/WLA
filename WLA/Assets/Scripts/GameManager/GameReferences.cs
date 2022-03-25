﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameReferences
{
    public static PlayerController player;
    public static List<EnemyAI> enemies = new List<EnemyAI>();


    public static void AddEnemy(EnemyAI obj)
    {
        enemies.Add(obj);
    }

    public static void RemoveEnemy(EnemyAI obj)
    {
        enemies.Remove(obj);
    }

}
