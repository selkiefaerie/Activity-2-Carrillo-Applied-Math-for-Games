using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 7f;
    public float lifetime = 5f;

    private Vector3 direction;

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            angle - 90f
        );
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}