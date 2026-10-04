using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public float speed = 8f;

    private CoinManager coinManager;

    void Start()
    {
        coinManager = FindFirstObjectByType<CoinManager>();
    }

    void Update()
    {
        if (coinManager == null || coinManager.coinTarget == null)
            return;

        Vector2 screenPosition =
            RectTransformUtility.WorldToScreenPoint(
                null,
                coinManager.coinTarget.position
            );

        Vector3 targetPosition =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    -Camera.main.transform.position.z
                )
            );

        targetPosition.z = transform.position.z;

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

        if (Vector2.Distance(
            transform.position,
            targetPosition
        ) < 0.1f)
        {
            coinManager.AddCoins(1);
            Destroy(gameObject);
        }
    }
}