using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 20;
    public int currentHP = 20;

    public Image realBar;
    public Image ghostBar;

    public float ghostDelay = 0.3f;
    public float ghostSpeed = 2f;

    private float ghostTimer;

    void Start()
    {
        currentHP = maxHP;

        if (realBar != null)
            realBar.fillAmount = 1f;

        if (ghostBar != null)
            ghostBar.fillAmount = 1f;
    }

    void Update()
    {
        if (ghostTimer > 0f)
        {
            ghostTimer -= Time.deltaTime;
            return;
        }

        if (ghostBar != null && realBar != null)
        {
            if (ghostBar.fillAmount > realBar.fillAmount)
            {
                ghostBar.fillAmount = Mathf.Lerp(
                    ghostBar.fillAmount,
                    realBar.fillAmount,
                    ghostSpeed * Time.deltaTime
                );
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP < 0)
            currentHP = 0;

        if (realBar != null)
        {
            realBar.fillAmount = (float)currentHP / maxHP;
        }

        ghostTimer = ghostDelay;

        Debug.Log("Player HP: " + currentHP + "/" + maxHP);
    }
}