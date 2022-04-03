using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gossip : MonoBehaviour
{
    //public GameObject textBubble;
    //public SpriteRenderer bubbleSprite;
    //public TextMeshPro text;

    private int gossipCount = 5;
    private float gossipTimer;
    private const float gossipInterval = 2f;

    private List<Employee> participants = new List<Employee>();

    //private bool canShareSecret;
    private bool sharingSecret = false;
    private Employee talking;

    private float medianX;
    private Vector3 leftOffset = new Vector2(-.75f, 0.3f);
    private Vector3 rightOffset = new Vector2(.5f, 0.3f);

    private TextBubble lastBubble = null;
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        gossipTimer -= Time.deltaTime;
        if ( gossipTimer < 0f )
        {
            gossipCount--;
            if ( gossipCount <= 0 )
                StopGossiping();
            else
            {
                GenerateGossip();
            }
        }
        
    }

    public void StartGossiping( Employee first, Employee second )
    {
        participants.Add(first);
        participants.Add(second);
        second.gossip = this;
        //canShareSecret = first.knowsSecret || second.knowsSecret;
        gossipTimer = gossipInterval;
        enabled = true;
        DetermineX();

        GenerateGossip();

        Debug.Log($"{participants[0].name} and {participants[1].name} begin gossiping");
    }

    public void JoinGossip( Employee employee )
    {
        participants.Add(employee);
        employee.gossip = this;
        //canShareSecret |= employee.knowsSecret;
        gossipCount += 2;
        DetermineX();

        Debug.Log($"{employee} joins a gossip sesh");
    }

    public void StopGossiping()
    {
        foreach ( Employee e in participants )
        {
            e.StopGossip();
        }
        if ( lastBubble != null )
            lastBubble.Remove();
        Destroy(gameObject);
    }

    private void ShareSecret( int level )
    {
        foreach ( Employee e in participants )
        {
            e.LearnSecret( level );
        }
    }

    private void DetermineX()
    {
        float x = 0;
        foreach ( Employee e in participants )
        {
            x += e.transform.position.x;
        }

        medianX = x / participants.Count;
    }

    private void GenerateGossip()
    {
        if ( sharingSecret )
        {
            ShareSecret(talking.gossipLevel);
        }

        gossipTimer = gossipInterval;

        int index = Random.Range(0, participants.Count);
        talking = participants[index];

        if ( talking.gossipLevel > 0 )
        {
            // 20% Chance to reveal secret
            sharingSecret = Random.Range(0f, 1f) > 0.8f;
        }

        Vector3 offset = talking.transform.position.x > medianX ? leftOffset : rightOffset;
        lastBubble = Instantiate(Game.instance.bubblePrefab, talking.transform.position + offset, Quaternion.identity);
        lastBubble.Initialize(gossipInterval, sharingSecret, index + 1);
        lastBubble.gameObject.SetActive(true);
    }
}
