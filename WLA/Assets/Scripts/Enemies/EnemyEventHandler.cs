using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEventHandler : MonoBehaviour
{
    EnemyAI enemy;
    void Awake()
    {
        enemy = GetComponentInParent<EnemyAI>();
    }

    public void StartHit()
    {
        enemy?.StartStun();
    }

    public void StopHit()
    {
        enemy?.StopStun();
    }
}
