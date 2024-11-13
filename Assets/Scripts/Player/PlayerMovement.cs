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

    private DialogueManager dialogueManager;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void Update()
    {
        if (dialogueManager != null && dialogueManager.IsDialogueActive())
        {
            anim.SetBool("Run", false);
            anim.SetBool("Grounded", grounded);
            StopWalkingSound();
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");

        if (!gliding && !dashing && !wallSliding)
        {
            Move(horizontalInput);
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentJumps > 0)
        {
            Jump();
        }

        if (AbilityManager.Instance.CanGlide)
        {
            HandleGliding();
        }

        if (AbilityManager.Instance.CanDash)
        {
            HandleDashing();
        }

        if (AbilityManager.Instance.CanBreak)
        {
            HandleGuitarHit();
        }

        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", grounded);

        HandleWalkingSound(horizontalInput);
    }

    private void Move(float horizontalInput)
    {
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }

    private void Jump()
    {
        if (!grounded && !AbilityManager.Instance.CanDoubleJump && currentJumps <= 1)
            return;
        
        StopWallSlide();
        StopGliding();
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        anim.SetTrigger("Jump");
        
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
        
        currentJumps--;
        grounded = false;
        StopWalkingSound();
    }


    private void HandleWalkingSound(float horizontalInput)
    {
        if (grounded && Mathf.Abs(horizontalInput) > 0.01f)
        {
            if (!audioSource.isPlaying || audioSource.clip != moveSound)
            {
                audioSource.clip = moveSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            StopWalkingSound();
        }
    }

    private void StopWalkingSound()
    {
        if (audioSource.isPlaying && audioSource.clip == moveSound)
        {
            audioSource.Stop();
        }
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
            anim.SetTrigger("Break");
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
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (IsSideCollision(contact.normal))
                {
                    StartWallSlide();
                    return;
                }
                else if (IsTopCollision(contact.normal))
                {
                    grounded = true;
                    currentJumps = maxJumps;
                    StopWallSlide();
                    return;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (IsSideCollision(contact.normal))
                {
                    StartWallSlide();
                    return;
                }
                else if (IsTopCollision(contact.normal))
                {
                    grounded = true;
                    StopWallSlide();
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
            StopWallSlide();
        }
    }

    private void StartWallSlide()
    {
        if (grounded || wallSliding) return;

        wallSliding = true;

        body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, -wallSlideSpeed));

        anim.SetBool("WallSlide", true);
    }

    private void StopWallSlide()
    {
        if (!wallSliding) return;

        wallSliding = false;
        anim.SetBool("WallSlide", false);
    }

    private bool IsTopCollision(Vector2 normal)
    {
        return Vector2.Dot(normal, Vector2.up) > 0.5f;
    }

    private bool IsSideCollision(Vector2 normal)
    {
        return Mathf.Abs(Vector2.Dot(normal, Vector2.right)) > 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, guitarHitRadius);
    }
}
