
using UnityEngine;

public class TurretPlacementPreview : MonoBehaviour
{
    public TurretPlacementChecker placementChecker;
    public CoinManager coinManager;
    public Camera mainCamera;

    public GameObject flameTurretPrefab;
    public GameObject sniperTurretPrefab;
    public GameObject shotgunTurretPrefab;

    public int flameCost = 10;
    public int sniperCost = 15;
    public int shotgunCost = 20;

    public float shotgunRotationStep = 15f;

    private GameObject previewObject;
    private SpriteRenderer[] previewRenderers;
    private GameObject selectedPrefab;

    private int selectedCost;
    private bool placing;
    private bool cardinalRotation;
    private float currentRotation;

    public void SelectFlame()
    {
        StartPlacement(flameTurretPrefab, flameCost, true);
    }

    public void SelectSniper()
    {
        StartPlacement(sniperTurretPrefab, sniperCost, true);
    }

    public void SelectShotgun()
    {
        StartPlacement(shotgunTurretPrefab, shotgunCost, false);
    }

    void Update()
    {
        if (!placing || previewObject == null)
            return;

        if (mainCamera == null ||
            placementChecker == null ||
            coinManager == null)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (cardinalRotation)
                currentRotation = (currentRotation + 90f) % 360f;
            else
                currentRotation =
                    (currentRotation + shotgunRotationStep) % 360f;

            previewObject.transform.rotation =
                Quaternion.Euler(0f, 0f, currentRotation);
        }

        // Convert the mouse position to world coordinates.
        Vector3 screenPosition = Input.mousePosition;

        screenPosition.z =
            Mathf.Abs(mainCamera.transform.position.z);

        Vector3 mousePosition =
            mainCamera.ScreenToWorldPoint(screenPosition);

        Vector2 placementPosition =
            new Vector2(mousePosition.x, mousePosition.y);

        // Move the preview to the mouse.
        previewObject.transform.position = new Vector3(
            placementPosition.x,
            placementPosition.y,
            0f
        );

        // Check the map and turret spacing.
        bool valid = placementChecker.IsValidPlacement(
            placementPosition
        );

        bool canAfford = coinManager.coins >= selectedCost;
        bool canPlace = valid && canAfford;

        foreach (SpriteRenderer sprite in previewRenderers)
        {
            if (sprite == null)
                continue;

            sprite.color = canPlace
                ? new Color(0.4f, 1f, 0.4f, 0.6f)
                : new Color(1f, 0.3f, 0.3f, 0.6f);
        }

        // Place the turret.
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            GameObject placedTurret = Instantiate(
                selectedPrefab,
                previewObject.transform.position,
                previewObject.transform.rotation
            );

            TurretBase turret =
                placedTurret.GetComponentInChildren<TurretBase>();

            if (turret != null)
                turret.SetRangeVisible(false);

            coinManager.AddCoins(-selectedCost);

            CancelPlacement();
            return;
        }

        // Cancel with right-click or Escape.
        if (Input.GetMouseButtonDown(1) ||
            Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    public void StartPlacement(
        GameObject prefab,
        int cost,
        bool useCardinalRotation
    )
    {
        if (prefab == null ||
            placementChecker == null ||
            coinManager == null ||
            mainCamera == null)
        {
            Debug.LogWarning(
                "Assign the turret prefabs, Placement Checker, " +
                "Coin Manager, and Main Camera."
            );

            return;
        }

        if (coinManager.coins < cost)
        {
            Debug.Log("Not enough coins to buy this turret.");
            CancelPlacement();
            return;
        }

        CancelPlacement();

        selectedPrefab = prefab;
        selectedCost = cost;
        cardinalRotation = useCardinalRotation;
        currentRotation = 0f;

        previewObject = Instantiate(prefab);
        previewObject.name = prefab.name + "_Preview";

        previewObject.transform.rotation = Quaternion.identity;

        // Disable turret attack scripts during preview.
        MonoBehaviour[] scripts =
            previewObject.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour script in scripts)
        {
            if (script == null)
                continue;

            if (script is FlameTurret ||
                script is SniperTurret ||
                script is ShotgunTurret)
            {
                script.enabled = false;
            }
            else if (!(script is TurretBase))
            {
                script.enabled = false;
            }
        }

        // Show the range during placement.
        TurretBase previewTurret =
            previewObject.GetComponentInChildren<TurretBase>(true);

        if (previewTurret != null)
            previewTurret.SetRangeVisible(true);

        LineRenderer[] lineRenderers =
            previewObject.GetComponentsInChildren<LineRenderer>(true);

        foreach (LineRenderer line in lineRenderers)
        {
            if (line != null)
                line.enabled = true;
        }

        TurretPreviewRange previewRange =
            previewObject.GetComponent<TurretPreviewRange>();

        if (previewRange == null)
            previewRange = previewObject.AddComponent<TurretPreviewRange>();

        // Disable colliders on the preview.
        Collider2D[] colliders =
            previewObject.GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D item in colliders)
            item.enabled = false;

        // Cache sprite renderers for valid/invalid coloring.
        previewRenderers =
            previewObject.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sprite in previewRenderers)
        {
            if (sprite != null)
                sprite.color = new Color(0.4f, 1f, 0.4f, 0.6f);
        }

        placing = true;
    }

    void CancelPlacement()
    {
        placing = false;

        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        previewRenderers = null;
        selectedPrefab = null;
        selectedCost = 0;
        currentRotation = 0f;
    }
}
