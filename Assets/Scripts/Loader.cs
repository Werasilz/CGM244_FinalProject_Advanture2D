using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject soundManager;
    public GameObject experienceManager;

    void Awake()
    {
        if (GameManager.instance == null)
        {
            GameObject gameManagerClone = Instantiate(gameManager);
            gameManagerClone.transform.name = "GameManager";

            GameObject experienceManagerClone = Instantiate(experienceManager);
            experienceManagerClone.transform.name = "ExperienceManager";

            GameObject soundManagerClone = Instantiate(soundManager);
            soundManagerClone.transform.name = "SoundManager";
        }
    }
}
