using System.Collections;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    [SerializeField] ShipLandingSequence shipLandingSequence;
    [SerializeField] PlayerMovementAdvanced playerMovementAdvanced;

    int waveCount = -1;
    bool waveComplete;
    public float timeBetweenWaves = 20;
    float timer;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;
    public int totalEnemies = 10;
    int enemiesSpawned = 0;
    [SerializeField] int enemiesKilled = 0;

    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip deathSound, gameOverSound, waveCompleteSound;

    int initialSlimePickUpAmount;
    int initialMushroomAmount;
    [SerializeField] ItemSO slimePickUpSO;
    [SerializeField] ItemSO mushroomSO;

    // Start is called before the first frame update
    void Start()
    {
        shipLandingSequence.OnExitShip += ShipLandingSequence_OnExitShip;
        playerMovementAdvanced.onDeath += PlayerMovementAdvanced_onDeath;

        initialSlimePickUpAmount = PlayerPrefs.GetInt(slimePickUpSO.itemName + "Amount");
        initialMushroomAmount = PlayerPrefs.GetInt(mushroomSO.itemName + "Amount");
    }

    private void ShipLandingSequence_OnExitShip()
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
        uIManager.UpdateEnemiesKilledUI(enemiesKilled, totalEnemies);
        uIManager.NewWave(waveCount);
        uIManager.OpenEnemiesKilled();
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
            int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
            enemiesSpawned++;
        }
    }

    public void EnemyKilled(Vector3 deathPosition)
    {
        enemiesKilled++;
        uIManager.UpdateEnemiesKilledUI(enemiesKilled, totalEnemies);
        AudioSource.PlayClipAtPoint(deathSound, deathPosition, 20);

        if (enemiesKilled >= totalEnemies && !waveComplete)
        {
            WaveComplete();
        }
    }

    void WaveComplete()
    {
        timer = timeBetweenWaves;
        waveComplete = true;
        uIManager.WaveComplete();
        uIManager.OpenTimeBeforeNewWave();
        audioSrc.PlayOneShot(waveCompleteSound);
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

    private void PlayerMovementAdvanced_onDeath()
    {
        audioSrc.PlayOneShot(gameOverSound, 0.5f);

        PlayerPrefs.SetInt(slimePickUpSO.itemName + "Amount", initialSlimePickUpAmount);
        PlayerPrefs.SetInt(mushroomSO.itemName + "Amount", initialMushroomAmount);
    }

    public int GetWaveCount()
    {
        return waveCount;
    }

    public float GetTimer()
    {
        return timer;
    }

    public bool IsWaveComplete()
    {
        return waveComplete;
    }

}
