using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;

    public List<GameObject> enemiesAlive = new List<GameObject>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies(int amount = 10)
    {
        for(int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
            enemiesAlive.Add(enemy);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemiesAlive.Remove(enemy);
        if(enemiesAlive.Count == 0)
        {
            Debug.Log("Wave completed");
        }
    }

}
