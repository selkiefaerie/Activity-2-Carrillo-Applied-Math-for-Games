
using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    protected bool turretActive = true;

    public LineRenderer lineRenderer;

    public string turretName = "Turret";
    [TextArea]
    public string turretDescription = "A defensive tower.";

    public int upgradeCost = 20;
    public int sellValue = 10;
    public int upgradeLevel = 1;
    public int maxUpgradeLevel = 3;

    public virtual void StopTurret()
    {
        turretActive = false;
    }

    public void SetRangeVisible(bool visible)
    {
        if (lineRenderer != null)
            lineRenderer.enabled = visible;
    }

    public virtual bool CanUpgrade()
    {
        return upgradeLevel < maxUpgradeLevel;
    }

    public virtual void Upgrade()
    {
        if (!CanUpgrade())
            return;

        upgradeLevel++;
        upgradeCost += 10;
        sellValue += 5;
    }
}
