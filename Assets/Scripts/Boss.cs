using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public List<Vector2> patrolPoints = new List<Vector2>();

    private bool patrolling = false;
    public float speed = 1f;

    private float timer;
    private float maxTime = 20f;
    private float minTime = 10f;
    private int patrolIndex;
    private const float distThreshold = 0.01f;

    private const float wobbleAngle = 15f;
    private float wobbleTimer = 0f;
    private const float wobbleInterval = 1f;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        if( patrolling )
        {
            // Move
            transform.position = Vector2.MoveTowards(transform.position, patrolPoints[patrolIndex], speed * Time.deltaTime);
            //direction = ( destination - (Vector2)transform.position ).normalized; // Move to Pathfinding
            //rb2D.velocity = direction * speed;
            // Check Distance
            if ( Vector2.SqrMagnitude((Vector2)transform.position - patrolPoints[patrolIndex]) < distThreshold )
            {
                patrolIndex++;
                if( patrolIndex >= patrolPoints.Count )
                {
                    patrolling = false;
                    timer = Random.Range(minTime, maxTime);
                    transform.localRotation = Quaternion.identity;
                }
                else
                {
                    wobbleTimer += Time.deltaTime;
                    if ( wobbleTimer > wobbleInterval )
                        wobbleTimer -= wobbleInterval;
                    float eval = Game.instance.wobbleCurve.Evaluate(wobbleTimer / wobbleInterval);
                    transform.localRotation = Quaternion.Euler(0, 0, wobbleAngle * eval);
                }
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if ( timer < 0 )
            {
                patrolIndex = 0;
                wobbleTimer = 0f;
                patrolling = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for ( int i = 1; i < patrolPoints.Count; i++ )
        {
            Gizmos.DrawLine(patrolPoints[i - 1], patrolPoints[i]);
        }
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        if ( collision.gameObject.CompareTag("Player") )
        {
            Debug.Log($"{collision.gameObject.name} (trigger) was spotted by the Boss {Game.instance.player.InOffice()}");
            if ( !Game.instance.player.InOffice() )
            {
                Game.instance.SeenByBoss();

                Audio.instance.PlaySFX(1, transform.position, Random.Range(0.9f, 1.1f));
            }
        }
    }
}
