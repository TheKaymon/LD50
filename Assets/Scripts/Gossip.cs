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
    private const float minInterval = 1.5f;
    private const float maxInterval = 3f;

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
        if ( !Game.paused )
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
    }

    public void StartGossiping( Employee first, Employee second )
    {
        //Debug.Log($"{first.name} and {second.name} begin gossiping");
        participants.Add(first);
        participants.Add(second);
        second.gossip = this;
        //canShareSecret = first.knowsSecret || second.knowsSecret;
        gossipTimer = Random.Range(minInterval, maxInterval);
        enabled = true;
        DetermineX();

        GenerateGossip();
    }

    public void JoinGossip( Employee employee )
    {
        participants.Add(employee);
        employee.gossip = this;
        //canShareSecret |= employee.knowsSecret;
        gossipCount += 2;
        DetermineX();

        //Debug.Log($"{employee} joins a gossip sesh");
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
            ShareSecret(talking.gossipLevel);

        gossipTimer = Random.Range(minInterval, maxInterval); ;

        int index = Random.Range(0, participants.Count);
        talking = participants[index];

        // 50% Chance to reveal secret per gossip level
        sharingSecret = Random.Range(0, 2) < talking.gossipLevel;

        bool onLeft = talking.transform.position.x > medianX;
        Vector3 offset = onLeft ? leftOffset : rightOffset;
        lastBubble = Instantiate(Game.instance.bubblePrefab, talking.transform.position + offset, Quaternion.identity);
        lastBubble.Initialize(gossipTimer, sharingSecret, onLeft);
        lastBubble.gameObject.SetActive(true);

        if( sharingSecret )
            Audio.instance.PlaySFX(2, transform.position, Random.Range(0.9f, 1.1f));
    }
}
