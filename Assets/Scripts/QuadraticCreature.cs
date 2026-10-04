using UnityEngine;

public class QuadraticCreature : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform controlPoint;
    public Transform target;

    public float moveSpeed = 2f;

    private float t = 0f;

    void Update()
    {
        if (spawnPoint == null || controlPoint == null || target == null)
            return;

        t += moveSpeed * Time.deltaTime / 10f;

        if (t >= 1f)
        {
            t = 1f;
            ReachTarget();
            return;
        }

        Vector2 p0 = spawnPoint.position;
        Vector2 p1 = controlPoint.position;
        Vector2 p2 = target.position;

        Vector2 p01 = Vector2.Lerp(p0, p1, t);
        Vector2 p12 = Vector2.Lerp(p1, p2, t);

        Vector2 position = Vector2.Lerp(p01, p12, t);

        transform.position = position;
    }

    void ReachTarget()
    {
        PlayerHP playerHP = FindFirstObjectByType<PlayerHP>();

        if (playerHP != null)
        {
            playerHP.TakeDamage(1);
        }

        Destroy(gameObject);
    }
}