
using UnityEngine;

public class ShotgunTurret : TurretBase
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Shotgun Settings")]
    public float range = 4f;
    public int bulletCount = 5;
    public float spreadAngle = 60f;
    public float fireRate = 1f;

    [Header("Sound Effects")]
    public AudioClip shootingSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private float fireTimer = 0f;
    private float baseRange;

    void Awake()
    {
        baseRange = range;
    }

    public override void Upgrade()
    {
        if (!CanUpgrade())
            return;

        base.Upgrade();

        range = baseRange + (upgradeLevel - 1) * 1f;
    }

    void Update()
    {
        if (!turretActive)
            return;

        DrawRange();

        Transform targetCreature = FindCreatureInRange();

        if (targetCreature != null)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireRate)
            {
                Shoot(targetCreature);
                fireTimer = 0f;
            }
        }
        else
        {
            fireTimer = fireRate;
        }
    }

    Transform FindCreatureInRange()
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        CreatureHealth[] creatures =
            FindObjectsByType<CreatureHealth>(
                FindObjectsSortMode.None
            );

        foreach (CreatureHealth creature in creatures)
        {
            if (creature == null)
                continue;

            CheckTarget(
                creature.transform,
                ref closestTarget,
                ref closestDistance
            );
        }

        PathEnemy[] enemies =
            FindObjectsByType<PathEnemy>(
                FindObjectsSortMode.None
            );

        foreach (PathEnemy enemy in enemies)
        {
            if (enemy == null)
                continue;

            CheckTarget(
                enemy.transform,
                ref closestTarget,
                ref closestDistance
            );
        }

        return closestTarget;
    }

    void CheckTarget(
        Transform target,
        ref Transform closestTarget,
        ref float closestDistance
    )
    {
        float distance = Vector2.Distance(
            transform.position,
            target.position
        );

        if (distance <= range && distance < closestDistance)
        {
            closestDistance = distance;
            closestTarget = target;
        }
    }

    void Shoot(Transform targetCreature)
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        if (targetCreature == null)
            return;

        // Play the shotgun firing sound once per shot.
        if (shootingSound != null)
        {
            AudioSource.PlayClipAtPoint(
                shootingSound,
                transform.position,
                soundVolume
            );
        }

        Vector2 directionToCreature =
            targetCreature.position - firePoint.position;

        directionToCreature.Normalize();

        float startAngle = -spreadAngle / 2f;

        float angleStep = bulletCount > 1
            ? spreadAngle / (bulletCount - 1)
            : 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector2 direction = Quaternion.Euler(
                0f,
                0f,
                angle
            ) * directionToCreature;

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
    }

    void DrawRange()
    {
        if (lineRenderer == null)
            return;

        int points = 60;
        lineRenderer.positionCount = points + 1;

        for (int i = 0; i <= points; i++)
        {
            float angle = (360f / points) * i;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector2 point =
                (Vector2)transform.position + direction * range;

            lineRenderer.SetPosition(i, point);
        }
    }
}
