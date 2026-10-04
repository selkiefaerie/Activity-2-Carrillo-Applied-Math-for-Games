using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public GameObject creaturePrefab;
    public Transform spawnPoint;
    public Transform controlPoint;
    public Transform controlPoint2;
    public Transform target;

    public float spawnInterval = 3f;

    private float spawnTimer = 0f;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnCreature();
            spawnTimer = 0f;
        }
    }

    void SpawnCreature()
    {
        if (creaturePrefab == null)
            return;

        GameObject creature = Instantiate(
            creaturePrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        QuadraticCreature quadratic =
            creature.GetComponent<QuadraticCreature>();

        if (quadratic != null)
        {
            quadratic.spawnPoint = spawnPoint;
            quadratic.controlPoint = controlPoint;
            quadratic.target = target;
        }

        CubicCreature cubic =
            creature.GetComponent<CubicCreature>();

        if (cubic != null)
        {
            cubic.spawnPoint = spawnPoint;
            cubic.controlPoint1 = controlPoint;
            cubic.controlPoint2 = controlPoint2;
            cubic.target = target;
        }
    }
}