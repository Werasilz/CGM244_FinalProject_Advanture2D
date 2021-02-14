using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Navigator : MonoBehaviour
{
    private GameObject player;
    private GameObject destination;
    private GameObject distanceText;
    [HideInInspector] public Slider slider;
    public float distance;

    void Awake()
    {
        distanceText = GameObject.Find("DistanceText");
        player = GameObject.Find("Player");
        player.GetComponent<Transform>();
        destination = GameObject.Find("Destination");
        slider = GameObject.Find("NavigatorPoint").GetComponent<Slider>();
    }

    void Update()
    {
        if (destination.transform.position.x > player.transform.position.x)                 // Check Destination On the Right
        {
            slider.transform.localPosition = new Vector2(850, 0);                           // Set Navigator On the Right
            slider.value = destination.transform.position.y - player.transform.position.y;  // Set Value by Distance
            distance = player.transform.position.x - destination.transform.position.x;      // Find Distance
            distanceText.GetComponent<TextMeshProUGUI>().SetText(Mathf.Abs(distance).ToString("0.0") + " m");
        }
        else if (destination.transform.position.x < player.transform.position.x)            // Check Destination On the Left
        {
            slider.transform.localPosition = new Vector2(-850, 0);                          // Set Navigator On the Left
            slider.value = destination.transform.position.y - player.transform.position.y;  // Set Value by Distance
            distance = player.transform.position.x - destination.transform.position.x;      // Find Distance
            distanceText.GetComponent<TextMeshProUGUI>().SetText(Mathf.Abs(distance).ToString("0.0") + " m");
        }
    }
}
