using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int pauseCoolDown = 0;
    // public bool movementEnabled = true;
    public static bool isPaused = false;
    // public static bool isDead = false;
    // Start is called before the first frame update

    // Update is called once per frame

    public GameObject ingameMenu;
    public GameObject deadMenu;
    public AudioSource music;

    private void Start()
    {
        music.Play();
    }

    private void Update()
    {
        // if (deadMenu.activeSelf)
        // {
        //     return;
        // }
        // print(InputManager.PauseWasPressed);
        if (InputManager.PauseWasPressed && !isPaused && pauseCoolDown > 30)
        {
            pauseCoolDown = 0;
            Time.timeScale = 0f;
            ingameMenu.SetActive(true);
            isPaused = true;
        }
        else if (InputManager.PauseWasPressed && isPaused && pauseCoolDown > 60)
        {
            pauseCoolDown = 0;
            Time.timeScale = 1f;
            ingameMenu.SetActive(false);
            isPaused = false;
        }
        // print(Time.timeScale);
        pauseCoolDown += 1;
    }

    public void switchPauseStats()
    {
        // if (deadMenu.activeSelf) return;
        if (!isPaused)
        {
            Time.timeScale = 0f;
            ingameMenu.SetActive(true);
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            ingameMenu.SetActive(false);
            isPaused = false;
        }
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene(); // Get the current scene
        SceneManager.LoadScene(currentScene.name); // Reload the scene by its name
        Time.timeScale = 1f;
        isPaused = false;
        // deadMenu.SetActive(false);
    }

    // public void EnableDeadMenu()
    // {
    //     deadMenu.SetActive(true);
    //     Time.timeScale = 0f;
    //     isDead = true;
    // }
}
