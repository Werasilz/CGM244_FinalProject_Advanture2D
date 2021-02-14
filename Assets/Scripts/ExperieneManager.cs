using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ExperieneManager : MonoBehaviour
{
    public static ExperieneManager instance;

    #region Player Experience Point
    private float levelPlayer;                                              // Level of Player
    public float expPlayer;                                                 // Exp of Player
    public float increaseDamagePercent;                                     // Increase Damage by Level Player
    [SerializeField] private float expAmount;                               // Amount of Exp
    [HideInInspector] public bool isAddExp;                                 // Check Get Exp
    private RectTransform expBar;                                           // For Get ExpBar Transform width and height
    private GameObject levelPlayerText;
    private GameObject expText;
    private GameObject player;
    public GameObject levelEffect;
    #endregion

    public AudioClip levelupSound;

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
    }

    public void GetComponent()
    {
        #region GetComponent
        levelPlayerText = GameObject.Find("LevelPlayerText");
        expText = GameObject.Find("ExpText");
        expBar = GameObject.Find("ExpBar").GetComponent<RectTransform>();
        player = GameObject.Find("Player");
        #endregion
    }

    void Start()
    {
        SettingExpSystem();
    }

    void Update()
    {
        ExpSystem();
    }

    void ExpSystem()
    {
        // Set Level Text to Player Position
        levelPlayerText.transform.position = new Vector2(player.transform.position.x + 0.2f, player.transform.position.y - 0.6f);

        expText.GetComponent<TextMeshProUGUI>().text = expPlayer + " / " + expAmount;
        levelPlayerText.GetComponent<TextMeshPro>().text = "Lv." + levelPlayer.ToString();
        expAmount = levelPlayer * 100;                                      // Calulate Exp Amount

        if (expPlayer >= expAmount)                                         // Level Up
        {
            SoundManager.instance.PlaySingle(levelupSound);
            GameObject effectClone = Instantiate(levelEffect, player.transform.position, Quaternion.identity);
            effectClone.transform.SetParent(player.gameObject.transform);
            Destroy(effectClone, 1);
            expPlayer -= expAmount;                                         // Decrease Exp Player
            levelPlayer += 1;                                               // Increase Level Player
            GameManager.instance.maxHpPlayer += 10;                         // Increase Max Hp Player
            GameManager.instance.hpPlayer = GameManager.instance.maxHpPlayer;
            increaseDamagePercent += 0.1f;                                  // Increase Damage Percent
            expBar.sizeDelta = new Vector2(0, 50);                          // Reset Exp Bar Width
        }

        if (isAddExp)                                                       // True When Get Exp
        {
            isAddExp = false;                                               // Reset Checking Value
            Vector3 expBarNewSize = expBar.sizeDelta;                       // Save Exp Bar Size

            float expBarWidth = 1920 / levelPlayer;                         // Width of Exp Bar
            expBarNewSize.x = expPlayer * expBarWidth / 100;                // Calulate Exp Bar New Size
            expBar.sizeDelta = expBarNewSize;                               // Set Exp Bar New Size to Exp Bar
        }
    }

    void SettingExpSystem()
    {
        levelPlayer = 1;
        expPlayer = 0;
        increaseDamagePercent = 1;
    }
}
