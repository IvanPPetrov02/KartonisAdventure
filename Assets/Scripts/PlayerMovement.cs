
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;

    private void Awake() 
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }
     private void Update() 
    {
        float horizontalInput =Input.GetAxis("Horizontal");

        body.velocity = new Vector2(Input.GetAxis("Horizontal")* speed,body.velocity.y);  
        //flip player when turning to another side
        if(horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < - 0.01f)
            transform.localScale = new Vector3(-1,1,1);

        if(Input.GetKey(KeyCode.Space) && grounded)
            Jump();


        //set animator paramers
        anim.SetBool("Run", horizontalInput !=0); 
        anim.SetBool("Grounded",grounded);
    }
    private void Jump() 
    {
        body.velocity = new Vector2(body.velocity.x, speed); 
        anim.SetTrigger("Jump");
        grounded = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        grounded = true;
    }
}
