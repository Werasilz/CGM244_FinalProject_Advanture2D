using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryManager : MonoBehaviour
{
    private void Awake()
    {
        SoundManager.instance.StopMusic();
    }

    void Start()
    {
        SoundManager.instance.PlayerMusic();
        Destroy(GameObject.Find("ExperienceManager"));
        Destroy(GameObject.Find("GameManager"));
    }
}
