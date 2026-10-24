
using UnityEngine;

public class PathEnemy : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;

    [Header("Enemy Health")]
    public int maxHP = 3;
    public int currentHP = 3;

    [Header("Base Damage")]
    public int damageToBase = 1;
    public PlayerHP playerHP;

    [Header("Gold Reward")]
    public int goldReward = 5;
    public GameObject coinPrefab;
    public CoinManager coinManager;

    [Header("Spawner")]
    public PathEnemySpawner spawner;

    private int waypointIndex;
    private bool finished;

    void Start()
    {
        currentHP = maxHP;

        if (playerHP == null)
            playerHP = FindFirstObjectByType<PlayerHP>();

        if (coinManager == null)
            coinManager = FindFirstObjectByType<CoinManager>();
    }

    void Update()
    {
        if (finished || waypoints == null || waypoints.Length == 0)
            return;

        Transform targetWaypoint = waypoints[waypointIndex];

        if (targetWaypoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(
            transform.position,
            targetWaypoint.position
        ) < 0.05f)
        {
            waypointIndex++;

            if (waypointIndex >= waypoints.Length)
                ReachBase();
        }
    }

    public void TakeDamage(int damage)
    {
        if (finished)
            return;

        currentHP -= damage;

        if (currentHP <= 0)
            Die();
    }

    void ReachBase()
    {
        if (finished)
            return;

        if (playerHP != null)
        {
            playerHP.TakeDamage(damageToBase);
        }
        else
        {
            Debug.LogWarning(
                "No PlayerHP found! Assign PlayerHP to the enemy prefab."
            );
        }

        FinishEnemy(false);
    }

    void Die()
    {
        FinishEnemy(true);
    }

    void FinishEnemy(bool defeated)
    {
        if (finished)
            return;

        finished = true;

        // Only defeated enemies award gold.
        if (defeated)
        {
            if (coinManager != null)
                coinManager.AddCoins(goldReward);

            if (coinPrefab != null)
            {
                Instantiate(
                    coinPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
        }

        if (spawner != null)
            spawner.EnemyFinished();

        Destroy(gameObject);
    }
}
