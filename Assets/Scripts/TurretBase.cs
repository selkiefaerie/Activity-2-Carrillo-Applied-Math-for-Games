using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    protected bool turretActive = true;

    public virtual void StopTurret()
    {
        turretActive = false;
    }
}