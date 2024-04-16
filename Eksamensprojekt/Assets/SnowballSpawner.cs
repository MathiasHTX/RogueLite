using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballSpawner : MonoBehaviour
{
    public Transform[] snowballSpawnpoints;
    public GameObject snowball;

    private float size;
    private float pushForce;
    private float randomDirectionX;
    public float launchForce;

    private float timeUntilStart = 1f;
    private float repeatEvery = 5f;

    void Start()
    {
        InvokeRepeating("SpawnSnowball", timeUntilStart, repeatEvery);
    }

    void SpawnSnowball()
    {
        randomDirectionX = Random.Range(-0.25f, 0.25f);
        Vector3 originForce = new Vector3(randomDirectionX, 0f, 1f);
        size = Random.Range(0, 0.3f) * 15f; // Change 5f to alter the spawned snowball size

        int randomSpawnpoint = Random.Range(0, snowballSpawnpoints.Length);
        Vector3 spawnPosition = snowballSpawnpoints[randomSpawnpoint].position;

        Instantiate(snowball, spawnPosition, Quaternion.identity);
        snowball.transform.localScale = new Vector3(size, size, size);
        Rigidbody rb = snowball.GetComponent<Rigidbody>();
        rb.AddForce(originForce * launchForce, ForceMode.Impulse);
    }
}
