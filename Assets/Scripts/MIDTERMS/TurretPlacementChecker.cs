
using UnityEngine;

public class TurretPlacementChecker : MonoBehaviour
{
    public Texture2D roadMask;
    public SpriteRenderer backgroundRenderer;
    public float roadThreshold = 0.5f;
    public float turretClearance = 0.8f;

    public bool IsValidPlacement(Vector2 worldPosition)
    {
        if (roadMask == null || backgroundRenderer == null)
            return false;

        Bounds bounds = backgroundRenderer.bounds;

        if (worldPosition.x < bounds.min.x ||
            worldPosition.x > bounds.max.x ||
            worldPosition.y < bounds.min.y ||
            worldPosition.y > bounds.max.y)
        {
            return false;
        }

        float normalizedX =
            (worldPosition.x - bounds.min.x) / bounds.size.x;

        float normalizedY =
            (worldPosition.y - bounds.min.y) / bounds.size.y;

        int pixelX = Mathf.Clamp(
            Mathf.FloorToInt(normalizedX * roadMask.width),
            0,
            roadMask.width - 1
        );

        int pixelY = Mathf.Clamp(
            Mathf.FloorToInt(normalizedY * roadMask.height),
            0,
            roadMask.height - 1
        );

        Color pixel = roadMask.GetPixel(pixelX, pixelY);

        if (pixel.r >= roadThreshold)
            return false;

        TurretBase[] turrets =
            FindObjectsByType<TurretBase>(FindObjectsSortMode.None);

        foreach (TurretBase turret in turrets)
        {
            if (turret == null || !turret.gameObject.activeInHierarchy)
                continue;

            if (turret.transform.name.EndsWith("_Preview"))
                continue;

            if (turret.GetComponentInParent<TurretPlacementPreview>() != null)
                continue;

            if (turret.GetComponentInChildren<TurretPlacementPreview>() != null)
                continue;

            float distance = Vector2.Distance(
                worldPosition,
                turret.transform.position
            );

            if (distance < turretClearance)
                return false;
        }

        return true;
    }
}
