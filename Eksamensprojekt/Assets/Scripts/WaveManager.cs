using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    int waveCount = -1;
    bool waveComplete;
    public float timeBetweenWaves = 20;
    float timer;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;
    public int totalEnemies = 10;
    int enemiesSpawned = 0;
    [SerializeField] int enemiesKilled = 0;

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
        NewWave();
    }

    public void NewWave()
    {
        waveComplete = false;
        waveCount++;
        enemiesKilled = 0;
        enemiesSpawned = 0;
        float spawnInterval = 20f;
        StartCoroutine(SpawnEnemiesCoroutine(spawnInterval));
        UIManager.instance.UpdateEnemiesKilledUI(enemiesKilled, totalEnemies);
        UIManager.instance.NewWave(waveCount);
        UIManager.instance.OpenEnemiesKilled();
    }

    private IEnumerator SpawnEnemiesCoroutine(float spawnInterval)
    {
        while (enemiesSpawned < totalEnemies)
        {
            SpawnEnemies();
            Debug.Log(enemiesSpawned);
          
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    void SpawnEnemies(int amount = 5)
    {
        Debug.Log("Spawned " + amount);

        for(int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
            enemiesSpawned++;
        }
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        UIManager.instance.UpdateEnemiesKilledUI(enemiesKilled, totalEnemies);

        if (enemiesKilled >= totalEnemies && !waveComplete)
        {
            WaveComplete();
        }
    }

    void WaveComplete()
    {
        timer = timeBetweenWaves;
        waveComplete = true;
        UIManager.instance.WaveComplete();
        UIManager.instance.OpenTimeBeforeNewWave();
    }

    private void Update()
    {
        if(waveComplete == true)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                NewWave();
            }
        }
    }

    public int GetWaveCount()
    {
        return waveCount;
    }

    public float GetTimer()
    {
        return timer;
    }

}
