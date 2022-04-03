using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public List<Vector2> patrolPoints = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //for ( int i = 1; i < patrolPoints.Count; i++ )
        //{
        //    Gizmos.DrawLine(patrolPoints[i - 1], patrolPoints[i]);
        //}
    }
}
