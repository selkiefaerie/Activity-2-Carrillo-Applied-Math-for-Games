using UnityEngine;

public class CubicCreature : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform controlPoint1;
    public Transform controlPoint2;
    public Transform target;

    public float moveSpeed = 2f;

    private float t = 0f;

    void Update()
    {
        if (spawnPoint == null ||
            controlPoint1 == null ||
            controlPoint2 == null ||
            target == null)
            return;

        t += moveSpeed * Time.deltaTime / 10f;

        if (t >= 1f)
        {
            t = 1f;
            ReachTarget();
            return;
        }

        Vector2 p0 = spawnPoint.position;
        Vector2 p1 = controlPoint1.position;
        Vector2 p2 = controlPoint2.position;
        Vector2 p3 = target.position;

        Vector2 p01 = Vector2.Lerp(p0, p1, t);
        Vector2 p12 = Vector2.Lerp(p1, p2, t);
        Vector2 p23 = Vector2.Lerp(p2, p3, t);

        Vector2 p012 = Vector2.Lerp(p01, p12, t);
        Vector2 p123 = Vector2.Lerp(p12, p23, t);

        Vector2 position = Vector2.Lerp(p012, p123, t);

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