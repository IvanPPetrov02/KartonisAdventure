using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] private GameObject breakEffect;

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