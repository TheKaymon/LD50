using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public Image reputationBar;
    public GameObject gameOverScreen;
    public TextMeshProUGUI gameOverText;

    public Gossip gossipPrefab;
    public TextBubble bubblePrefab;

    public Color knowColor;

    private int reputation;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        reputation = 100;
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if ( !paused )
            timer += Time.deltaTime;
    }

    public void SeenByBoss( bool inOffice )
    {
        if( inOffice)
            reputation += 5;
        else
            reputation -= 10;

        if ( reputation <= 0 )
            EndGame();
    }

    public void SecretShared()
    {
        reputation -= 4;

        if ( reputation <= 0 )
            EndGame();
    }

    public void EndGame()
    {
        paused = true;

        gameOverText.SetText("Game Over");
        gameOverScreen.SetActive(true);
    }
}
