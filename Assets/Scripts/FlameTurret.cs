
using UnityEngine;

public class FlameTurret : TurretBase
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Flame Settings")]
    public float range = 5f;
    public float coneAngle = 45f;
    public float fireRate = 0.2f;

    [Header("Sound Effects")]
    public AudioClip shootingSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    public float fadeOutDuration = 0.3f;

    private float fireTimer = 0f;
    private float baseFireRate;

    private AudioSource flameAudioSource;
    private float fadeTimer;
    private bool fadingOut;

    void Awake()
    {
        baseFireRate = fireRate;

        // Create a dedicated audio source for the flame sound.
        flameAudioSource = gameObject.AddComponent<AudioSource>();
        flameAudioSource.playOnAwake = false;
        flameAudioSource.loop = true;
        flameAudioSource.spatialBlend = 0f;
        flameAudioSource.volume = soundVolume;

        if (shootingSound != null)
            flameAudioSource.clip = shootingSound;
    }

    public override void Upgrade()
    {
        if (!CanUpgrade())
            return;

        base.Upgrade();

        if (upgradeLevel == 2)
            fireRate = baseFireRate * 0.75f;
        else if (upgradeLevel == 3)
            fireRate = baseFireRate * 0.5f;
    }

    void Update()
    {
        if (!turretActive)
        {
            StopFlameSound();
            UpdateSoundFade();
            return;
        }

        DrawCone();

        if (IsCreatureInCone())
        {
            // Start or continue the flame sound.
            StartFlameSound();

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
            StopFlameSound();
        }

        UpdateSoundFade();
    }

    void StartFlameSound()
    {
        if (shootingSound == null || flameAudioSource == null)
            return;

        // Cancel any fade-out if firing resumes.
        fadingOut = false;
        fadeTimer = 0f;

        if (flameAudioSource.clip != shootingSound)
            flameAudioSource.clip = shootingSound;

        flameAudioSource.volume = soundVolume;

        // Only start playback if it isn't already playing.
        if (!flameAudioSource.isPlaying)
            flameAudioSource.Play();
    }

    void StopFlameSound()
    {
        if (flameAudioSource == null ||
            !flameAudioSource.isPlaying ||
            fadingOut)
            return;

        fadingOut = true;
        fadeTimer = fadeOutDuration;
    }

    void UpdateSoundFade()
    {
        if (!fadingOut || flameAudioSource == null)
            return;

        if (fadeOutDuration <= 0f)
        {
            flameAudioSource.Stop();
            flameAudioSource.volume = soundVolume;
            fadingOut = false;
            return;
        }

        fadeTimer -= Time.deltaTime;

        flameAudioSource.volume =
            soundVolume * Mathf.Clamp01(fadeTimer / fadeOutDuration);

        if (fadeTimer <= 0f)
        {
            flameAudioSource.Stop();
            flameAudioSource.volume = soundVolume;
            fadingOut = false;
        }
    }

    bool IsCreatureInCone()
    {
        CreatureHealth[] creatures =
            FindObjectsByType<CreatureHealth>(
                FindObjectsSortMode.None
            );

        foreach (CreatureHealth creature in creatures)
        {
            if (creature != null &&
                IsTargetInCone(creature.transform.position))
                return true;
        }

        PathEnemy[] enemies =
            FindObjectsByType<PathEnemy>(
                FindObjectsSortMode.None
            );

        foreach (PathEnemy enemy in enemies)
        {
            if (enemy != null &&
                IsTargetInCone(enemy.transform.position))
                return true;
        }

        return false;
    }

    bool IsTargetInCone(Vector2 targetPosition)
    {
        Vector2 offset =
            targetPosition - (Vector2)transform.position;

        float distance = offset.magnitude;

        if (distance > range)
            return false;

        float angle =
            Vector2.Angle(transform.right, offset.normalized);

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
            float angle =
                -coneAngle / 2f + (coneAngle / points) * i;

            Vector2 direction = Quaternion.Euler(
                0f,
                0f,
                angle
            ) * transform.right;

            Vector2 point =
                (Vector2)transform.position + direction * range;

            lineRenderer.SetPosition(i + 1, point);
        }

        lineRenderer.SetPosition(
            points + 2,
            transform.position
        );
    }

    void OnDisable()
    {
        if (flameAudioSource != null)
            flameAudioSource.Stop();
    }
}
