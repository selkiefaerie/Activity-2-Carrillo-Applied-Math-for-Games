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
        {
            Shoot();
        }

        creatureWasInSight = creatureInSight;
    }

    bool IsCreatureInSight()
    {
        CreatureHealth[] creatures = FindObjectsByType<CreatureHealth>();

        foreach (CreatureHealth creature in creatures)
        {
            if (creature == null)
                continue;

            Vector2 start = transform.position;
            Vector2 forward = transform.right.normalized;
            Vector2 creaturePosition = creature.transform.position;

            Vector2 toCreature = creaturePosition - start;

            float distanceAlongLine =
                Vector2.Dot(toCreature, forward);

            if (distanceAlongLine < 0)
                continue;

            if (distanceAlongLine > range)
                continue;

            Vector2 closestPoint =
                start + forward * distanceAlongLine;

            float distanceFromLine =
                Vector2.Distance(
                    creaturePosition,
                    closestPoint
                );

            if (distanceFromLine <= sightWidth)
                return true;
        }

        return false;
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
        {
            projectileScript.Setup(direction);
        }
    }

    void DrawSightLine()
    {
        if (lineRenderer == null)
            return;

        lineRenderer.positionCount = 2;

        Vector2 start = transform.position;

        Vector2 end =
            start + (Vector2)transform.right * range;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}