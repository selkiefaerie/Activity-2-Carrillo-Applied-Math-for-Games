using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 5f;
    public float hitDistance = 0.4f;

    private Vector2 direction;

    public void Setup(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move projectile manually
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Find creatures
        CreatureHealth[] creatures = FindObjectsByType<CreatureHealth>(
            FindObjectsSortMode.None
        );

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
                creature.TakeDamage(1);
                Destroy(gameObject);
                return;
            }
        }
    }
}