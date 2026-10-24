
using UnityEngine;

public class FlameTurret : TurretBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float range = 5f;
    public float coneAngle = 45f;
    public float fireRate = 0.2f;

    private float fireTimer = 0f;

    void Update()
    {
        if (!turretActive)
            return;

        DrawCone();

        if (IsCreatureInCone())
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireRate)
            {
                Shoot();
                fireTimer = 0f;
            }
        }
        else
        {
            fireTimer = 0f;
        }
    }

    bool IsCreatureInCone()
    {
        CreatureHealth[] creatures =
            FindObjectsByType<CreatureHealth>(FindObjectsSortMode.None);

        foreach (CreatureHealth creature in creatures)
        {
            if (creature != null && IsTargetInCone(creature.transform.position))
                return true;
        }

        PathEnemy[] enemies =
            FindObjectsByType<PathEnemy>(FindObjectsSortMode.None);

        foreach (PathEnemy enemy in enemies)
        {
            if (enemy != null && IsTargetInCone(enemy.transform.position))
                return true;
        }

        return false;
    }

    bool IsTargetInCone(Vector2 targetPosition)
    {
        Vector2 offset = targetPosition - (Vector2)transform.position;
        float distance = offset.magnitude;

        if (distance > range)
            return false;

        float angle = Vector2.Angle(transform.right, offset.normalized);

        return angle <= coneAngle / 2f;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        float randomAngle = Random.Range(
            -coneAngle / 2f,
            coneAngle / 2f
        );

        Vector2 direction = Quaternion.Euler(
            0f,
            0f,
            randomAngle
        ) * transform.right;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        TurretProjectile projectileScript =
            projectile.GetComponent<TurretProjectile>();

        if (projectileScript != null)
            projectileScript.Setup(direction);
    }

    void DrawCone()
    {
        if (lineRenderer == null)
            return;

        int points = 30;
        lineRenderer.positionCount = points + 3;

        lineRenderer.SetPosition(0, transform.position);

        for (int i = 0; i <= points; i++)
        {
            float angle = -coneAngle / 2f + (coneAngle / points) * i;

            Vector2 direction = Quaternion.Euler(
                0f,
                0f,
                angle
            ) * transform.right;

            Vector2 point = (Vector2)transform.position + direction * range;

            lineRenderer.SetPosition(i + 1, point);
        }

        lineRenderer.SetPosition(points + 2, transform.position);
    }
}
