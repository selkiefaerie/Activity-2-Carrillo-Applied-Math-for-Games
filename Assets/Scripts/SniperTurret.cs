using UnityEngine;

public class SniperTurret : TurretBase
{
    public Transform player;

    public LineRenderer lineRenderer;

    public GameObject projectilePrefab;
    public Transform firePoint;

    public float range = 7f;
    public float sightWidth = 0.5f;

    private bool playerWasInSight = false;

    void Update()
    {
        if (!turretActive)
            return;

        DrawSightLine();

        bool playerInSight = IsPlayerInSight();

        if (playerInSight && !playerWasInSight)
        {
            Shoot();
        }

        playerWasInSight = playerInSight;
    }

    bool IsPlayerInSight()
    {
        if (player == null)
            return false;

        Vector2 start = transform.position;

        Vector2 forward = transform.right.normalized;

        Vector2 playerPosition = player.position;

        Vector2 toPlayer = playerPosition - start;

        float distanceAlongLine =
            Vector2.Dot(toPlayer, forward);

        // Player is behind the turret
        if (distanceAlongLine < 0)
            return false;

        // Player is outside the sight range
        if (distanceAlongLine > range)
            return false;

        // Find how far the player is from the sight line
        Vector2 closestPoint =
            start + forward * distanceAlongLine;

        float distanceFromLine =
            Vector2.Distance(
                playerPosition,
                closestPoint
            );

        return distanceFromLine <= sightWidth;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        // Shoot in the exact same direction as the sight line
        Vector2 direction = transform.right.normalized;

        GameObject projectile = Instantiate(
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

    void DrawSightLine()
    {
        if (lineRenderer == null)
            return;

        lineRenderer.positionCount = 2;

        Vector2 start = transform.position;

        Vector2 end =
            start + (Vector2)transform.right * range;

        lineRenderer.SetPosition(
            0,
            start
        );

        lineRenderer.SetPosition(
            1,
            end
        );
    }
}