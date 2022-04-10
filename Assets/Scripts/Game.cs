using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    public Color gossipColor;

    public Color knowColor;

    private const int maxReputation = 100;
    private int reputation;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        reputation = maxReputation - 10;
        DisplayReputation();
        timer = 0f;
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ( !paused )
        {
            timer += Time.deltaTime;

            //if( Input.GetKeyDown(KeyCode.Escape )
        }
    }

    public void SeenByBoss()
    {
        reputation -= 10;

        if ( reputation <= 0 )
        {
            reputation = 0;
            EndGame();
        }

        DisplayReputation();
    }

    public void SecretShared()
    {
        reputation -= 5;

        if ( reputation <= 0 )
        {
            reputation = 0;
            EndGame();
        }

        DisplayReputation();
    }

    public void EndGame()
    {
        paused = true;

        gameOverText.SetText($"You managed to hold onto your reputation for {Mathf.FloorToInt(timer / 30f)} hours.");
        gameOverScreen.SetActive(true);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void DisplayReputation()
    {
        reputationBar.fillAmount = ( (float)reputation / maxReputation );
    }
}
