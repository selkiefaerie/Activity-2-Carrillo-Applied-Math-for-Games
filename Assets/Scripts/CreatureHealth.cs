
using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    public int maxHP = 1;
    public int currentHP = 1;

    [Header("Gold Reward")]
    public int goldReward = 5;
    public GameObject coinPrefab;
    public CoinManager coinManager;

    private bool isDead;

    void Start()
    {
        currentHP = maxHP;

        if (coinManager == null)
            coinManager = FindFirstObjectByType<CoinManager>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHP -= damage;

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

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

        Destroy(gameObject);
    }
}
