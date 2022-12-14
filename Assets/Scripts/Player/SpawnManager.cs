using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : SingletonForMonobehaviour<SpawnManager>
{
    SpawnPoint[] spawnpoints;

    private void Awake()
    {
        spawnpoints = GetComponentsInChildren<SpawnPoint>();

    }

    public Transform GetSpawnpoint() 
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}
