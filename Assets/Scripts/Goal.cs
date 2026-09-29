using UnityEngine;

public class Goal : MonoBehaviour
{
    public float goalDistance = 1f;

    void Update()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.transform.position
            );

        if (distance <= goalDistance)
        {
            LevelManager.Instance.WinGame();
        }
    }
}