
using UnityEngine;

public class SniperTurret : TurretBase
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int projectileDamage = 1;

    [Header("Sniper Settings")]
    public float range = 7f;
    public float sightWidth = 0.5f;
    public float fireRate = 1.5f;

    [Header("Sound Effects")]
    public AudioClip shootingSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private float fireTimer;

    void Awake()
    {
        projectileDamage = GetDamageForLevel();
    }

    void Start()
    {
        fireTimer = 0f;
    }

    int GetDamageForLevel()
    {
        return upgradeLevel;
    }

    public override void Upgrade()
    {
        if (!CanUpgrade())
            return;

        base.Upgrade();
        projectileDamage = GetDamageForLevel();
    }

    void Update()
    {
        if (!turretActive)
            return;

        DrawSightLine();

        fireTimer -= Time.deltaTime;

        if (IsCreatureInSight() && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    bool IsCreatureInSight()
    {
        PathEnemy[] enemies =
            FindObjectsByType<PathEnemy>(
                FindObjectsSortMode.None
            );

        foreach (PathEnemy enemy in enemies)
        {
            if (enemy != null &&
                IsTargetInSight(enemy.transform.position))
            {
                return true;
            }
        }

        // Compatibility with older enemies using CreatureHealth.
        CreatureHealth[] creatures =
            FindObjectsByType<CreatureHealth>(
                FindObjectsSortMode.None
            );

        foreach (CreatureHealth creature in creatures)
        {
            if (creature != null &&
                IsTargetInSight(creature.transform.position))
            {
                return true;
            }
        }

        return false;
    }

    bool IsTargetInSight(Vector2 targetPosition)
    {
        Vector2 start = transform.position;
        Vector2 forward = transform.right.normalized;
        Vector2 toTarget = targetPosition - start;

        float distanceAlongLine =
            Vector2.Dot(toTarget, forward);

        if (distanceAlongLine < 0 ||
            distanceAlongLine > range)
        {
            return false;
        }

        Vector2 closestPoint =
            start + forward * distanceAlongLine;

        float distanceFromLine =
            Vector2.Distance(targetPosition, closestPoint);

        return distanceFromLine <= sightWidth;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "Sniper needs a Projectile Prefab and Fire Point!",
                this
            );
            return;
        }

        // Play the firing sound.
        if (shootingSound != null)
        {
            AudioSource.PlayClipAtPoint(
                shootingSound,
                transform.position,
                soundVolume
            );
        }

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
            projectileScript.Setup(
                direction,
                projectileDamage
            );
        }
        else
        {
            Debug.LogWarning(
                "Sniper projectile is missing TurretProjectile!",
                projectile
            );
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
