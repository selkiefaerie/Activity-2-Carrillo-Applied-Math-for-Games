using UnityEngine;

public class RocketBarrage : MonoBehaviour
{
    public GameObject rocketPrefab;

    public float fireInterval = 3f;
    public int rocketCount = 4;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            FireBarrage();
            timer = 0f;
        }
    }

    void FireBarrage()
    {
        float angleSpacing = 360f / rocketCount;
        float firstAngle = angleSpacing / 2f;

        for (int i = 0; i < rocketCount; i++)
        {
            float angle = firstAngle + (angleSpacing * i);

            float radians = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f
            );

            GameObject rocket = Instantiate(
                rocketPrefab,
                transform.position,
                Quaternion.identity
            );

            Rocket rocketScript = rocket.GetComponent<Rocket>();

            rocketScript.SetDirection(direction);
        }
    }
}