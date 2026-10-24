
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHP = 20;
    public int currentHP = 20;

    [Header("Health Bar")]
    public Image realBar;
    public Image ghostBar;

    public float ghostDelay = 0.3f;
    public float ghostSpeed = 2f;

    [Header("Damage Sound")]
    public AudioClip damageSound;
    [Range(0f, 1f)]
    public float damageVolume = 1f;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    private float ghostTimer;
    private bool gameOver;

    void Start()
    {
        Time.timeScale = 1f;
        gameOver = false;
        currentHP = maxHP;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (realBar != null)
            realBar.fillAmount = 1f;

        if (ghostBar != null)
            ghostBar.fillAmount = 1f;
    }

    void Update()
    {
        if (gameOver)
            return;

        if (ghostTimer > 0f)
        {
            ghostTimer -= Time.deltaTime;
            return;
        }

        if (ghostBar != null && realBar != null)
        {
            if (ghostBar.fillAmount > realBar.fillAmount)
            {
                ghostBar.fillAmount = Mathf.Lerp(
                    ghostBar.fillAmount,
                    realBar.fillAmount,
                    ghostSpeed * Time.deltaTime
                );
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (gameOver || damage <= 0)
            return;

        int previousHP = currentHP;

        currentHP = Mathf.Max(0, currentHP - damage);

        // Play the damage sound only when HP actually decreases.
        if (currentHP < previousHP && damageSound != null)
        {
            Camera mainCamera = Camera.main;

            Vector3 soundPosition = mainCamera != null
                ? mainCamera.transform.position
                : transform.position;

            AudioSource.PlayClipAtPoint(
                damageSound,
                soundPosition,
                damageVolume
            );
        }

        if (realBar != null)
        {
            realBar.fillAmount =
                (float)currentHP / maxHP;
        }

        ghostTimer = ghostDelay;

        Debug.Log("Player HP: " + currentHP + "/" + maxHP);

        if (currentHP <= 0)
            GameOver();
    }

    void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        else
            Debug.LogWarning("Game Over Panel is not assigned!");

        Time.timeScale = 0f;

        Debug.Log("GAME OVER!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
