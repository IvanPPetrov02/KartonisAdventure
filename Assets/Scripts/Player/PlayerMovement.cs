using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float glideGravityScale = 0.4f;
    [SerializeField] private float glideSpeed = 5f;
    [SerializeField] private int maxJumps = 2;

    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private bool gliding;
    private float glideDirection;
    private int currentJumps;
    private Vector3 originalScale;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (!gliding)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (horizontalInput > 0.01f)
                transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            else if (horizontalInput < -0.01f)
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
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

        // Use the glide animation trigger to play the animation once
        anim.SetTrigger("StartGlide");
    }

    private void StopGliding()
    {
        if (!gliding) return;

        gliding = false;
        body.gravityScale = 1f;
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
}
