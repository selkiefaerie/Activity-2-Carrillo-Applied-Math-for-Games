
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public PathEnemySpawner enemySpawner;
    public int totalWaves = 10;
    public float timeBetweenWaves = 5f;

    [Header("Victory Sound")]
    public AudioClip victorySound;
    [Range(0f, 1f)]
    public float victoryVolume = 1f;

    [Header("Win Screen")]
    public GameObject winPanel;

    private int currentWave;
    private float waveTimer;
    private bool waitingForNextWave = true;
    private bool waveInProgress;
    private bool gameWon;

    void Start()
    {
        Time.timeScale = 1f;
        gameWon = false;
        waveTimer = 2f;

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Update()
    {
        if (gameWon || !waitingForNextWave || waveInProgress)
            return;

        waveTimer -= Time.deltaTime;

        if (waveTimer <= 0f)
            StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWave >= totalWaves || enemySpawner == null)
            return;

        currentWave++;

        waveInProgress = true;
        waitingForNextWave = false;

        int slimes = 0;
        int bats = 0;
        int golems = 0;

        switch (currentWave)
        {
            case 1: slimes = 5; break;
            case 2: slimes = 7; break;
            case 3: slimes = 9; break;
            case 4: slimes = 11; break;
            case 5: slimes = 8; bats = 4; break;
            case 6: slimes = 9; bats = 6; break;
            case 7: slimes = 8; bats = 6; golems = 2; break;
            case 8: slimes = 10; bats = 7; golems = 3; break;
            case 9: slimes = 12; bats = 8; golems = 4; break;
            case 10: slimes = 14; bats = 10; golems = 5; break;
        }

        Debug.Log(
            "Wave " + currentWave +
            " started! Slimes: " + slimes +
            ", Bats: " + bats +
            ", Golems: " + golems
        );

        enemySpawner.StartWave(slimes, bats, golems);
    }

    public void FinishWave()
    {
        if (!waveInProgress || gameWon)
            return;

        waveInProgress = false;

        // Play a victory toot when a wave is completed.
        if (victorySound != null)
        {
            Camera mainCamera = Camera.main;

            Vector3 soundPosition = mainCamera != null
                ? mainCamera.transform.position
                : transform.position;

            AudioSource.PlayClipAtPoint(
                victorySound,
                soundPosition,
                victoryVolume
            );
        }

        if (currentWave >= totalWaves)
        {
            GameWon();
            return;
        }

        waitingForNextWave = true;
        waveTimer = timeBetweenWaves;
    }

    void GameWon()
    {
        if (gameWon)
            return;

        gameWon = true;

        if (winPanel != null)
            winPanel.SetActive(true);
        else
            Debug.LogWarning("Win Panel is not assigned!");

        Time.timeScale = 0f;

        Debug.Log("VICTORY! All waves completed!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
