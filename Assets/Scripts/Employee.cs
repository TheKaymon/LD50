using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Employee : MonoBehaviour
{
    //public enum State { Task, Gossip, Scared, Travel };
    public enum Task { Gossip, Break, Bathroom, Work };
    private static float[] taskMinDur = { 7f, 4f, 3f, 5f };
    private static float[] taskMaxDur = { 8f, 6f, 7f, 10f };
    private static int taskCount = System.Enum.GetNames(typeof(Task)).Length;

    public string firstName;
    //private Color outlineColor;
    public TextMeshPro label;
    public EmployeeVisual sprite;

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

    private const float distThreshold = 0.01f;
    private List<Vector2> path = new List<Vector2>();
    private int pathIndex;

    // Start is called before the first frame update
    void Start()
    {
        cubicle = transform.position;
        //rb2D = GetComponent<Rigidbody2D>();
        firstName = Game.instance.office.RegisterEmployee(this);
        label.SetText(firstName);
        if ( gossipLevel == maxGossip )
            label.color = Game.instance.knowColor;

        currentTask = Task.Work;
        taskTimer = Random.Range( taskMinDur[(int)currentTask], taskMaxDur[(int)currentTask]);
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
                if ( Vector2.SqrMagnitude((Vector2)transform.position - destination) < distThreshold )
                {
                    pathIndex++;

                    if ( pathIndex < path.Count )
                    {
                        destination = path[pathIndex];
                    }
                    else
                    {
                        travelling = false;
                        sprite.SetWobble(false);

                        if ( currentTask == Task.Gossip )
                            PickNextTask();
                        return;
                    }
                    //rb2D.velocity = Vector2.zero;
                    // Wobble

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
                label.color = Game.instance.knowColor;
            }
        }
    }

    public void StopGossip()
    {
        //Debug.Log($"{name} stopping {currentTask}");
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
        sprite.SetWobble(false);
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
            sprite.SetWobble(false);
        }
    }

    private void JoinGossip( Employee targ )
    {
        targ.gossip.JoinGossip(this);
        currentTask = Task.Gossip;
        //taskTimer = taskDurations[(int)currentTask];
        gossipTarget = targ;
        travelling = false;
        sprite.SetWobble(false);
    }

    private void PickNextTask()
    {
        Task previousTask = currentTask;

        if ( previousTask != Task.Work )
        {
            currentTask = Task.Work;
            FindPath(cubicle);
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
                // Determine Gossip Target
                gossipTarget = Game.instance.office.GetRandomEmployee(this);
                FindPath(gossipTarget.cubicle);
                // Reset Weight
                taskWeight[(int)Task.Gossip] = 1;
            }
            else if ( result <= totalWeights[1] )
            {
                currentTask = Task.Break;
                FindPath(Game.instance.office.breakroom);
                // Reset Weight
                taskWeight[(int)Task.Break] = 1;
            }
            else if ( result <= totalWeights[2] )
            {
                currentTask = Task.Bathroom;
                FindPath(Game.instance.office.bathroom);
                // Reset Weight
                taskWeight[(int)Task.Bathroom] = 1;
            }
            else
                currentTask = Task.Work;
        }

        taskTimer = Random.Range(taskMinDur[(int)currentTask], taskMaxDur[(int)currentTask]);

    }

    private void FindPath( Vector2 target )
    {
        path.Clear();
        path.AddRange(Game.instance.office.pathfinder.FindPath(transform.position, target));
        if ( path.Count > 0 )
        {
            pathIndex = 0;
            destination = path[0];
            travelling = true;
            sprite.SetWobble(true);
        }
        else
        {
            Debug.Log($"{name} failed to find path to {currentTask} ({target})");
            PickNextTask();
        }
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        if ( gossip == null && travelling && collision.CompareTag("Employee") )
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

    private void OnDrawGizmos()
    {
        if ( travelling && path.Count > 0 )
        {
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawSphere(path[pathIndex], 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, path[pathIndex]);
            for ( int i = pathIndex+1; i < path.Count; i++ )
            {
                //Gizmos.color = Color.yellow;
                //Gizmos.DrawSphere(path[i], 0.1f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}
