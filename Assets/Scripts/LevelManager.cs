using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject winUI;

    private bool gameWon;

    void Awake()
    {
        Instance = this;
    }

    public void PlayerHit()
    {
        if (gameWon)
            return;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void WinGame()
    {
        if (gameWon)
            return;

        gameWon = true;

        TurretBase[] turrets = FindObjectsByType<TurretBase>(
            FindObjectsSortMode.None
        );

        foreach (TurretBase turret in turrets)
        {
            turret.StopTurret();
        }

        if (winUI != null)
        {
            winUI.SetActive(true);
        }
    }
}