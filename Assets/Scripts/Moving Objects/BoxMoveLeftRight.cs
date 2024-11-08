using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMoveLeftRight : MonoBehaviour
{
    public float speed = 5f; // Speed of the movement
    public float distance = 3f; // Distance to travel before reversing direction

    private Vector2 startPos;
    private bool movingRight = true;

    void Start()
    {
        // Record the starting position of the box
        startPos = transform.position;
    }

    void Update()
    {
        // Move the box automatically left and right
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        // Reverse direction when reaching the desired distance
        if (Vector2.Distance(startPos, transform.position) >= distance)
        {
            movingRight = !movingRight;
            startPos = transform.position;
        }
    }
}