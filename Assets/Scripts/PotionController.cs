using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PotionController : MonoBehaviour
{
    public GameObject healthPopup;
    private GameObject LevelPlayerText;
    public AudioClip potionSound;
    public GameObject hpEffect;

    private void Start()
    {
        LevelPlayerText = GameObject.Find("LevelPlayerText");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            SoundManager.instance.PlaySingle(potionSound);
            GameObject effectClone = Instantiate(hpEffect, other.transform.position, Quaternion.identity);
            effectClone.transform.SetParent(other.gameObject.transform);
            Destroy(effectClone, 1);
            int hpValue = Random.Range(5, 11);                                              // Random HpValue
            GameManager.instance.hpPlayer += hpValue;                                       // Add To HpPlayer
            GameObject healthPopupClone = Instantiate(healthPopup, other.transform.position, Quaternion.identity);
            healthPopupClone.GetComponent<TextMeshPro>().SetText("+" + hpValue.ToString());
            healthPopupClone.transform.SetParent(LevelPlayerText.transform);
            Destroy(gameObject);
        }
    }
}
