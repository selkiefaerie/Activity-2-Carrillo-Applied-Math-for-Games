using UnityEngine;

public class ShotgunTurret : TurretBase
{
    public Transform player;

    public LineRenderer lineRenderer;

    public GameObject projectilePrefab;
    public Transform firePoint;

    public float range = 4f;

    public int bulletCount = 5;
    public float spreadAngle = 60f;

    private bool playerWasInRange = false;

    void Update()
    {
        if (!turretActive)
            return;

        DrawRange();

        bool playerInRange = IsPlayerInRange();

        if (playerInRange && !playerWasInRange)
        {
            Shoot();
        }

        playerWasInRange = playerInRange;
    }

    bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

        return distance <= range;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        Vector2 directionToPlayer =
            player.position - firePoint.position;

        directionToPlayer.Normalize();

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
                ) * directionToPlayer;

            GameObject projectile =
                Instantiate(
                    projectilePrefab,
                    firePoint.position,
                    Quaternion.identity
                );

            TurretProjectile projectileScript =
                projectile.GetComponent<TurretProjectile>();

            projectileScript.Setup(
                direction,
                player
            );
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