using UnityEngine;

public class ShotgunTurret : TurretBase
{
    public LineRenderer lineRenderer;

    public GameObject projectilePrefab;
    public Transform firePoint;

    public float range = 4f;

    public int bulletCount = 5;
    public float spreadAngle = 60f;

    private bool creatureWasInRange = false;
    private CreatureHealth targetCreature;

    void Update()
    {
        if (!turretActive)
            return;

        DrawRange();

        targetCreature = FindCreatureInRange();

        bool creatureInRange = targetCreature != null;

        if (creatureInRange && !creatureWasInRange)
        {
            Shoot();
        }

        creatureWasInRange = creatureInRange;
    }

    CreatureHealth FindCreatureInRange()
    {
        CreatureHealth[] creatures = FindObjectsByType<CreatureHealth>();

        CreatureHealth closestCreature = null;
        float closestDistance = Mathf.Infinity;

        foreach (CreatureHealth creature in creatures)
        {
            if (creature == null)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    creature.transform.position
                );

            if (distance <= range && distance < closestDistance)
            {
                closestDistance = distance;
                closestCreature = creature;
            }
        }

        return closestCreature;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        if (targetCreature == null)
            return;

        Vector2 directionToCreature =
            targetCreature.transform.position - firePoint.position;

        directionToCreature.Normalize();

        float startAngle =
            -spreadAngle / 2f;

        float angleStep = 0f;

        if (bulletCount > 1)
        {
            angleStep =
                spreadAngle / (bulletCount - 1);
        }

        for (int i = 0; i < bulletCount; i++)
        {
            float angle =
                startAngle + angleStep * i;

            Vector2 direction =
                Quaternion.Euler(
                    0f,
                    0f,
                    angle
                ) * directionToCreature;

            GameObject projectile =
                Instantiate(
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
    }

    void DrawRange()
    {
        if (lineRenderer == null)
            return;

        int points = 60;

        lineRenderer.positionCount =
            points + 1;

        for (int i = 0; i <= points; i++)
        {
            float angle =
                (360f / points) * i;

            Vector2 direction =
                new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

            Vector2 point =
                (Vector2)transform.position
                + direction * range;

            lineRenderer.SetPosition(
                i,
                point
            );
        }
    }
}