using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float glideGravityScale = 0.4f;
    [SerializeField] private float glideSpeed = 5f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float guitarHitRadius = 1.5f;
    [SerializeField] private LayerMask breakableLayer; // Layer for breakable walls
    [SerializeField] private float dashForce = 15f;    // Force applied during dash
    [SerializeField] private float dashCooldown = 1f;  // Cooldown time for dash

    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private bool gliding;
    private bool dashing;
    private float glideDirection;
    private int currentJumps;
    private Vector3 originalScale;
    private float lastDashTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (!gliding && !dashing)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (horizontalInput > 0.01f)
                transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            else if (horizontalInput < -0.01f)
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        else if (gliding)
        {
            body.velocity = new Vector2(glideDirection * glideSpeed, body.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentJumps < maxJumps)
        {
            Jump();
        }

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

        // Dash when pressing Left Shift and not on cooldown
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            Dash();
        }

        // Check for breaking wall when pressing "B"
        if (Input.GetKeyDown(KeyCode.B))
        {
            BreakWallWithGuitar();
        }

        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", grounded);
    }

    private void Jump()
    {
        StopGliding();
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        anim.SetTrigger("Jump");

        currentJumps++;
        grounded = false;
    }

    private void StartGliding()
    {
        if (grounded) return;

        gliding = true;

        if (body.velocity.y > 0)
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
        }

        glideDirection = body.velocity.x > 0 ? 1f : (body.velocity.x < 0 ? -1f : 0f);

        body.gravityScale = glideGravityScale;
        anim.SetTrigger("StartGlide");
    }

    private void StopGliding()
    {
        if (!gliding) return;

        gliding = false;
        body.gravityScale = 1f;
        anim.SetBool("Glide", false);
    }

    private void Dash()
    {
        dashing = true;
        lastDashTime = Time.time;

        // Determine dash direction based on player facing direction (localScale.x)
        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        body.velocity = new Vector2(dashDirection * dashForce, body.velocity.y);

        anim.SetTrigger("Dash");

        // End dash after a short time
        Invoke(nameof(EndDash), 0.1f); // Adjust the dash duration as needed
    }

    private void EndDash()
    {
        dashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
            currentJumps = 0;
            StopGliding();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }

    // Method to break walls when pressing "B"
    private void BreakWallWithGuitar()
    {
        Collider2D[] hitWalls = Physics2D.OverlapCircleAll(transform.position, guitarHitRadius, breakableLayer);

        if (hitWalls.Length > 0)
        {
            foreach (Collider2D wall in hitWalls)
            {
                wall.GetComponent<BreakableWall>()?.Break();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, guitarHitRadius);
    }
}
