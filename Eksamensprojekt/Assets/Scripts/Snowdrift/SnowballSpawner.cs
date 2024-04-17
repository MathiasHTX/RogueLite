using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballSpawner : MonoBehaviour
{
    public GameObject snowballPrefab;
    public Transform[] snowballSpawnpoints;
    public float timeUntilStart = 2f;
    public float repeatEvery = 5f;
    public float launchForce = 10f;  // Adjust this as necessary

    void Start()
    {
        InvokeRepeating("SpawnSnowball", timeUntilStart, repeatEvery);
    }

    void SpawnSnowball()
    {
        float randomDirectionX = Random.Range(-0.17f, 0.17f);
        Vector3 originForce = new Vector3(randomDirectionX, 0f, 1f);    
        float size = Random.Range(0.135f, 0.3f) * 8f;

        int randomSpawnpoint = Random.Range(0, snowballSpawnpoints.Length);
        Vector3 spawnPosition = snowballSpawnpoints[randomSpawnpoint].position;

        GameObject spawnedSnowball = Instantiate(snowballPrefab, spawnPosition, Quaternion.identity);
        Destroy(spawnedSnowball, 15f);
        Snowball snowballScript = spawnedSnowball.GetComponent<Snowball>();
        if (snowballScript != null)
        {
            snowballScript.Initialize(originForce, size, launchForce);
        }
    }
}
