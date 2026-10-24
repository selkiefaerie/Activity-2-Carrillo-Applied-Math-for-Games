
using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 5f;
    public float hitDistance = 0.4f;

    public int damage = 1;

    private Vector2 direction;

    public void Setup(Vector2 newDirection)
    {
        Setup(newDirection, 1);
    }

    public void Setup(Vector2 newDirection, int newDamage)
    {
        direction = newDirection.normalized;
        damage = Mathf.Max(1, newDamage);
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);

        CreatureHealth[] creatures =
            FindObjectsByType<CreatureHealth>(FindObjectsSortMode.None);

        foreach (CreatureHealth creature in creatures)
        {
            if (creature == null)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                creature.transform.position
            );

            if (distance <= hitDistance)
            {
                creature.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        PathEnemy[] enemies =
            FindObjectsByType<PathEnemy>(FindObjectsSortMode.None);

        foreach (PathEnemy enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance <= hitDistance)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
    }
}
