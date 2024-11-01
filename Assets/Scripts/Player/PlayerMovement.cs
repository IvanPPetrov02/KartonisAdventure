using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float glideGravityScale = 0.4f;
    [SerializeField] private float glideSpeed = 5f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float guitarHitRadius = 1.5f;
    [SerializeField] private LayerMask breakableLayer;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float wallSlideSpeed = 2f;

    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private bool gliding;
    private bool dashing;
    private bool wallSliding;
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

        if (!gliding && !dashing && !wallSliding)
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            Dash();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BreakWallWithGuitar();
        }

        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", grounded);
    }

    private void Jump()
    {
        StopWallSlide();
        StopGliding();
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        anim.SetTrigger("Jump");

        currentJumps++;
        grounded = false;
    }

    private void StartGliding()
    {
        if (grounded || wallSliding) return;

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

        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        body.velocity = new Vector2(dashDirection * dashForce, body.velocity.y);

        anim.SetTrigger("Dash");

        Invoke(nameof(EndDash), 0.1f);
    }

    private void EndDash()
    {
        dashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Allow full jump refresh if the player is on top of the ground
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal == Vector2.up)
                {
                    grounded = true;
                    currentJumps = 0;
                    StopGliding();
                    StopWallSlide();
                    return;
                }
                else if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                {
                    // Player is on the side of the ground; initiate sliding on ground sides
                    StartWallSlide();
                    return;
                }
            }
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Refresh only one jump on any wall contact
            grounded = true;
            if (currentJumps >= maxJumps)
            {
                currentJumps = maxJumps - 1;
            }
            StopGliding();
            StopWallSlide(); // Ensure wall does not slide
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            grounded = false;
            StopWallSlide();
        }
    }

    private void StartWallSlide()
    {
        wallSliding = true;
        body.velocity = new Vector2(0, -wallSlideSpeed); // Slow downward slide speed
    }

    private void StopWallSlide()
    {
        wallSliding = false;
    }

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
