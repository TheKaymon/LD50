using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;
    public static bool paused = false;
    void Awake()
    {
        if ( instance != null )
            Debug.Log("Multiple Game Managers!");
        instance = this;
    }

    public Office office;
    public Player player;
    //public Boss boss;
    public AnimationCurve wobbleCurve;

    public Gossip gossipPrefab;
    public TextBubble bubblePrefab;

    public Color knowColor;

    private int secretKnowers;

    // Start is called before the first frame update
    void Start()
    {
        secretKnowers = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SecretShared()
    {
        secretKnowers++;

        if( secretKnowers >= office.EmployeeCount )
        {
            Debug.Log("Everyone knows your secret!");
        }
    }
}
