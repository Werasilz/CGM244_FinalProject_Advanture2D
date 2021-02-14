using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region Component
    private Rigidbody2D rb2d;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Local Scale Player
    private Vector3 normalScalePlayer = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 normalScalePlayerWithFlip = new Vector3(-0.5f, 0.5f, 0.5f);
    #endregion

    #region Player Status
    private float speed = 5;                                                // Player Walk Speed
    private bool isFlip;                                                    // Check Sprite is Flip
    [HideInInspector] public bool isWalk;                                   // Check Player Walk
    private bool isKnockBack;                                               // Check Player KnockBack
    private bool isKnockBackByBoss;                                         // Check Player KnockBack
    #endregion

    #region Value for Damage System
    [HideInInspector] public float stackHit;                                // Input Hit from Enemy
    [HideInInspector] public float stackHitBoss;                            // Input Hit from Boss
    private float damaged;                                                  // Damage to Decrease HpPlayer
    private bool isPopup;                                                   // Check Damage Popup is Show
    #endregion

    #region GameObject
    private VariableJoystick variableJoystick;
    public GameObject hitEffect;
    public GameObject damagePopupPrefab;
    #endregion

    #region Check Direction Player
    private bool isLeft;
    private bool isRight;
    private bool isFront;
    private bool isBack;
    public bool isPlayerLeft;                                               // Check Player is Left Of Boss
    public bool isPlayerRight;                                              // Check Player is Right Of Boss
    #endregion

    #region Hit Point For Effect Spawn
    private GameObject frontHitPoint;
    private GameObject backHitPoint;
    private GameObject leftHitPoint;
    private GameObject rightHitPoint;
    #endregion

    #region Audio
    public AudioClip attackSound;
    public AudioClip beatSound;
    #endregion

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        variableJoystick = GameObject.Find("Variable Joystick").GetComponent<VariableJoystick>();
        frontHitPoint = GameObject.Find("FrontHitPoint");
        backHitPoint = GameObject.Find("BackHitPoint");
        leftHitPoint = GameObject.Find("LeftHitPoint");
        rightHitPoint = GameObject.Find("RightHitPoint");
    }

    void FixedUpdate()
    {
        BeatByEnemy();
        BeatByBoss();

        if (!GameManager.instance.isComplete)
        {
            if (!GameManager.instance.playWithJoyStick)
            {
                MovementKeyBoard();
            }
            else
            {
                MovementJoyStick();
            }
        }
    }

    void Update()
    {
        FlipSprite();
        KnockBack();

        if (!GameManager.instance.isComplete)
        {
            if (!GameManager.instance.playWithJoyStick)
            {
                AnimationPlayerWalkKeyBoard();
                PlayerAttackKeyBoard();
            }
            else
            {
                AnimationPlayerWalkJoyStick();
            }
        }

        if (GameManager.instance.hpPlayer <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    #region KnockBack Section
    void KnockBack()
    {
        if (isKnockBack)                                                    // Check Knockback by Player Direction 
        {
            Invoke("ResetKnockBack", 0.5f);
            float knockbackForce = 100;

            if (isRight)
            {
                rb2d.AddForce(Vector2.right * knockbackForce);
            }
            else if (isLeft)
            {
                rb2d.AddForce(Vector2.left * knockbackForce);
            }
            else if (isFront)
            {
                rb2d.AddForce(Vector2.down * knockbackForce);
            }
            else if (isBack)
            {
                rb2d.AddForce(Vector2.up * knockbackForce);
            }
        }

        if (isKnockBackByBoss)
        {
            Invoke("ResetKnockBack", 0.5f);
            float knockbackForce = 100;

            if (isPlayerLeft)
            {
                rb2d.AddForce(Vector2.left * knockbackForce);
            }
            else if (isPlayerRight)
            {
                rb2d.AddForce(Vector2.right * knockbackForce);
            }
        }
    }

    void ResetKnockBack()
    {
        isKnockBack = false;
        isKnockBackByBoss = false;
    }
    #endregion

    #region Play With KeyBoard
    void MovementKeyBoard()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical);
        Vector2 newPos = new Vector2(transform.position.x, transform.position.y);
        newPos = newPos + movement * speed * Time.deltaTime;
        rb2d.MovePosition(newPos);
    }

    void AnimationPlayerWalkKeyBoard()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        anim.SetFloat("HorizontalSpeed", Mathf.Abs(horizontal));
        anim.SetFloat("VerticalSpeed", Mathf.Abs(vertical));

        #region Walking
        if (horizontal != 0 || vertical != 0)
        {
            isWalk = true;
            anim.SetBool("isCross", false);

            if (horizontal > 0)                                             // Right Direction
            {
                isFlip = true;
            }
            else if (horizontal < 0)                                        // Left Direction
            {
                isFlip = false;
            }
            else if (vertical > 0)                                          // Up Direction
            {
                anim.SetBool("isDown", false);
                anim.SetBool("isUp", true);
            }
            else if (vertical < 0)                                          // Down Direction
            {
                anim.SetBool("isDown", true);
                anim.SetBool("isUp", false);
            }
        }
        else
        {
            isWalk = false;
            anim.SetBool("isDown", false);
            anim.SetBool("isUp", false);
            anim.SetBool("isCross", false);
        }
        #endregion

        #region Set Cross Direction
        if (horizontal > 0 && vertical > 0 || horizontal < 0 && vertical < 0 || horizontal > 0 && vertical < 0 || horizontal < 0 && vertical > 0)
        {
            anim.SetBool("isCross", true);
        }
        #endregion

        #region Check Direction by bool
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LeftWalk") || anim.GetCurrentAnimatorStateInfo(0).IsName("LeftIdle"))
        {
            // Walk Left
            if (!isFlip)
            {
                isLeft = true;
                isRight = false;

                isBack = false;
                isFront = false;
            }
            // Walk Right
            else if (isFlip)
            {
                isLeft = false;
                isRight = true;

                isBack = false;
                isFront = false;
            }
        }

        // Walk Front
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FrontWalk") || anim.GetCurrentAnimatorStateInfo(0).IsName("FrontIdle"))
        {
            isFront = true;
            isBack = false;

            isLeft = false;
            isRight = false;

            isFlip = false;
        }

        // Walk Back
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BackWalk") || anim.GetCurrentAnimatorStateInfo(0).IsName("BackIdle"))
        {
            isFront = false;
            isBack = true;

            isLeft = false;
            isRight = false;

            isFlip = false;
        }
        #endregion
    }

    void PlayerAttackKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isWalk && !GameManager.instance.isPause)
        {
            if (isLeft || isRight)
            {
                anim.SetTrigger("Attack");
                HitEffect();
                SoundManager.instance.PlaySingle(attackSound);
            }
            else if (isFront)
            {
                isFlip = false;
                anim.SetTrigger("FrontAttack");
                HitEffect();
                SoundManager.instance.PlaySingle(attackSound);
            }
            else if (isBack)
            {
                isFlip = false;
                anim.SetTrigger("BackAttack");
                HitEffect();
                SoundManager.instance.PlaySingle(attackSound);
            }
        }
    }
    #endregion

    #region Play With JoyStick
    void MovementJoyStick()
    {
        float horizontal = variableJoystick.Horizontal;
        float vertical = variableJoystick.Vertical;
        Vector2 movement = new Vector2(horizontal, vertical);
        Vector2 newPos = new Vector2(transform.position.x, transform.position.y);
        newPos = newPos + movement * speed * Time.deltaTime;
        rb2d.MovePosition(newPos);
    }

    void AnimationPlayerWalkJoyStick()
    {
        float horizontal = variableJoystick.Horizontal;
        float vertical = variableJoystick.Vertical;
        anim.SetFloat("HorizontalSpeed", Mathf.Abs(horizontal));
        anim.SetFloat("VerticalSpeed", Mathf.Abs(vertical));

        #region Walking
        if (horizontal != 0 || vertical != 0)
        {
            isWalk = true;
            anim.SetBool("isCross", false);


            if (horizontal > 0)                                             // Right Direction
            {
                isFlip = true;
            }
            else if (horizontal < 0)                                        // Left Direction
            {
                isFlip = false;
            }
            else if (vertical > 0)                                          // Up Direction
            {
                anim.SetBool("isDown", false);
                anim.SetBool("isUp", true);
            }
            else if (vertical < 0)                                          // Down Direction
            {
                anim.SetBool("isDown", true);
                anim.SetBool("isUp", false);
            }
        }
        else
        {
            isWalk = false;
            anim.SetBool("isDown", false);
            anim.SetBool("isUp", false);
            anim.SetBool("isCross", false);
        }
        #endregion

        #region Set Cross Direction
        if (horizontal > 0 && vertical > 0 || horizontal < 0 && vertical < 0 || horizontal > 0 && vertical < 0 || horizontal < 0 && vertical > 0)
        {
            anim.SetBool("isCross", true);
        }
        #endregion

        #region Check Direction by bool
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LeftWalk") || anim.GetCurrentAnimatorStateInfo(0).IsName("LeftIdle"))
        {
            // Walk Left
            if (!isFlip)
            {
                isLeft = true;
                isRight = false;

                isBack = false;
                isFront = false;
            }
            // Walk Right
            else if (isFlip)
            {
                isLeft = false;
                isRight = true;

                isBack = false;
                isFront = false;
            }
        }

        // Walk Front
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FrontWalk") || anim.GetCurrentAnimatorStateInfo(0).IsName("FrontIdle"))
        {
            isFront = true;
            isBack = false;

            isLeft = false;
            isRight = false;

            isFlip = false;
        }

        // Walk Back
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BackWalk") || anim.GetCurrentAnimatorStateInfo(0).IsName("BackIdle"))
        {
            isFront = false;
            isBack = true;

            isLeft = false;
            isRight = false;

            isFlip = false;
        }
        #endregion
    }

    public void PlayerAttackJoyStick()
    {
        if (!isWalk && !GameManager.instance.isPause)
        {
            if (isLeft || isRight)
            {
                anim.SetTrigger("Attack");
                HitEffect();
                SoundManager.instance.PlaySingle(attackSound);
            }
            else if (isFront)
            {
                isFlip = false;
                anim.SetTrigger("FrontAttack");
                HitEffect();
                SoundManager.instance.PlaySingle(attackSound);
            }
            else if (isBack)
            {
                isFlip = false;
                anim.SetTrigger("BackAttack");
                HitEffect();
                SoundManager.instance.PlaySingle(attackSound);
            }
        }
    }
    #endregion

    #region Damage Section
    void BeatByEnemy()
    {
        if (stackHit > 0)                                                   // Enemy Hit to Player
        {
            int minDamage = 5;                                              // Set min and max Damage
            int maxDamage = 16;
            damaged = Random.Range(minDamage, maxDamage);                   // Random Damage from min and max value
            stackHit = 0;                                                   // Reset Hit
        }

        if (damaged > 0 && !isPopup)                                        // Have Damage and Not have Damage Popup
        {
            SoundManager.instance.PlaySingle(beatSound);
            isPopup = true;
            GameManager.instance.hpPlayer -= damaged;                       // Decrease HpEnemy by Damage
            SpawnPopupDamage();

            if (isFront || isBack || isLeft || isRight)                     // Knockback
            {
                anim.SetTrigger("isBeat");
                isKnockBack = true;
            }

            Invoke("ResetPlayer", 0.5f);
        }
    }

    void BeatByBoss()
    {
        if (stackHitBoss > 0)                                               // Boss Hit to Player
        {
            int minDamage = 20 * GameManager.instance.damageDouble;         // Set min and max Damage
            int maxDamage = 31 * GameManager.instance.damageDouble;
            damaged = Random.Range(minDamage, maxDamage);                   // Random Damage from min and max value
            stackHitBoss = 0;                                               // Reset Hit
        }

        if (damaged > 0 && !isPopup)                                        // Have Damage and Not have Damage Popup
        {
            SoundManager.instance.PlaySingle(beatSound);
            isPopup = true;
            GameManager.instance.hpPlayer -= damaged;                       // Decrease HpEnemy by Damage
            SpawnPopupDamage();

            if (isPlayerLeft || isPlayerRight)                              // Knockback
            {
                anim.SetTrigger("isBeat");
                isKnockBackByBoss = true;
            }

            Invoke("ResetPlayer", 0.5f);
        }
    }

    void SpawnPopupDamage()
    {
        GameObject popupClone = Instantiate(damagePopupPrefab, new Vector3(transform.position.x + 0.2f, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
        popupClone.GetComponent<TextMeshPro>().SetText("-" + damaged.ToString("0"));
        popupClone.transform.SetParent(GameObject.Find("LevelPlayerText").transform);
        spriteRenderer.color = new Color(100, 0, 0, 255);
    }

    void ResetPlayer()
    {
        damaged = 0;
        isPopup = false;
        spriteRenderer.color = new Color(255, 255, 255, 255);
    }
    #endregion

    void HitEffect()
    {
        if (isLeft)
        {
            GameObject hitClone = Instantiate(hitEffect, leftHitPoint.transform.position, Quaternion.identity);
            hitClone.gameObject.GetComponent<Animator>().SetTrigger("hitLeft");
            Destroy(hitClone, 0.3f);
        }
        else if (isRight)
        {
            GameObject hitClone = Instantiate(hitEffect, rightHitPoint.transform.position, Quaternion.identity);
            hitClone.gameObject.GetComponent<Animator>().SetTrigger("hitRight");
            Destroy(hitClone, 0.3f);
        }
        else if (isFront)
        {
            GameObject hitClone = Instantiate(hitEffect, frontHitPoint.transform.position, Quaternion.identity);
            hitClone.gameObject.GetComponent<Animator>().SetTrigger("hitFront");
            Destroy(hitClone, 0.3f);
        }
        else if (isBack)
        {
            GameObject hitClone = Instantiate(hitEffect, backHitPoint.transform.position, Quaternion.identity);
            hitClone.gameObject.GetComponent<Animator>().SetTrigger("hitBack");
            Destroy(hitClone, 0.3f);
        }
    }

    void FlipSprite()
    {
        if (isFlip)
        {
            transform.localScale = normalScalePlayerWithFlip;
        }
        else if (!isFlip)
        {
            transform.localScale = normalScalePlayer;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player Attack Enemy
        if (other.CompareTag("Enemy"))
        {
            if (other.transform.position.x < transform.position.x)          // Enemy on the left
            {
                if (!isFlip && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftAttack"))
                {
                    other.gameObject.GetComponent<EnemyController>().stackHit = 1;
                }

                if (isRight)                                                // Check Enemy Behind Player
                {
                    other.gameObject.GetComponent<EnemyController>().isBehindPlayer = true;
                }
                else if (isLeft)
                {
                    other.gameObject.GetComponent<EnemyController>().isBehindPlayer = false;
                }
            }

            if (other.transform.position.x > transform.position.x)          // Enemy on the Right
            {
                if (isFlip && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftAttack"))
                {
                    other.gameObject.GetComponent<EnemyController>().stackHit = 1;
                }

                if (isLeft)                                                 // Check Enemy Behind Player
                {
                    other.gameObject.GetComponent<EnemyController>().isBehindPlayer = true;
                }
                else if (isRight)
                {
                    other.gameObject.GetComponent<EnemyController>().isBehindPlayer = false;
                }
            }

            if (other.transform.position.y < transform.position.y)          // Enemy on the Bottom
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("FrontAttack"))
                {
                    other.gameObject.GetComponent<EnemyController>().stackHit = 1;
                }

                if (isBack)                                                 // Check Enemy Behind Player
                {
                    other.gameObject.GetComponent<EnemyController>().isBehindPlayer = true;
                }
                else if (isFront)
                {
                    other.gameObject.GetComponent<EnemyController>().isBehindPlayer = false;
                }
            }

            if (other.transform.position.y > transform.position.y)          // Enemy on the Top
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("BackAttack"))
                {
                    other.gameObject.GetComponent<EnemyController>().stackHit = 1;
                }

                if (isFront)                                                // Check Enemy Behind Player
                {
                    other.gameObject.GetComponent<EnemyController>().isBehindPlayer = true;
                }
                else if (isBack)
                {
                    other.gameObject.GetComponent<EnemyController>().isBehindPlayer = false;
                }
            }
        }

        if (other.CompareTag("Boss"))
        {
            if (other.transform.position.x < transform.position.x)          // Enemy on the left
            {
                if (!isFlip && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftAttack"))
                {
                    other.gameObject.GetComponentInParent<EnemyController>().stackHit = 1;
                }
            }

            if (other.transform.position.x > transform.position.x)          // Enemy on the Right
            {
                if (isFlip && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftAttack"))
                {
                    other.gameObject.GetComponentInParent<EnemyController>().stackHit = 1;
                }
            }

            if (other.transform.position.y < transform.position.y)          // Enemy on the Bottom
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("FrontAttack"))
                {
                    other.gameObject.GetComponentInParent<EnemyController>().stackHit = 1;
                }
            }

            if (other.transform.position.y > transform.position.y)          // Enemy on the Top
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("BackAttack"))
                {
                    other.gameObject.GetComponentInParent<EnemyController>().stackHit = 1;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyArea"))                                  // Check Player Enter Enemy Area
        {
            other.GetComponentInParent<EnemyController>().isIntoArea = true;
        }

        if (other.gameObject.CompareTag("Destination"))                     // Check Player Arrive to Destinationn
        {
            if (!GameManager.instance.isArrive)
            {
                GameManager.instance.point += 1;                            // Set Next Destination Point
                GameManager.instance.isArrive = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyArea"))                                  // Check Player Exit Enemy Area
        {
            other.GetComponentInParent<EnemyController>().isIntoArea = false;
        }

        if (other.CompareTag("Enemy"))                                      // Reset variable isBehindPlayer when Player exit Enemy Area
        {
            other.gameObject.GetComponent<EnemyController>().isBehindPlayer = false;
        }

        if (other.gameObject.CompareTag("Destination"))                     // Reset Check Arrive Destination Value
        {
            GameManager.instance.isArrive = false;
        }
    }

}
