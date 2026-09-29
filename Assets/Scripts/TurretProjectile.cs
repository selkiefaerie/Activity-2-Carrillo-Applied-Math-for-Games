using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 5f;
    public float hitDistance = 0.4f;

    private Vector2 direction;
    private Transform player;

    public void Setup(Vector2 newDirection, Transform target)
    {
        direction = newDirection.normalized;
        player = target;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= hitDistance)
        {
            LevelManager.Instance.PlayerHit();
        }
    }
}