
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public PathEnemySpawner enemySpawner;
    public int totalWaves = 10;
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 5f;

    private int currentWave;
    private float waveTimer;
    private bool waitingForNextWave = true;
    private bool waveInProgress;

    void Start()
    {
        waveTimer = 2f;
    }

    void Update()
    {
        if (!waitingForNextWave || waveInProgress)
            return;

        waveTimer -= Time.deltaTime;

        if (waveTimer <= 0f)
        {
            StartNextWave();
        }
    }

    void StartNextWave()
    {
        if (currentWave >= totalWaves || enemySpawner == null)
            return;

        currentWave++;
        waveInProgress = true;
        waitingForNextWave = false;

        int enemyCount = enemiesPerWave + (currentWave - 1) * 2;

        enemySpawner.StartWave(enemyCount);
    }

    public void FinishWave()
    {
        waveInProgress = false;
        waitingForNextWave = true;
        waveTimer = timeBetweenWaves;
    }
}
