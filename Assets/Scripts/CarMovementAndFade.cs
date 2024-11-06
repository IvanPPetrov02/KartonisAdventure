using UnityEngine;

public class CarMovementAndFade : MonoBehaviour
{
    public float speed = 2f; // Speed at which the car moves
    public Vector2 direction = Vector2.up; // Direction of movement (up or down)
    public float fadeDistance = 10f; // Distance at which the car starts to fade
    public float fadeSpeed = 1f; // Speed of fading
    private SpriteRenderer spriteRenderer;
    private float initialAlpha;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialAlpha = spriteRenderer.color.a; // Save the initial alpha
        }
    }

    void Update()
    {
        // Move the car
        transform.Translate(direction * speed * Time.deltaTime);

        // Calculate fade based on distance from start point
        float distance = Vector3.Distance(transform.position, transform.position - new Vector3(direction.x, direction.y, 0) * fadeDistance);

        if (spriteRenderer != null)
        {
            // Calculate the new alpha based on the distance
            float newAlpha = Mathf.Lerp(initialAlpha, 0, 1 - (distance / fadeDistance));
            Color color = spriteRenderer.color;
            color.a = newAlpha;
            spriteRenderer.color = color;

            // Destroy the car if it has fully faded
            if (newAlpha <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
