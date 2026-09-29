using UnityEngine;

public class FlameTurret : TurretBase
{
    public Transform player;

    public LineRenderer lineRenderer;

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

        if (IsPlayerInCone())
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

    bool IsPlayerInCone()
    {
        if (player == null)
            return false;

        Vector2 directionToPlayer =
            player.position - transform.position;

        float distance = directionToPlayer.magnitude;

        if (distance > range)
            return false;

        float angle = Vector2.Angle(
            transform.right,
            directionToPlayer
        );

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

        Vector2 direction =
            Quaternion.Euler(0f, 0f, randomAngle)
            * transform.right;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        TurretProjectile projectileScript =
            projectile.GetComponent<TurretProjectile>();

        projectileScript.Setup(direction, player);
    }

    void DrawCone()
    {
        if (lineRenderer == null)
            return;

        int points = 30;

        lineRenderer.positionCount = points + 2;

        lineRenderer.SetPosition(
            0,
            transform.position
        );

        for (int i = 0; i <= points; i++)
        {
            float angle =
                -coneAngle / 2f +
                (coneAngle / points) * i;

            Vector2 direction =
                Quaternion.Euler(0f, 0f, angle)
                * transform.right;

            Vector2 point =
                (Vector2)transform.position
                + direction * range;

            lineRenderer.SetPosition(
                i + 1,
                point
            );
        }

        lineRenderer.positionCount = points + 3;

        lineRenderer.SetPosition(
            points + 2,
            transform.position
        );
    }
}