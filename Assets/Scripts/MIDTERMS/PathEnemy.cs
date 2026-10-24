
using UnityEngine;

public class PathEnemy : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public int maxHP = 3;
    public int currentHP = 3;

    public PathEnemySpawner spawner;

    private int waypointIndex;
    private bool finished;

    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        if (finished || waypoints == null || waypoints.Length == 0)
            return;

        Transform targetWaypoint = waypoints[waypointIndex];

        if (targetWaypoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.05f)
        {
            waypointIndex++;

            if (waypointIndex >= waypoints.Length)
                ReachBase();
        }
    }

    public void TakeDamage(int damage)
    {
        if (finished)
            return;

        currentHP -= damage;

        if (currentHP <= 0)
            FinishEnemy();
    }

    void ReachBase()
    {
        FinishEnemy();
    }

    void FinishEnemy()
    {
        if (finished)
            return;

        finished = true;

        if (spawner != null)
            spawner.EnemyFinished();

        Destroy(gameObject);
    }
}
