
using UnityEngine;

public class PathEnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] waypoints;
    public float spawnInterval = 3f;
    public WaveManager waveManager;

    private float spawnTimer;
    private int enemiesToSpawn;
    private int enemiesSpawned;
    private int activeEnemies;
    private bool waveActive;

    void Update()
    {
        if (!waveActive || enemyPrefab == null || waypoints == null || waypoints.Length == 0)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && enemiesSpawned < enemiesToSpawn)
        {
            SpawnEnemy();
            spawnTimer = 0f;
            enemiesSpawned++;
            activeEnemies++;

            if (enemiesSpawned >= enemiesToSpawn)
                waveActive = false;
        }
    }

    public void StartWave(int enemyCount)
    {
        enemiesToSpawn = enemyCount;
        enemiesSpawned = 0;
        activeEnemies = 0;
        spawnTimer = spawnInterval;
        waveActive = true;
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(
            enemyPrefab,
            waypoints[0].position,
            Quaternion.identity
        );

        PathEnemy pathEnemy = enemy.GetComponent<PathEnemy>();

        if (pathEnemy != null)
        {
            pathEnemy.waypoints = waypoints;
            pathEnemy.spawner = this;
        }
    }

    public void EnemyFinished()
    {
        activeEnemies--;

        if (activeEnemies <= 0 && enemiesSpawned >= enemiesToSpawn && !waveActive)
        {
            if (waveManager != null)
                waveManager.FinishWave();
        }
    }
}
