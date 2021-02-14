using System.Runtime.InteropServices.ComTypes;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    #region Player Status
    public int level = 0;                                                   // Level Game Play
    public int score = 0;                                                   // Score by Kill Enemy
    public float hpPlayer = 100;                                            // Player Health
    public float maxHpPlayer = 100;                                         // Max HP
    #endregion

    #region Text User Interface
    private Text hpText;
    private Text scoreText;
    #endregion

    #region Navigator
    private GameObject destination;
    private GameObject navigatorPoint;
    [HideInInspector] public int point;
    [HideInInspector] public bool isArrive;
    #endregion

    #region Game Value
    public int dropRate;                                                    // Drop Item Rate Defalut = 5
    public bool playWithJoyStick;                                           // Check Play by
    public bool isPause;                                                    // Check Game Pause
    public bool keyItem;                                                    // Check Get Key Item
    public bool isComplete;                                                 // Check Game Clear
    public bool isGameover;
    public bool isRestart;
    public int damageDouble = 1;
    #endregion

    #region GameObject
    private GameObject blackScreen;
    private GameObject blocker;
    private GameObject enemyGroup;
    private GameObject completeTab;
    private GameObject pauseTab;
    private GameObject gameoverTab;
    #endregion

    void Awake()
    {
        #region Singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        #endregion

        GetComponent();
        SetLevel();
    }

    public void GetComponent()
    {
        blackScreen = GameObject.Find("BlackScreen");
        hpText = GameObject.Find("HpPlayer").GetComponent<Text>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        destination = GameObject.Find("Destination");
        navigatorPoint = GameObject.Find("NavigatorPoint");
        blocker = GameObject.Find("BlockerGroup");
        enemyGroup = GameObject.Find("EnemyGroup");
        completeTab = GameObject.Find("CompleteTab");
        pauseTab = GameObject.Find("PauseTab");
        gameoverTab = GameObject.Find("GameoverTab");

        for (int i = 0; i < enemyGroup.transform.childCount; i++)
        {
            enemyGroup.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        SetUpOnStart();
    }

    public void SetUpOnStart()
    {
        blackScreen.SetActive(false);                                       // Set Black Screen Disable on Start
        completeTab.SetActive(false);                                       // Set Complete Disable on Start
        gameoverTab.SetActive(false);
        point = 1;                                                          // Set Start Point of Navigator
        isComplete = false;
        isRestart = false;
    }

    void Update()
    {
        #region Set Variable to Text UI
        hpText.text = hpPlayer.ToString();
        scoreText.text = score.ToString();
        #endregion

        PotionHelper();
        CheckKeyItem();
        LimitHpPlayer();
        GodMode();
        PauseSystem();
        Navigator();

        if (hpPlayer <= 0)
        {
            gameoverTab.SetActive(true);
        }
    }

    void PotionHelper()                                                     // Helper for Player when Low HP
    {
        if (level == 3 && hpPlayer <= 50)                                   // Change Drop Rate to 3 when HP < 50
        {
            dropRate = 3;
        }
        else if (level == 3 && hpPlayer <= 30)                              // Change Drop Rate to 2 when HP < 30
        {
            dropRate = 2;
        }
        else                                                                // Reset Drop Rate
        {
            dropRate = 5;
        }
    }

    void CheckKeyItem()
    {
        if (keyItem)                                                        // Check Get Key Item
        {
            keyItem = false;                                                // Reset Key Item

            if (level == 1 && point >= 5)
            {
                Invoke("LevelClear", 5);                                        // Call Level Clear
            }

            if (level == 2 && point >= 6)
            {
                Invoke("LevelClear", 5);                                        // Call Level Clear
            }

            if (level == 3 && point >= 4)
            {
                Invoke("LevelClear", 5);                                        // Call Level Clear
            }
        }
    }

    void LevelClear()                                                       // Load Next Level
    {
        isComplete = true;
        completeTab.SetActive(true);
        blackScreen.SetActive(true);
        enemyGroup.SetActive(false);
    }

    void LimitHpPlayer()
    {
        if (hpPlayer > maxHpPlayer)                                         // Set HpPlayer Limit By Max HP Player
        {
            hpPlayer = maxHpPlayer;
        }
    }

    public void SetLevel()                                                  // Check Level
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            level = 1;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            level = 2;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            level = 3;
        }
    }

    void PauseSystem()
    {
        if (isPause)
        {
            if (!pauseTab.activeInHierarchy)
            {
                blackScreen.SetActive(true);
                pauseTab.SetActive(true);
                Time.timeScale = 0;
            }
        }
        else if (!isPause)
        {
            blackScreen.SetActive(false);
            pauseTab.SetActive(false);
            Time.timeScale = 1;
        }
    }

    void GodMode()
    {
        if (Input.GetKeyDown(KeyCode.F1))                                   // Full HP
        {
            hpPlayer = maxHpPlayer;
        }

        if (Input.GetKeyDown(KeyCode.F2))                                   // Level Up
        {
            ExperieneManager.instance.expPlayer += 10000;
        }
    }

    void Navigator()                                                        // Check Level to Set Destination
    {
        if (level == 1)
        {
            if (point == 1)
            {
                destination.transform.position = new Vector2(-4, 32);
            }

            if (point == 2)
            {
                destination.transform.position = new Vector2(-15, 15);
            }

            if (point == 3)
            {
                destination.transform.position = new Vector2(-30, 18);
            }

            if (point == 4)
            {
                destination.transform.position = new Vector2(-4, 52);
                Destroy(blocker);
            }
            if (point == 5)
            {
                navigatorPoint.SetActive(false);
            }
        }

        if (level == 2)
        {
            if (point == 1)
            {
                destination.transform.position = new Vector2(18, -2);
            }

            if (point == 2)
            {
                destination.transform.position = new Vector2(20, 18);
            }

            if (point == 3)
            {
                destination.transform.position = new Vector2(35, 31);
            }

            if (point == 4)
            {
                destination.transform.position = new Vector2(4, 45);
            }
            if (point == 5)
            {
                destination.transform.position = new Vector2(42, 60);
                Destroy(blocker);
            }
            if (point == 6)
            {
                navigatorPoint.SetActive(false);
            }
        }

        if (level == 3)
        {
            if (point == 1)
            {
                destination.transform.position = new Vector2(9, -4);
            }

            if (point == 2)
            {
                destination.transform.position = new Vector2(20, -7);
                Destroy(blocker);
            }

            if (point == 3)
            {
                destination.transform.position = new Vector2(32, -15);
            }

            if (point == 4)
            {
                navigatorPoint.SetActive(false);
            }
        }
    }
}
