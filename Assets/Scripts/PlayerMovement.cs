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

    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private bool gliding;
    private float glideDirection;
    private int currentJumps;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (!gliding)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (horizontalInput > 0.01f)
                transform.localScale = Vector3.one;
            else if (horizontalInput < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
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
        anim.SetBool("Glide", true);
    }

    private void StopGliding()
    {
        if (!gliding) return;

        gliding = false;
        body.gravityScale = 1f;
        anim.SetBool("Glide", false);
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
        // Detect breakable walls in front of the player within a certain radius
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
        // Visualize the guitar hit radius for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, guitarHitRadius);
    }
}
