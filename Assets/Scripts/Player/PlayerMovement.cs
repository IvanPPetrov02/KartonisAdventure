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
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip glideSound;

    private Rigidbody2D body;
    private Animator anim;
    private AudioSource audioSource;
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
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (!gliding && !dashing && !wallSliding)
        {
            Move(horizontalInput);
        }
        else if (gliding)
        {
            body.velocity = new Vector2(glideDirection * glideSpeed, body.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentJumps < maxJumps)
        {
            Jump();
        }

        HandleGliding();
        HandleDashing();
        HandleGuitarHit();

        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", grounded);
    }

private void Move(float horizontalInput)
{
    body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

    if (horizontalInput > 0.01f)
        transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
    else if (horizontalInput < -0.01f)
        transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

    // Only play movement sound if the player is grounded
    if (horizontalInput != 0 && grounded && !audioSource.isPlaying)
    {
        audioSource.clip = moveSound;
        audioSource.loop = true;
        audioSource.Play();
    }
    else if ((horizontalInput == 0 || !grounded) && audioSource.isPlaying)
    {
        audioSource.Stop();
    }
}


    private void Jump()
    {
        StopWallSlide();
        StopGliding();
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        anim.SetTrigger("Jump");
        audioSource.PlayOneShot(jumpSound);

        currentJumps++;
        grounded = false;
    }

    private void HandleGliding()
    {
        if (!grounded && Input.GetKey(KeyCode.G) && !gliding)
        {
            StartGliding();
        }
        else if (Input.GetKeyUp(KeyCode.G) || grounded)
        {
            StopGliding();
        }
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
        audioSource.clip = glideSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopGliding()
    {
        if (!gliding) return;

        gliding = false;
        body.gravityScale = 1f;
        anim.SetBool("Glide", false);
        if (audioSource.clip == glideSound && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void HandleDashing()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            Dash();
        }
    }

    private void Dash()
    {
        dashing = true;
        lastDashTime = Time.time;

        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        body.velocity = new Vector2(dashDirection * dashForce, body.velocity.y);

        anim.SetTrigger("Dash");
        audioSource.PlayOneShot(dashSound);

        Invoke(nameof(EndDash), 0.1f);
    }

    private void EndDash()
    {
        dashing = false;
    }

    private void HandleGuitarHit()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            BreakWallWithGuitar();
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            HandleGroundCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            HandleWallCollision();
        }
    }

    private void HandleGroundCollision(Collision2D collision)
    {
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
                StartWallSlide();
                return;
            }
        }
    }

    private void HandleWallCollision()
    {
        grounded = true;
        if (currentJumps >= maxJumps)
        {
            currentJumps = maxJumps - 1;
        }
        StopGliding();
        StopWallSlide();
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
        body.velocity = new Vector2(0, -wallSlideSpeed);
    }

    private void StopWallSlide()
    {
        wallSliding = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, guitarHitRadius);
    }
}
