
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;

public class TurretSelectionManager : MonoBehaviour
{
    public Camera mainCamera;
    public CoinManager coinManager;
    public GameObject managementPanel;

    public TMP_Text turretNameText;
    public TMP_Text turretLevelText;
    public TMP_Text turretStatsText;
    public TMP_Text upgradeButtonText;

    public Button upgradeButton;
    public Button sellButton;
    public Button closeButton;

    public int sellRefundPercent = 50;

    private TurretBase selectedTurret;

    void Start()
    {
        if (managementPanel != null)
            managementPanel.SetActive(false);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeSelectedTurret);

        if (sellButton != null)
            sellButton.onClick.AddListener(SellSelectedTurret);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (managementPanel != null &&
                managementPanel.activeInHierarchy &&
                RectTransformUtility.RectangleContainsScreenPoint(
                    managementPanel.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    null))
            {
                return;
            }

            if (EventSystemIsOverUI())
                return;

            SelectTurretAtMouse();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            ClosePanel();
    }

    bool EventSystemIsOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current != null &&
               UnityEngine.EventSystems.EventSystem.current
                   .IsPointerOverGameObject();
    }

    void SelectTurretAtMouse()
    {
        if (mainCamera == null)
            return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                -mainCamera.transform.position.z
            )
        );

        Collider2D hit = Physics2D.OverlapPoint(
            new Vector2(mousePosition.x, mousePosition.y)
        );

        if (hit == null)
        {
            ClosePanel();
            return;
        }

        TurretBase turret = hit.GetComponentInParent<TurretBase>();

        if (turret == null)
        {
            ClosePanel();
            return;
        }

        SelectTurret(turret);
    }

    void SelectTurret(TurretBase turret)
    {
        selectedTurret = turret;

        if (managementPanel != null)
            managementPanel.SetActive(true);

        RefreshPanel();
    }

    void RefreshPanel()
    {
        if (selectedTurret == null)
            return;

        if (turretNameText != null)
            turretNameText.text = selectedTurret.turretName;

        if (turretLevelText != null)
        {
            turretLevelText.text =
                "Level " + selectedTurret.upgradeLevel +
                " / " + selectedTurret.maxUpgradeLevel;
        }

        if (upgradeButtonText != null)
        {
            upgradeButtonText.text = selectedTurret.CanUpgrade()
                ? "UPGRADE - " + selectedTurret.upgradeCost + " COINS"
                : "MAX LEVEL";
        }

        if (turretStatsText != null)
            turretStatsText.text = GetTurretStats();

        if (upgradeButton != null)
            upgradeButton.interactable = selectedTurret.CanUpgrade();

        if (sellButton != null)
            sellButton.interactable = true;
    }

    string GetTurretStats()
    {
        if (selectedTurret is FlameTurret flame)
        {
            return
                "Range: " + flame.range.ToString("F1") +
                "\nCone Angle: " + flame.coneAngle.ToString("F0") + " degrees" +
                "\nFire Interval: " + flame.fireRate.ToString("F2") + " sec" +
                "\nAttack Rate: " +
                (flame.fireRate > 0 ? (1f / flame.fireRate).ToString("F1") : "N/A") +
                " attacks/sec" +
                GetOptionalStats(selectedTurret);
        }

        if (selectedTurret is SniperTurret sniper)
        {
            return
                "Range: " + sniper.range.ToString("F1") +
                "\nSight Width: " + sniper.sightWidth.ToString("F2") +
                "\nAttack Trigger: When a target enters sight" +
                GetOptionalStats(selectedTurret);
        }

        if (selectedTurret is ShotgunTurret shotgun)
        {
            return
                "Range: " + shotgun.range.ToString("F1") +
                GetOptionalStats(selectedTurret);
        }

        return "No stats available for this turret.";
    }

    string GetOptionalStats(TurretBase turret)
    {
        string result = "";

        string[] damageNames =
        {
            "projectileDamage", "damage", "baseDamage"
        };

        string[] rateNames =
        {
            "fireRate", "attackRate", "shotsPerSecond"
        };

        object script = turret;

        foreach (MonoBehaviour component in
                 turret.GetComponents<MonoBehaviour>())
        {
            if (component is FlameTurret ||
                component is SniperTurret ||
                component is ShotgunTurret)
            {
                script = component;
                break;
            }
        }

        System.Type type = script.GetType();

        foreach (string fieldName in damageNames)
        {
            FieldInfo field = type.GetField(
                fieldName,
                BindingFlags.Public | BindingFlags.Instance
            );

            if (field != null &&
                field.FieldType == typeof(float))
            {
                result += "\nDamage: " + field.GetValue(script);
                break;
            }
        }

        foreach (string fieldName in rateNames)
        {
            FieldInfo field = type.GetField(
                fieldName,
                BindingFlags.Public | BindingFlags.Instance
            );

            if (field != null &&
                field.FieldType == typeof(float) &&
                fieldName != "fireRate")
            {
                result += "\nAttack Rate: " + field.GetValue(script);
                break;
            }
        }

        return result;
    }

    void UpgradeSelectedTurret()
    {
        if (selectedTurret == null || coinManager == null)
            return;

        if (!selectedTurret.CanUpgrade())
            return;

        if (coinManager.coins < selectedTurret.upgradeCost)
        {
            Debug.Log("Not enough coins to upgrade this turret.");
            return;
        }

        int cost = selectedTurret.upgradeCost;

        coinManager.AddCoins(-cost);
        selectedTurret.Upgrade();

        RefreshPanel();
    }

    void SellSelectedTurret()
    {
        if (selectedTurret == null || coinManager == null)
            return;

        int refund = Mathf.RoundToInt(
            selectedTurret.sellValue * sellRefundPercent / 100f
        );

        coinManager.AddCoins(refund);

        Destroy(selectedTurret.gameObject);
        selectedTurret = null;

        ClosePanel();
    }

    void ClosePanel()
    {
        selectedTurret = null;

        if (managementPanel != null)
            managementPanel.SetActive(false);
    }
}
