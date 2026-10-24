
using UnityEngine;

public class SniperTurret : TurretBase
{
    public LineRenderer lineRenderer;
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float range = 7f;
    public float sightWidth = 0.5f;

    private bool creatureWasInSight = false;

    void Update()
    {
        if (!turretActive)
            return;

        DrawSightLine();

        bool creatureInSight = IsCreatureInSight();

        if (creatureInSight && !creatureWasInSight)
            Shoot();

        creatureWasInSight = creatureInSight;
    }

    bool IsCreatureInSight()
    {
        CreatureHealth[] creatures =
            FindObjectsByType<CreatureHealth>(FindObjectsSortMode.None);

        foreach (CreatureHealth creature in creatures)
        {
            if (creature != null && IsTargetInSight(creature.transform.position))
                return true;
        }

        PathEnemy[] enemies =
            FindObjectsByType<PathEnemy>(FindObjectsSortMode.None);

        foreach (PathEnemy enemy in enemies)
        {
            if (enemy != null && IsTargetInSight(enemy.transform.position))
                return true;
        }

        return false;
    }

    bool IsTargetInSight(Vector2 targetPosition)
    {
        Vector2 start = transform.position;
        Vector2 forward = transform.right.normalized;
        Vector2 toTarget = targetPosition - start;

        float distanceAlongLine = Vector2.Dot(toTarget, forward);

        if (distanceAlongLine < 0 || distanceAlongLine > range)
            return false;

        Vector2 closestPoint = start + forward * distanceAlongLine;

        float distanceFromLine = Vector2.Distance(
            targetPosition,
            closestPoint
        );

        return distanceFromLine <= sightWidth;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        Vector2 direction = transform.right.normalized;

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

    void DrawSightLine()
    {
        if (lineRenderer == null)
            return;

        lineRenderer.positionCount = 2;

        Vector2 start = transform.position;
        Vector2 end = start + (Vector2)transform.right * range;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
