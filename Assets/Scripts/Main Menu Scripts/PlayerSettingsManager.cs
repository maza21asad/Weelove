using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSettingsManager : MonoBehaviour
{
    public static PlayerSettingsManager Instance;

    public string selectedRegion;
    public string selectedMode;
    public string selectedLanguage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("Region", selectedRegion);
        PlayerPrefs.SetString("Language", selectedLanguage);
        PlayerPrefs.SetString("Mode", selectedMode);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        selectedRegion = PlayerPrefs.GetString("Region", string.Empty);
        selectedMode = PlayerPrefs.GetString("Mode", string.Empty);
        selectedLanguage = PlayerPrefs.GetString("Language", string.Empty);
    }

    public bool IsFirstLaunch()
    {
        return string.IsNullOrEmpty(selectedRegion) || string.IsNullOrEmpty(selectedLanguage) || string.IsNullOrEmpty(selectedMode) ;
    }

    public void SwitchScreen(int screenId)
    {
        SceneManager.LoadScene(screenId);
    }
}
