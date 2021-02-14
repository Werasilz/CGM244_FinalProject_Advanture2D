using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    #region Component
    private Transform player;
    private Transform playerShadow;
    private Rigidbody2D rb2d;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D enemyArea;
    private CircleCollider2D bossArea;
    #endregion

    #region Movement Value
    private Vector2 movement;
    private float speed = 0.3f;
    #endregion

    #region GameObject
    public GameObject shadowPrefab;                             // Enemy Shadow
    public GameObject enemyHpBar;                               // Enemy HP Bar in child of Enemy
    public GameObject damagePopupPrefab;                        // Show Normal Damage
    public GameObject criticalDamagePopupPrefab;                // Show Critical Damage
    public GameObject warningPrefab;                            // Spawn When Enter Enemy Area
    public GameObject smokeEffect;                              // Spawn When Enemy Dead
    public GameObject hpPotion;                                 // Random Spawn after Enemy Dead
    public GameObject dangerZone;                               // Spawn Before Boss Attack
    #endregion

    #region Value for Damage System
    [HideInInspector] public float stackHit;                    // Input Hit from Player
    private float damaged;                                      // Damage to Decrease HpEnemy
    [SerializeField] private float hpEnemy;                     // Enemy Health
    private float changeToHpBar;                                // Calculate Value for change HpEnemy to HpBar
    private bool isCritical;                                    // Check Damage is Critical
    #endregion

    #region Popup Check
    [HideInInspector] public bool isWarning;                    // Check Waring Show
    private bool isPopup;                                       // Check Damage Popup is Show
    #endregion

    #region Enemy Status
    [HideInInspector] public bool isIntoArea;                   // Check Player is enter Enemy Area
    [HideInInspector] public bool isBehindPlayer;               // Check Enemy is Behind Player
    [SerializeField] private int enemyType;                     // Type of Enemy Set when Spawn by SpawnEnemy Script
    [HideInInspector] public bool isDead;                       // Check Enemy Dead
    [HideInInspector] public float expEnemy;                    // Exp Of Enemy for give to Player
    #endregion

    #region Boss Status
    public bool isBoss;                                         // Check for Boss
    private bool isBossAttack;                                  // Check Boss Attack
    private bool isBossHardAttack;                              // Check Boss HardAttack
    private bool isDanger;                                      // Check DangerZone Spawn
    private bool isBehindBoss;                                  // Check Player Behind Boss
    private bool isPlayerLeft;                                  // Check Player On the Left of Boss
    private bool isPlayerRight;                                 // Check Player On the Right of Boss
    private bool isCooldown;                                    // Check Boss Attack Cooldown
    #endregion

    #region Audio
    public AudioClip deadSound;
    public AudioClip beatSound;
    public AudioClip beatBossSound;
    #endregion

    void Awake()
    {
        #region GetComponent
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyArea = gameObject.GetComponentInChildren<CircleCollider2D>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        playerShadow = GameObject.Find("PlayerShadow").GetComponent<Transform>();

        if (isBoss)
        {
            bossArea = GetComponent<CircleCollider2D>();
        }
        #endregion
    }

    void Start()
    {
        SetUpEnemy();                                           // Setup Hp, Exp of Enemy By Enemy Type
        changeToHpBar = hpEnemy / 2;                            // Calculate Value for change HpEnemy to HpBar
    }

    void Update()
    {
        SortingLayerOrder();
    }

    void FixedUpdate()
    {
        BeatByPlayer();
        EnemyDead();
        CheckEnemyArea();

        #region Boss Section
        BossControl();
        CheckPlayerBehind();
        #endregion
    }

    #region Boss Section
    void BossControl()
    {
        if (isBoss)
        {
            if (isBossAttack)
            {
                StopCoroutine("BossRandomAttack");              // Stop Call Attack When Boss Attack
            }

            if (!isBossAttack)
            {
                StartCoroutine("BossRandomAttack");             // Call Attack When Boss Not Attack

                if (GameManager.instance.point >= 4)            // Check Player arrive Point 4
                {
                    EnemyFollowPlayer();                        // Let Boss follow Player
                }
            }
        }
    }

    void CheckPlayerBehind()
    {
        // Player On The Right && Boss On the Left
        if (isBoss && player.transform.position.x > transform.position.x && spriteRenderer.flipX == false)
        {
            isBehindBoss = true;
        }
        // Player On The Left && Boss On the Right
        else if (isBoss && player.transform.position.x < transform.position.x && spriteRenderer.flipX == true)
        {
            isBehindBoss = true;
        }
        else
        {
            isBehindBoss = false;
        }
    }

    IEnumerator BossRandomAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2, 5));

            int i = Random.Range(1, 3);

            // Normal Attack
            // Boss Attack Timimg
            // Call BossRandomAttack
            // Wait 2 - 4 Seconds
            // Spawn Danger Zone 2 Seconds
            // Boss Attack 1 Seconds
            // Cooldown 1 Seconds
            // Wait 1 Seconds to Reset Attack
            if (i == 1)
            {
                if (!isDanger && !isBossAttack)
                {
                    Debug.Log("Normal Attack");
                    // Spawn On the Right
                    if (player.transform.position.x > transform.position.x)
                    {
                        GameObject dangerZoneClone = Instantiate(dangerZone, new Vector2(transform.position.x + 1.5f, transform.position.y - 1f), Quaternion.identity);
                        dangerZoneClone.transform.SetParent(gameObject.transform);
                        isDanger = true;
                        Destroy(dangerZoneClone, 2);
                    }
                    // Spawn On the Left
                    else if (player.transform.position.x < transform.position.x)
                    {
                        GameObject dangerZoneClone = Instantiate(dangerZone, new Vector2(transform.position.x - 1.5f, transform.position.y - 1f), Quaternion.identity);
                        dangerZoneClone.transform.SetParent(gameObject.transform);
                        isDanger = true;
                        Destroy(dangerZoneClone, 2);
                    }

                    // Spawn On the Bottom
                    if (player.transform.position.y < transform.position.y)
                    {
                        GameObject dangerZoneClone = Instantiate(dangerZone, new Vector2(transform.position.x, transform.position.y - 1.8f), Quaternion.identity);
                        dangerZoneClone.transform.SetParent(gameObject.transform);
                        isDanger = true;
                        Destroy(dangerZoneClone, 2);
                    }
                    // Spawn On the Top
                    else if (player.transform.position.y > transform.position.y)
                    {
                        GameObject dangerZoneClone = Instantiate(dangerZone, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                        dangerZoneClone.transform.SetParent(gameObject.transform);
                        isDanger = true;
                        Destroy(dangerZoneClone, 2);
                    }

                    yield return new WaitForSeconds(2);
                    isBossAttack = true;
                    GameManager.instance.damageDouble = 1;
                    anim.SetTrigger("isAttack");
                    Invoke("Cooldown", 1);
                    Invoke("ResetAttack", 3);
                }
            }

            // Hard Attack
            // Boss Attack Timimg
            // Call BossRandomAttack
            // Wait 2 - 4 Seconds
            // Boss Prepare Attack 2 Seconds
            // Boss Attack
            // Cooldown 1 Seconds
            // Wait 1 Seconds to Reset Attack
            else if (i == 2)
            {
                Debug.Log("HardAttack");
                if (!isDanger && !isBossHardAttack)
                {
                    isDanger = true;
                    anim.SetTrigger("isPrepare");
                    bossArea.radius = 5;
                    yield return new WaitForSeconds(2);
                    isBossHardAttack = true;
                    GameManager.instance.damageDouble = 2;
                    anim.SetTrigger("isHardAttack");
                    Invoke("Cooldown", 1);
                    Invoke("ResetAttack", 3);
                }
            }
        }
    }

    void ResetAttack()
    {
        isBossAttack = false;
        isBossHardAttack = false;
        isDanger = false;
        isCooldown = false;
    }

    void Cooldown()
    {
        isCooldown = true;

        if (isBossHardAttack)
        {
            bossArea.radius = 2;
        }
    }
    #endregion

    #region Enemy Area Section
    void EnemyAreaSize()
    {
        if (isIntoArea)                                         // Enemy Area Size Big
        {
            enemyArea.radius = 15;
            speed = 0.6f;
        }
        else if (!isIntoArea)                                   // Enemy Area Size Small
        {
            enemyArea.radius = 10;
            speed = 0.3f;
        }
    }

    void CheckEnemyArea()
    {
        if (!isBoss)                                            // Call EnemyAreaSize for Enemy Only
        {
            EnemyAreaSize();                                    // Change Enemy Area Size Big or Small
        }

        if (isIntoArea && !isDead)                              // Check Player Walk into Enemy Area
        {
            EnemyFollowPlayer();                                // Enemy Walking to Player

            if (isWarning == false)                             // Spawn Warning Prefab
            {
                isWarning = true;
                Vector2 warningPos = new Vector2(transform.position.x, transform.position.y + 0.5f);
                GameObject warningClone = Instantiate(warningPrefab, warningPos, Quaternion.identity);
                warningClone.transform.SetParent(gameObject.transform);
            }
        }
        else if (!isIntoArea)
        {
            isWarning = false;                                  // Reset Warning
            anim.SetBool("isWalk", false);                      // Enemy Stop Walking
        }

        // Follow Player in Boss Level
        if (GameManager.instance.level == 3 && GameManager.instance.point >= 4)
        {
            EnemyFollowPlayer();
        }
    }
    #endregion

    #region Damage and Hp Section
    void BeatByPlayer()
    {
        if (stackHit > 0)                                       // Player Hit to Enemy
        {
            isCritical = false;                                 // Reset isCritical
            float minDamage = 20 * ExperieneManager.instance.increaseDamagePercent;
            float maxDamage = 46 * ExperieneManager.instance.increaseDamagePercent;
            int Critical = Random.Range(1, 11);                 // Random Critical 1 - 10

            if (Critical == 10)                                 // Critical when Random Number 10
            {
                isCritical = true;
                minDamage = 50 * ExperieneManager.instance.increaseDamagePercent;
                maxDamage = 71 * ExperieneManager.instance.increaseDamagePercent;
            }

            damaged = Random.Range(minDamage, maxDamage);       // Random Damage from min and max value
            stackHit = 0;                                       // Reset Hit
        }

        if (damaged > 0 && !isPopup && !isDead)                 // Have Damage and Not have Damage Popup
        {
            if (isBoss)
            {
                SoundManager.instance.PlaySingle(beatBossSound);
            }
            else if (!isBoss)
            {
                SoundManager.instance.PlaySingle(beatSound);
            }

            DamagetoEnemy();
            EnemyHpCalculate();
            Invoke("ResetEnemy", 0.5f);
        }
    }

    void ResetEnemy()
    {
        damaged = 0;
        isPopup = false;
        spriteRenderer.color = new Color(255, 255, 255, 255);
    }

    void EnemyHpCalculate()
    {
        Vector3 barScaleX = enemyHpBar.transform.localScale;    // Save HpBar Scale to barScaleX
        hpEnemy -= damaged;                                     // Decrease HpEnemy by Damage
        barScaleX.x -= damaged / changeToHpBar;                 // Decrease EnemyHpBar Scale X
        enemyHpBar.transform.localScale = barScaleX;            // Set EnemyHpBar to new Scale
    }

    void DamagetoEnemy()
    {
        isPopup = true;
        spriteRenderer.color = new Color(100, 0, 0, 255);       // Set Enemy Sprite to Red

        if (!isBoss)
        {
            anim.SetTrigger("isBeat");                              // Set Animation 
        }

        if (isCritical)                                         // Spawn Critical Damage Popup
        {
            GameObject popupClone = Instantiate(criticalDamagePopupPrefab, new Vector3(transform.position.x + 0.2f, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            popupClone.GetComponent<TextMeshPro>().SetText("-" + damaged.ToString("0"));
            isCritical = false;                                 // Reset Critical
        }
        else if (!isCritical)                                   // Spawn Normal Damage Popup
        {
            GameObject popupClone = Instantiate(damagePopupPrefab, new Vector3(transform.position.x + 0.2f, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            popupClone.GetComponent<TextMeshPro>().SetText("-" + damaged.ToString("0"));
        }
    }

    void EnemyDead()
    {
        if (hpEnemy <= 0 && !isDead)
        {
            SoundManager.instance.PlaySingle(deadSound);
            isDead = true;                                      // Set Checking Value to True
            anim.SetBool("isDead", true);                       // Play Animation to Dead
            enemyHpBar.gameObject.SetActive(false);             // Hide Enemy Hp Bar
            StartCoroutine("BlinkEffect");                      // Play Blink Effect
            Invoke("StopBlinkEffect", 1);                       // Stop Blink Effect
            ExperieneManager.instance.isAddExp = true;
            ExperieneManager.instance.expPlayer += expEnemy;    // Add Exp To Player
            GameManager.instance.score += 1;                    // Score + 1
        }
    }
    #endregion

    #region Effect and Spawn Object Section
    IEnumerator BlinkEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(255, 255, 255, 0);
            shadowPrefab.SetActive(false);

            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(255, 255, 255, 255);
            shadowPrefab.SetActive(true);
        }
    }

    void StopBlinkEffect()
    {
        StopCoroutine("EnemyDeadAnimation");
        SpawnSmoke();                                           // Spawn Smoke Effect
        SpawnItem();                                            // Random Spawn Item
        Destroy(gameObject);                                    // Destroy Enemy
    }

    void SpawnSmoke()
    {
        GameObject cloneSmoke = Instantiate(smokeEffect, transform.position, transform.rotation);
        Destroy(cloneSmoke, 0.5f);
    }

    void SpawnItem()
    {
        int i = Random.Range(1, GameManager.instance.dropRate + 1);

        if (i == GameManager.instance.dropRate && !isBoss)       // Spawn Item for Enemy Only
        {
            GameObject hpPotionClone = Instantiate(hpPotion, transform.position, Quaternion.identity);
        }
    }
    #endregion

    void EnemyFollowPlayer()
    {
        FlipSprite();
        Vector3 direction = player.position - transform.position;                                   // Find Player Direction
        movement = direction;
        rb2d.MovePosition((Vector2)transform.position + (movement * speed * Time.deltaTime));       // Follow Player by Direction
        anim.SetBool("isWalk", true);
    }

    void SortingLayerOrder()
    {
        // Sorting Layer Order
        // Layer -2 = Shadow
        // Layer -1 = Enemy Hp Bar
        // Layer 0 = Enemy Behind Player and Environment
        // Layer 1 = Player
        // Layer 2 = Enemy Ahead Player
        // Layer 3 = Damage Popup and Warning

        if (shadowPrefab.transform.position.y < playerShadow.transform.position.y)      // Enemy ahead Player
        {
            spriteRenderer.sortingOrder = 2;
        }
        else if (shadowPrefab.transform.position.y > playerShadow.transform.position.y) // Enemy behind Player
        {
            spriteRenderer.sortingOrder = 0;
        }
    }

    void FlipSprite()
    {
        if (transform.position.x < player.transform.position.x) // Enemy On The Left of Player
        {
            if (!isBoss)
            {
                spriteRenderer.flipX = true;
            }
            else if (isBoss && !isBossAttack && !isDanger)
            {
                spriteRenderer.flipX = true;
                isPlayerRight = true;
                isPlayerLeft = false;
            }
        }

        if (transform.position.x > player.transform.position.x) // Enemy On The Right of Player
        {
            if (!isBoss)
            {
                spriteRenderer.flipX = false;
            }
            else if (isBoss && !isBossAttack && !isDanger)
            {
                spriteRenderer.flipX = false;
                isPlayerLeft = true;
                isPlayerRight = false;
            }
        }
    }

    void SetUpEnemy()
    {
        // Type Of Enemy
        // Type 1 = Jelly
        // Type 2 = Snail
        // Type 3 = Skeleton
        // Type 4 = Boss

        // HP Enemy Table
        // Level    [1]     [2]     [3]
        // Type 1   200     300     400
        // Type 2   400     500     600
        // Type 3   600     700     800

        // Exp Enemy Table
        // Level    [1]     [2]     [3]
        // Type 1   15,26   35,46   55,66
        // Type 2   35,46   55,66   75,86
        // Type 3   55,66   75,86   95,106

        if (GameManager.instance.level == 1)
        {
            if (enemyType == 1)
            {
                hpEnemy = 200;
                expEnemy = Random.Range(15, 26);
            }
            else if (enemyType == 2)
            {
                hpEnemy = 400;
                expEnemy = Random.Range(35, 46);
            }
            else if (enemyType == 3)
            {
                hpEnemy = 600;
                expEnemy = Random.Range(55, 66);
            }
        }

        if (GameManager.instance.level == 2)
        {
            if (enemyType == 1)
            {
                hpEnemy = 300;
                expEnemy = Random.Range(35, 46);
            }
            else if (enemyType == 2)
            {
                hpEnemy = 500;
                expEnemy = Random.Range(55, 66);
            }
            else if (enemyType == 3)
            {
                hpEnemy = 700;
                expEnemy = Random.Range(75, 86);
            }
        }

        if (GameManager.instance.level == 3)
        {
            if (enemyType == 1)
            {
                hpEnemy = 400;
                expEnemy = Random.Range(55, 66);
            }
            else if (enemyType == 2)
            {
                hpEnemy = 600;
                expEnemy = Random.Range(75, 86);
            }
            else if (enemyType == 3)
            {
                hpEnemy = 800;
                expEnemy = Random.Range(95, 106);
            }
        }

        if (enemyType == 4)
        {
            hpEnemy = 5000;
            expEnemy = Random.Range(201, 301);
            speed = 0.1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            // Enemy Attack Player
            if (isBehindPlayer && !isBoss && !isDead && other.gameObject.GetComponentInParent<PlayerController>().isWalk == false)
            {
                anim.SetTrigger("isAttack");
                other.gameObject.GetComponentInParent<PlayerController>().stackHit = 1;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            // Boss Attack Player
            if (isBoss && isBossAttack && !isBehindBoss && !isCooldown)
            {
                other.gameObject.GetComponentInParent<PlayerController>().stackHitBoss = 1;

                if (isPlayerLeft)
                {
                    other.gameObject.GetComponentInParent<PlayerController>().isPlayerLeft = true;
                    other.gameObject.GetComponentInParent<PlayerController>().isPlayerRight = false;
                }
                else if (isPlayerRight)
                {
                    other.gameObject.GetComponentInParent<PlayerController>().isPlayerRight = true;
                    other.gameObject.GetComponentInParent<PlayerController>().isPlayerLeft = false;
                }
            }

            if (isBoss && isBossHardAttack && !isCooldown)
            {
                other.gameObject.GetComponentInParent<PlayerController>().stackHitBoss = 1;

                if (isPlayerLeft)
                {
                    other.gameObject.GetComponentInParent<PlayerController>().isPlayerLeft = true;
                    other.gameObject.GetComponentInParent<PlayerController>().isPlayerRight = false;
                }
                else if (isPlayerRight)
                {
                    other.gameObject.GetComponentInParent<PlayerController>().isPlayerRight = true;
                    other.gameObject.GetComponentInParent<PlayerController>().isPlayerLeft = false;
                }
            }
        }
    }
}
