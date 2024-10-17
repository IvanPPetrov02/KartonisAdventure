using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;  // Speed at which the camera follows the player
    public Transform target;        // Reference to the player or target to follow

    // LateUpdate is called once per frame after all Update functions have been called
    void LateUpdate()
    {
        // Make sure the target is assigned
        if (target != null)
        {
            // Calculate the desired camera position exactly at the player's position, with a fixed Z value
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -10f);

            // Smoothly move the camera towards the target position using Lerp
            transform.position = Vector3.Lerp(transform.position, targetPosition, FollowSpeed * Time.deltaTime);
        }
    }
}