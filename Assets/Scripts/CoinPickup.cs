
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Coin Movement")]
    public float speed = 8f;

    [Header("Collection Sound")]
    public AudioClip collectionSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private CoinManager coinManager;
    private bool collected = false;

    void Start()
    {
        coinManager = FindFirstObjectByType<CoinManager>();
    }

    void Update()
    {
        if (collected)
            return;

        if (coinManager == null || coinManager.coinTarget == null)
            return;

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector2 screenPosition =
            RectTransformUtility.WorldToScreenPoint(
                null,
                coinManager.coinTarget.position
            );

        Vector3 targetPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    -mainCamera.transform.position.z
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
            collected = true;

            // Play the collection sound once.
            if (collectionSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    collectionSound,
                    mainCamera.transform.position,
                    soundVolume
                );
            }

            // Add the coin to the counter.
            coinManager.AddCoins(1);

            Destroy(gameObject);
        }
    }
}
