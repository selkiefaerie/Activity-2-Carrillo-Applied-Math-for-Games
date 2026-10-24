
using UnityEngine;

public class TurretPreviewRange : MonoBehaviour
{
    private FlameTurret flameTurret;
    private SniperTurret sniperTurret;
    private ShotgunTurret shotgunTurret;
    private LineRenderer rangeLine;

    void Awake()
    {
        flameTurret = GetComponentInChildren<FlameTurret>(true);
        sniperTurret = GetComponentInChildren<SniperTurret>(true);
        shotgunTurret = GetComponentInChildren<ShotgunTurret>(true);

        TurretBase turret = GetComponentInChildren<TurretBase>(true);

        if (turret != null)
            rangeLine = turret.lineRenderer;

        if (rangeLine == null)
            rangeLine = GetComponentInChildren<LineRenderer>(true);

        if (rangeLine != null)
        {
            rangeLine.enabled = true;
            rangeLine.useWorldSpace = true;
        }
    }

    void LateUpdate()
    {
        if (rangeLine == null)
            return;

        if (flameTurret != null)
            DrawCone();
        else if (sniperTurret != null)
            DrawSniperLine();
        else if (shotgunTurret != null)
            DrawCircle();
    }

    void DrawCone()
    {
        int points = 30;
        rangeLine.positionCount = points + 3;

        Vector2 origin = flameTurret.transform.position;
        rangeLine.SetPosition(0, origin);

        for (int i = 0; i <= points; i++)
        {
            float angle = -flameTurret.coneAngle / 2f
                + flameTurret.coneAngle / points * i;

            Vector2 direction = Quaternion.Euler(0f, 0f, angle)
                * flameTurret.transform.right;

            rangeLine.SetPosition(
                i + 1,
                origin + direction * flameTurret.range
            );
        }

        rangeLine.SetPosition(points + 2, origin);
    }

    void DrawSniperLine()
    {
        Vector2 origin = sniperTurret.transform.position;
        Vector2 end = origin
            + (Vector2)sniperTurret.transform.right * sniperTurret.range;

        rangeLine.positionCount = 2;
        rangeLine.SetPosition(0, origin);
        rangeLine.SetPosition(1, end);
    }

    void DrawCircle()
    {
        int points = 60;
        rangeLine.positionCount = points + 1;

        Vector2 origin = shotgunTurret.transform.position;

        for (int i = 0; i <= points; i++)
        {
            float angle = 360f * i / points;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            rangeLine.SetPosition(
                i,
                origin + direction * shotgunTurret.range
            );
        }
    }
}
