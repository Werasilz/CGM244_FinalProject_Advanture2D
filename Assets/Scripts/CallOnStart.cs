using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallOnStart : MonoBehaviour
{
    void Awake()
    {
        if (GameManager.instance.isRestart && GameManager.instance.level == 1)
        {
            GameManager.instance.isRestart = false;
            GameManager.instance.GetComponent();
            GameManager.instance.SetLevel();
            GameManager.instance.SetUpOnStart();
            ExperieneManager.instance.GetComponent();
            SoundManager.instance.StopMusic();
            SoundManager.instance.PlayerMusic();
        }
        else
        {
            GameManager.instance.GetComponent();
            GameManager.instance.SetLevel();
            GameManager.instance.SetUpOnStart();
            ExperieneManager.instance.GetComponent();
            SoundManager.instance.StopMusic();
            SoundManager.instance.PlayerMusic();
        }
    }
}
