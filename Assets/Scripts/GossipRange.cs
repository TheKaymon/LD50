using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GossipRange : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        // If Gossip task
        // Check for Gossip Target
        // Else chance to gossip
        Debug.Log($"{name} collided with {collision.name}");
    }
}
