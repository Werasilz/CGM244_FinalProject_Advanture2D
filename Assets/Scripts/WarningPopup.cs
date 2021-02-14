using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningPopup : MonoBehaviour
{
    private Color textColor;
    private SpriteRenderer spriteRenderer;
    private float disappearTime;
    public bool isFinish;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        disappearTime = 0.5f;                                                       // Time To Disappear
        textColor = spriteRenderer.color;                                           // Save Color To textColor
    }

    private void Update()
    {
        float moveY = 0.3f;
        transform.position += new Vector3(0, moveY) * Time.deltaTime;               // Text Move Up
        disappearTime -= Time.deltaTime;                                            // Decrease DisappearTime
        transform.localScale += new Vector3(0.02f, 0.02f, 0.02f) * Time.deltaTime;  // Increase Text Scale

        if (disappearTime < 0)                                                      // FadeAlpha when Disappear < 0
        {
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            spriteRenderer.color = textColor;
        }

        if (textColor.a < 0)                                                        // Destroy When Alpha < 0
        {
            Destroy(gameObject);
        }
    }
}
