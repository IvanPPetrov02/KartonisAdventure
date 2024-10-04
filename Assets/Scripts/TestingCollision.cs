using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Triggered with: " + other.gameObject.name);
}

}
