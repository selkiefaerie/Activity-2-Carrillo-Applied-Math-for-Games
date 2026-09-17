using UnityEngine;

public class RocketPowerUp : MonoBehaviour
{
    public int rocketIncrease = 1;
    public int maxRocketCount = 8;
    public float pickupDistance = 1f;

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.transform.position
        );

        if (distance <= pickupDistance)
        {
            RocketBarrage barrage = player.GetComponent<RocketBarrage>();

            if (barrage != null)
            {
                barrage.rocketCount = Mathf.Min(
                    barrage.rocketCount + rocketIncrease,
                    maxRocketCount
                );

                Destroy(gameObject);
            }
        }
    }
}