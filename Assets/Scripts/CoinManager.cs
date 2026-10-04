using UnityEngine;
using TMPro;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public int coins = 100;
    public TMP_Text coinText;
    public RectTransform coinTarget;

    public float countSpeed = 10f;
    public float punchScale = 1.2f;
    public float punchDuration = 0.15f;

    private int displayedCoins;
    private Vector3 originalScale;
    private Coroutine countCoroutine;

    void Start()
    {
        displayedCoins = coins;

        if (coinText != null)
        {
            coinText.text = displayedCoins.ToString();
            originalScale = coinText.transform.localScale;
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;

        if (countCoroutine != null)
        {
            StopCoroutine(countCoroutine);
        }

        countCoroutine = StartCoroutine(AnimateCoins());
    }

    IEnumerator AnimateCoins()
    {
        while (displayedCoins < coins)
        {
            displayedCoins++;

            if (coinText != null)
            {
                coinText.text = displayedCoins.ToString();
                StartCoroutine(PunchText());
            }

            yield return new WaitForSeconds(1f / countSpeed);
        }

        displayedCoins = coins;

        if (coinText != null)
        {
            coinText.text = displayedCoins.ToString();
        }

        countCoroutine = null;
    }

    IEnumerator PunchText()
    {
        if (coinText == null)
            yield break;

        coinText.transform.localScale =
            originalScale * punchScale;

        float timer = 0f;

        while (timer < punchDuration)
        {
            timer += Time.deltaTime;

            float progress =
                timer / punchDuration;

            coinText.transform.localScale =
                Vector3.Lerp(
                    originalScale * punchScale,
                    originalScale,
                    progress
                );

            yield return null;
        }

        coinText.transform.localScale =
            originalScale;
    }
}