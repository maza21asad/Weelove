using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the new scene's QuestionManager and reset it
        QuestionManager qm = FindObjectOfType<QuestionManager>();
        if (qm != null)
        {
            qm.OnSceneReloaded();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
