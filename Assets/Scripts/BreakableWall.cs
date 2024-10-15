using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] private GameObject breakEffect; // The effect that plays when the wall is broken

    public void Break()
    {
        // Instantiate break effect if it exists
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        // Destroy the wall
        Destroy(gameObject);
    }
}