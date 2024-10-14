using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // Movement speed
    [SerializeField] private float jumpForce = 10f; // Jump force
    [SerializeField] private float glideGravityScale = 0.4f; // Gravity scale when gliding
    [SerializeField] private float glideSpeed = 5f; // Horizontal speed when gliding
    [SerializeField] private int maxJumps = 2; // Number of allowed jumps (2 for double jump)

    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private bool gliding;
    private float glideDirection; // Stores the initial direction when gliding starts
    private int currentJumps; // Tracks how many jumps the player has made

    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Apply movement based on gliding state
        if (!gliding)
        {
            // Normal horizontal movement
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            // Flip player when turning
            if (horizontalInput > 0.01f)
                transform.localScale = Vector3.one; // Face right
            else if (horizontalInput < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1); // Face left
        }
        else
        {
            // Maintain the initial direction when gliding
            body.velocity = new Vector2(glideDirection * glideSpeed, body.velocity.y);
        }

        // Handle jumping and double jump
        if (Input.GetKeyDown(KeyCode.Space) && currentJumps < maxJumps)
        {
            Jump(); // Jump or double jump
        }

        // Gliding toggle based on the "G" key when airborne
        if (!grounded)
        {
            if (Input.GetKey(KeyCode.G) && !gliding)
            {
                StartGliding();
            }
            else if (Input.GetKeyUp(KeyCode.G) || grounded)
            {
                StopGliding();
            }
        }

        // Update animator parameters
        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", grounded);
    }

    private void Jump()
    {
        // Reset gliding state and allow jumping or double jump
        StopGliding();

        body.velocity = new Vector2(body.velocity.x, jumpForce); // Apply jump force
        anim.SetTrigger("Jump");

        currentJumps++; // Increment jump count
        grounded = false;
    }

    private void StartGliding()
    {
        // Do not allow gliding if the player is grounded
        if (grounded) return;

        gliding = true;

        // Stop upward velocity (if the player is going up) when gliding starts
        if (body.velocity.y > 0)
        {
            body.velocity = new Vector2(body.velocity.x, 0f); // Reset upward velocity
        }

        // Determine the initial horizontal direction for gliding
        glideDirection = body.velocity.x > 0 ? 1f : (body.velocity.x < 0 ? -1f : 0f);

        // Reduce gravity for slower falling during gliding
        body.gravityScale = glideGravityScale;
        anim.SetBool("Glide", true);
    }

    private void StopGliding()
    {
        if (!gliding) return; // Ensure gliding is only stopped once

        gliding = false;
        body.gravityScale = 1f; // Restore normal gravity
        anim.SetBool("Glide", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
            currentJumps = 0; // Reset the jump count when touching the ground
            StopGliding(); // Stop gliding when grounded
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }
}
