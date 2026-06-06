using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Config Data")]
    public GameSettingsData settingsData; // ScriptableObject reference

    [Header("Panels")]
    public GameObject chooseRegionPanel;
    public GameObject chooseModePanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("UI References")]
    public TMP_Dropdown regionDropdown;
    public TMP_Dropdown languageDropdown;
    public TMP_Text modeText;
    public TMP_Text modeLockedText;
    public TMP_Text regionText;
    public TMP_Text languageText;

    [Header("Back Buttons")]
    public Button regionBackButton;
    public Button modeBackButton;

    private int currentRegionIndex = -1;
    private bool comingFromMainMenu = false;

    private static bool hasOpenedOnce = false;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        // If returning from another scene
        if (hasOpenedOnce)
        {
            ShowMainMenu();
            return;
        }

        // First app open
        hasOpenedOnce = true;

        ShowChooseRegion(false);
    }

    // REGION + LANGUAGE
    public void ShowChooseRegion(bool fromMainMenu)
    {
        HideAll();
        chooseRegionPanel.SetActive(true);
        comingFromMainMenu = fromMainMenu;

        // Show or hide back button
        regionBackButton.gameObject.SetActive(fromMainMenu);

        // Populate regions dropdown
        regionDropdown.ClearOptions();
        var regionNames = settingsData.regions.ConvertAll(r => r.regionName);
        regionDropdown.AddOptions(regionNames);

        // Listen for region change
        regionDropdown.onValueChanged.RemoveAllListeners();
        regionDropdown.onValueChanged.AddListener(OnRegionChanged);

        // Preselect saved region if coming from main menu
        if (fromMainMenu)
        {
            int savedRegionIndex = settingsData.regions.FindIndex(r => r.regionName == PlayerSettingsManager.Instance.selectedRegion);
            if (savedRegionIndex >= 0)
                regionDropdown.value = savedRegionIndex;
            else
                regionDropdown.value = 0;
        }
        else
        {
            regionDropdown.value = 0;
        }

        OnRegionChanged(regionDropdown.value);
    }

    private void OnRegionChanged(int regionIndex)
    {
        currentRegionIndex = regionIndex;
        PlayerSettingsManager.Instance.selectedRegion = settingsData.regions[regionIndex].regionName;

        // Update languages based on region
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(settingsData.regions[regionIndex].languages);

        // Listen for language change
        languageDropdown.onValueChanged.RemoveAllListeners();
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        // Preselect saved language if coming from main menu
        if (comingFromMainMenu)
        {
            int savedLangIndex = settingsData.regions[regionIndex].languages.FindIndex(l => l == PlayerSettingsManager.Instance.selectedLanguage);
            languageDropdown.value = savedLangIndex >= 0 ? savedLangIndex : 0;
        }
        else
        {
            languageDropdown.value = 0;
            PlayerSettingsManager.Instance.selectedLanguage = settingsData.regions[regionIndex].languages[0];
        }
    }

    private void OnLanguageChanged(int languageIndex)
    {
        PlayerSettingsManager.Instance.selectedLanguage =
            settingsData.regions[currentRegionIndex].languages[languageIndex];
    }

    public void ConfirmRegionAndLanguage()
    {
        PlayerSettingsManager.Instance.Save();

        // If player already selected a mode before
        if (!string.IsNullOrEmpty(PlayerSettingsManager.Instance.selectedMode))
        {
            ShowMainMenu();
        }
        else
        {
            // First time only
            ShowChooseMode(false);
        }
    }

    // MODE
    public void SetMode(int modeIndex)
    {
        var mode = settingsData.modes[modeIndex];

        // Hide message by default
        modeLockedText.gameObject.SetActive(false);

        if (!mode.isUnlocked)
        {
            Debug.Log("Mode is locked");
            modeLockedText.text = "This mode is locked";
            modeLockedText.gameObject.SetActive(true);
            return;
        }

        //PlayerSettingsManager.Instance.selectedMode = settingsData.modes[modeIndex];
        PlayerSettingsManager.Instance.selectedMode = mode.modeName;
        PlayerSettingsManager.Instance.Save();

        if (comingFromMainMenu)
            ShowMainMenu();
        else
            ShowMainMenu();
    }

    public void ShowChooseMode(bool fromMainMenu)
    {
        HideAll();
        chooseModePanel.SetActive(true);
        comingFromMainMenu = fromMainMenu;

        // Show or hide back button
        modeBackButton.gameObject.SetActive(fromMainMenu);
    }

    // MAIN MENU
    public void ShowMainMenu()
    {
        HideAll();
        mainMenuPanel.SetActive(true);

        regionText.text = $"Current Region: {PlayerSettingsManager.Instance.selectedRegion}";
        modeText.text = $"Current Mode: {PlayerSettingsManager.Instance.selectedMode}";
        languageText.text = $"Current Language: {PlayerSettingsManager.Instance.selectedLanguage}";
    }

    public void ShowSettings()
    {
        HideAll();
        settingsPanel.SetActive(true);
    }

    private void HideAll()
    {
        chooseRegionPanel.SetActive(false);
        chooseModePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void SwitchScreen(int screenId)
    {
        SceneManager.LoadScene(screenId);
    }
}