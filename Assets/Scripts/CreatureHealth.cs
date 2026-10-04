using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    public int maxHP = 1;
    public int currentHP = 1;

    public GameObject coinPrefab;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
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