using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int quantityToSpawn;

    [HideInInspector]
    public int spawnedCount = 0;
}