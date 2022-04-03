using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Employee : MonoBehaviour
{
    public enum Task { Gossip, Break, Bathroom, Work };
    private static float[] taskDurations = { 8f, 4f, 6f, 5f };
    private static int taskCount = System.Enum.GetNames(typeof(Task)).Length;

    public string firstName;
    public Color eyeColor;
    public Color tieColor;
    //private Color outlineColor;
    private int employeeID;

    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer tie;
    public SpriteRenderer outline;
    //private Rigidbody2D rb2D;

    //public bool knowsSecret;
    public const int maxGossip = 4;
    public int gossipLevel = 0;
    public Gossip gossip;

    private float taskTimer;
    private Task currentTask;
    private bool travelling = false;
    private float speed = 2f;
    //private Vector2 direction;
    private Vector2 destination; // Path?
    private Vector2 cubicle;
    private int[] taskWeight = { 1, 1, 1, 3 };
    private int[] totalWeights = { 0, 0, 0, 0 };
    private Employee gossipTarget;
    

    // Start is called before the first frame update
    void Start()
    {
        //rb2D = GetComponent<Rigidbody2D>();
        employeeID = Game.instance.office.RegisterEmployee(this);
        cubicle = transform.position;

        // Get Random Eye Color
        // Get Random Tie Color
        // Get Random First Name
        // Randomly Flip Sprites

        eyes.color = eyeColor;
        tie.color = tieColor;
        if ( gossipLevel == maxGossip )
            outline.color = Game.instance.knowColor;

        currentTask = Task.Work;
        taskTimer = taskDurations[(int)currentTask];
    }

    // Update is called once per frame
    void Update()
    {
        if ( !Game.paused )
        {
            if ( travelling )
            {
                // Move
                transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                //direction = ( destination - (Vector2)transform.position ).normalized; // Move to Pathfinding
                //rb2D.velocity = direction * speed;
                // Check Distance
                if ( Vector2.SqrMagnitude((Vector2)transform.position - destination) < 0.2f )
                {
                    travelling = false;

                    if ( currentTask == Task.Gossip )
                        PickNextTask();
                    //rb2D.velocity = Vector2.zero;
                }
            }
            else if( currentTask != Task.Gossip )
            {
                //rb2D.velocity = Vector2.MoveTowards(rb2D.velocity, Vector2.zero, speed * Time.deltaTime);
                taskTimer -= Time.deltaTime;
                if ( taskTimer < 0 )
                {
                    PickNextTask();
                }
            }
        }
    }

    public void LearnSecret( int level )
    {
        if ( gossipLevel < level )
        {
            gossipLevel++;
            if ( gossipLevel == maxGossip )
            {
                Game.instance.SecretShared();
                outline.color = Game.instance.knowColor;
            }
        }
    }

    public void StopGossip()
    {
        gossip = null;
        PickNextTask();
    }

    // Request to Gossip
    public void GossipWith( Employee with )
    {
        currentTask = Task.Gossip;
        //taskTimer = taskDurations[(int)currentTask];
        gossipTarget = with;
        travelling = false;
    }

    // Start Gossip
    private void StartGossip( Employee targ )
    {
        if ( targ.gossip != null )
            JoinGossip(targ);
        else
        {
            currentTask = Task.Gossip;
            //taskTimer = taskDurations[(int)currentTask];
            gossipTarget = targ;
            gossip = Instantiate(Game.instance.gossipPrefab, transform.position, Quaternion.identity);
            //Debug.Log($"{name} is trying to gossip with {targ}");
            gossip.StartGossiping(this, targ);
            targ.GossipWith(this);
            travelling = false;
        }
    }

    private void JoinGossip( Employee targ )
    {
        targ.gossip.JoinGossip(this);
        currentTask = Task.Gossip;
        //taskTimer = taskDurations[(int)currentTask];
        gossipTarget = targ;
        travelling = false;
    }

    private void PickNextTask()
    {
        Task previousTask = currentTask;

        if ( previousTask != Task.Work )
        {
            currentTask = Task.Work;
            destination = cubicle;
            taskWeight[(int)Task.Gossip] += 1;
        }
        else
        {
            int totalWeight = 0;
            for ( int i = 0; i < taskWeight.Length; i++ )
            {
                totalWeight += taskWeight[i];
                totalWeights[i] = totalWeight;
            }
            int result = Random.Range(0, totalWeight);
            if ( result <= totalWeights[0] )
            {
                currentTask = Task.Gossip;
            }
            else if ( result <= totalWeights[1] )
            {
                currentTask = Task.Break;
            }
            else if ( result <= totalWeights[2] )
            {
                currentTask = Task.Bathroom;
            }
            else
                currentTask = Task.Work;
        }

        taskTimer = taskDurations[(int)currentTask];

        // Decide if Travelling
        if ( previousTask != currentTask )
        {
            travelling = true;
        }

        if ( currentTask == Task.Gossip )
        {
            // Determine Gossip Target
            gossipTarget = Game.instance.office.GetRandomEmployee(employeeID);
            destination = gossipTarget.cubicle;
            // Reset Weight
            taskWeight[(int)Task.Gossip] = 1;
        }
        else if( currentTask == Task.Break )
        {
            destination = Game.instance.office.breakroom;
            // Reset Weight
            taskWeight[(int)Task.Break] = 1;
        }
        else if ( currentTask == Task.Bathroom )
        {
            destination = Game.instance.office.bathroom;
            // Reset Weight
            taskWeight[(int)Task.Bathroom] = 1;
        }

    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        if ( gossip == null && collision.CompareTag("Employee") )
        {
            Employee other = collision.GetComponent<Employee>();
            //Debug.Log($"{name} collided with {collision.name}");

            // If Gossip task
            if ( gossip == null && currentTask == Task.Gossip )
            {
                // Check for Gossip Target
                if( other == gossipTarget )
                {
                    StartGossip( gossipTarget );
                }
            }
            else // chance to gossip
            {
                if( Random.Range(0, 1f) < 0.25f )
                {
                    if ( other.gossip != null )
                    {
                        JoinGossip(other);
                    }
                    else
                    {
                        StartGossip(other);
                    }
                }
            }
        }
    }
}
