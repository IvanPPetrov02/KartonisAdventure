using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMoveUpDown : MonoBehaviour
{
    public float speed = 5f; // Speed of the movement
    public float height = 3f; // Distance to move up and down

    private Vector2 startPos;
    private bool movingUp = true;

    void Start()
    {
        // Record the starting position of the box
        startPos = transform.position;
    }

    void Update()
    {
        // Move the box automatically up and down
        if (movingUp)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }

        // Reverse direction when reaching the desired height
        if (Vector2.Distance(startPos, transform.position) >= height)
        {
            movingUp = !movingUp;
            startPos = transform.position;
        }
    }
}
