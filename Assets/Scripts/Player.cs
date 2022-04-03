using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool showTestPath = false;
    public Transform testTarg;
    public Node targNode;
    public Node playerNode;
    public List<Vector2> path = new List<Vector2>();
    public NodeGrid grid;

    public Rigidbody2D rb;
    public Transform visual;
    public LayerMask interactMask;
    public Vector2 officeMin;
    public Vector2 officeMax;

    public float interactRadius = 1f;
    public float speed = 50f;

    private const float wobbleAngle = 15f;
    //private bool wobbling = false;
    private float wobbleTimer = 0f;
    private const float wobbleInterval = 1f;
    private float zRot = 0f;

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

        if( rb.velocity.magnitude > 0.1f )
        {
            wobbleTimer += Time.deltaTime;
            if ( wobbleTimer > wobbleInterval )
                wobbleTimer -= wobbleInterval;
            float eval = Game.instance.wobbleCurve.Evaluate(wobbleTimer / wobbleInterval);
            zRot = wobbleAngle * eval;
            visual.localRotation = Quaternion.Euler(0, 0, zRot);
        }
        else
        {
            zRot = Mathf.MoveTowards(zRot, 0, 3 * wobbleAngle * Time.deltaTime);
            visual.localRotation = Quaternion.Euler(0, 0, zRot);
        }

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


        if ( showTestPath )
        {
            Node newPlayer = grid.NodeFromWorld(transform.position);
            Node newTarg = grid.NodeFromWorld(testTarg.position);
            if ( newPlayer != playerNode || newTarg != targNode )
            {
                path = Game.instance.office.pathfinder.FindPath(transform.position, testTarg.position);
                playerNode = newPlayer;
                targNode = newTarg;
            }
        }
    }

    public bool InOffice()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        return (x > officeMin.x && x < officeMax.x && y > officeMin.y && y < officeMax.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);

        Gizmos.DrawWireCube(( officeMin + officeMax ) / 2f, ( officeMax - officeMin ));

        if( showTestPath && path.Count > 0 )
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(path[0], 0.1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, path[0]);
            for ( int i = 0 + 1; i < path.Count; i++ )
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(path[i], 0.1f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}
