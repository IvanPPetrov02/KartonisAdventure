using UnityEngine;

public class Enemy_sideways : MonoBehaviour
{
[SerializeField]private float damage;
[SerializeField]private float speed;
[SerializeField]private float movementDistance;
private bool movingLeft;
private float leftEdnge;
private float rightEdge;
private void Awake() 
{
    if(movingLeft)
    {
        if(transform.position.x > leftEdnge)
        {
            transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        else
            movingLeft = false;
    }    
    else
    {
        if(transform.position.x < rightEdge)
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        else
            movingLeft = true;
    }
}
private void OnTriggerEnter2D(Collider2D collision) 
{
    if(collision.tag == "Player")
    {
        collision.GetComponent<Health>().TakeDamage(damage);
    }
}
}
