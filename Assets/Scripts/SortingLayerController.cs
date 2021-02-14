using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private GameObject player;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player").GetComponent<Transform>().gameObject;
    }

    void Update()
    {
        SortingLayerOrder();
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

        if (transform.position.y < player.transform.position.y)
        {
            spriteRenderer.sortingOrder = 2;
        }
        else if (transform.position.y > player.transform.position.y)
        {
            spriteRenderer.sortingOrder = 0;
        }
    }
}
