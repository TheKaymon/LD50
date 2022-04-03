using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public LayerMask interactMask;

    public float interactRadius = 1f;
    public float speed = 50f;


    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horz, vert).normalized;

        //transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3) direction, speed * Time.deltaTime);
        rb.velocity = direction * speed;

        if ( Input.GetButtonDown("Jump") )
        {
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, interactRadius); //Layermask
            foreach ( Collider2D hit in results )
            {
                if ( hit.CompareTag("Gossip") )
                {
                    hit.GetComponent<Gossip>().StopGossiping();
                    Debug.Log(hit.name);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
