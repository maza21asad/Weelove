//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement; // Needed for scene loading

//public class Canvas : MonoBehaviour
//{


//    public void LoadMainMenu()
//    { 
//               SceneManager.LoadScene(0); 
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Canvas : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;

    private bool isPaused = false;

    // ---------------- PAUSE GAME ----------------
    public void PauseGame()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f; // Freeze game
        isPaused = true;
    }

    // ---------------- RESUME GAME ----------------
    public void ResumeGame()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
        isPaused = false;
    }

    // ---------------- RELOAD CURRENT SCENE ----------------
    public void ReloadGame()
    {
        Time.timeScale = 1f; // IMPORTANT (reset before reload)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ---------------- LOAD MAIN MENU ----------------
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // IMPORTANT (reset before switching)

        AudioListener.pause = false;
        SceneManager.LoadScene(0);
    }
}