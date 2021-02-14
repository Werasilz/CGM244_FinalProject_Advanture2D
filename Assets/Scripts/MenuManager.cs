using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public AudioClip clickSound;

    public void LoadGamePlay()
    {
        SoundManager.instance.PlaySingle(clickSound);
        SceneManager.LoadScene("Level1");
    }

    public void ExitGame()
    {
        SoundManager.instance.PlaySingle(clickSound);
        Application.Quit();
    }

    public void NextLevel()
    {
        SoundManager.instance.PlaySingle(clickSound);
        GameManager.instance.isComplete = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void MainMenu()
    {
        SoundManager.instance.PlaySingle(clickSound);
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        SoundManager.instance.PlaySingle(clickSound);

        if (GameManager.instance.level == 1)
        {
            GameManager.instance.hpPlayer = GameManager.instance.maxHpPlayer;
            //GameManager.instance.isGameover = false;
            GameManager.instance.isRestart = true;
            SceneManager.LoadScene("Level1");
        }

        if (GameManager.instance.level == 2)
        {
            GameManager.instance.hpPlayer = GameManager.instance.maxHpPlayer;
            //GameManager.instance.isGameover = false;
            SceneManager.LoadScene("Level2");
        }

        if (GameManager.instance.level == 3)
        {
            GameManager.instance.hpPlayer = GameManager.instance.maxHpPlayer;
            //GameManager.instance.isGameover = false;
            SceneManager.LoadScene("Level3");
        }
    }

    public void PauseButton()
    {
        SoundManager.instance.PlaySingle(clickSound);

        if (!GameManager.instance.isPause)
        {
            GameManager.instance.isPause = true;
        }
        else if (GameManager.instance.isPause)
        {
            GameManager.instance.isPause = false;
        }
    }

    public void SoundButton()
    {
        SoundManager.instance.PlaySingle(clickSound);

        if (SoundManager.instance.musicSource.volume > 0)
        {
            SoundManager.instance.musicSource.volume = 0;
            SoundManager.instance.efxSource.volume = 0;
        }
        else if (SoundManager.instance.musicSource.volume == 0)
        {
            SoundManager.instance.musicSource.volume = 0.5f;
            SoundManager.instance.efxSource.volume = 1;
        }
    }

    public void CreditsButton()
    {
        SoundManager.instance.PlaySingle(clickSound);
        SceneManager.LoadScene("Credits");
    }
}
