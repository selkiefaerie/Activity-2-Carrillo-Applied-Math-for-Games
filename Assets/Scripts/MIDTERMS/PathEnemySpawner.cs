
using UnityEngine;

public class PathEnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemyPrefab; // Slime
    public GameObject batPrefab;
    public GameObject golemPrefab;

    [Header("Path")]
    public Transform[] waypoints;

    [Header("Spawning")]
    public float spawnInterval = 3f;

    [Header("Wave Manager")]
    public WaveManager waveManager;

    private float spawnTimer;
    private int slimesRemaining;
    private int batsRemaining;
    private int golemsRemaining;
    private int enemiesSpawned;
    private int totalEnemiesToSpawn;
    private int activeEnemies;
    private bool waveActive;

    void Update()
    {
        if (!waveActive ||
            waypoints == null ||
            waypoints.Length == 0)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval &&
            enemiesSpawned < totalEnemiesToSpawn)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    public void StartWave(int slimes, int bats, int golems)
    {
        slimesRemaining = slimes;
        batsRemaining = bats;
        golemsRemaining = golems;

        enemiesSpawned = 0;
        activeEnemies = 0;

        totalEnemiesToSpawn = slimes + bats + golems;
        spawnTimer = spawnInterval;
        waveActive = totalEnemiesToSpawn > 0;

        if (!waveActive && waveManager != null)
            waveManager.FinishWave();
    }

    void SpawnEnemy()
    {
        GameObject chosenPrefab = ChooseEnemyPrefab();

        if (chosenPrefab == null)
        {
            Debug.LogError("A required enemy prefab is missing!");
            return;
        }

        GameObject enemy = Instantiate(
            chosenPrefab,
            waypoints[0].position,
            Quaternion.identity
        );

        PathEnemy pathEnemy = enemy.GetComponent<PathEnemy>();

        if (pathEnemy != null)
        {
            pathEnemy.waypoints = waypoints;
            pathEnemy.spawner = this;
        }
        else
        {
            Debug.LogError(
                chosenPrefab.name + " is missing PathEnemy!"
            );
        }

        enemiesSpawned++;
        activeEnemies++;

        if (enemiesSpawned >= totalEnemiesToSpawn)
            waveActive = false;
    }

    GameObject ChooseEnemyPrefab()
    {
        int availableTypes = 0;

        if (slimesRemaining > 0) availableTypes++;
        if (batsRemaining > 0) availableTypes++;
        if (golemsRemaining > 0) availableTypes++;

        if (availableTypes == 0)
            return null;

        int choice = Random.Range(0, availableTypes);

        if (slimesRemaining > 0)
        {
            if (choice == 0)
            {
                slimesRemaining--;
                return enemyPrefab;
            }

            choice--;
        }

        if (batsRemaining > 0)
        {
            if (choice == 0)
            {
                batsRemaining--;
                return batPrefab;
            }

            choice--;
        }

        if (golemsRemaining > 0)
        {
            golemsRemaining--;
            return golemPrefab;
        }

        return null;
    }

    public void EnemyFinished()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);

        if (activeEnemies == 0 &&
            enemiesSpawned >= totalEnemiesToSpawn &&
            !waveActive)
        {
            if (waveManager != null)
                waveManager.FinishWave();
        }
    }
}
