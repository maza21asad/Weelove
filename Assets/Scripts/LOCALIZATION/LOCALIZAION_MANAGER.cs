//using UnityEngine;

//public class LocalizationManager : MonoBehaviour
//{
//    public enum Language
//    {
//        English,
//        Hebrew,
//        Russian,
//        Arabic,
//        Amharic, // ← add this

//        ENG,
//        Chinese,
//        Spanish

//    }

//    public static Language currentLanguage = Language.English;

//    public void SetEnglish() { currentLanguage = Language.English; RefreshAll(); }
//    public void SetHebrew() { currentLanguage = Language.Hebrew; RefreshAll(); }
//    public void SetRussian() { currentLanguage = Language.Russian; RefreshAll(); }
//    public void SetArabic() { currentLanguage = Language.Arabic; RefreshAll(); }
//    public void SetAmharic() { currentLanguage = Language.Amharic; RefreshAll(); } // ← add this

//    public void SetEng() { currentLanguage = Language.ENG; RefreshAll(); }
//    public void SetChinese() { currentLanguage = Language.Chinese; RefreshAll(); }
//    public void SetSpanish() { currentLanguage = Language.Spanish; RefreshAll(); }

//    void RefreshAll()
//    {
//        LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();
//        foreach (var t in allTexts)
//            t.Refresh();
//    }
//}

using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public enum Language
    {
        English,
        Hebrew,
        Russian,
        Arabic,
        Amharic,

        ENG,
        Chinese,
        Spanish
    }

    [Header("Default Language")]
    [SerializeField] private Language defaultLanguage = Language.Hebrew;

    public static Language currentLanguage;

    private void Awake()
    {
        // If player already selected language before
        if (!string.IsNullOrEmpty(PlayerSettingsManager.Instance.selectedLanguage))
        {
            // Convert saved string to enum
            if (System.Enum.TryParse(
                PlayerSettingsManager.Instance.selectedLanguage,
                out Language savedLanguage))
            {
                currentLanguage = savedLanguage;
            }
            else
            {
                currentLanguage = defaultLanguage;
            }
        }
        else
        {
            // First launch
            currentLanguage = defaultLanguage;
        }

        RefreshAll();
    }

    public void SetEnglish() { SetLanguage(Language.English); }
    public void SetHebrew() { SetLanguage(Language.Hebrew); }
    public void SetRussian() { SetLanguage(Language.Russian); }
    public void SetArabic() { SetLanguage(Language.Arabic); }
    public void SetAmharic() { SetLanguage(Language.Amharic); }

    public void SetEng() { SetLanguage(Language.ENG); }
    public void SetChinese() { SetLanguage(Language.Chinese); }
    public void SetSpanish() { SetLanguage(Language.Spanish); }

    private void SetLanguage(Language language)
    {
        currentLanguage = language;

        // Save selected language
        PlayerSettingsManager.Instance.selectedLanguage = language.ToString();
        PlayerSettingsManager.Instance.Save();

        RefreshAll();
    }

    void RefreshAll()
    {
        LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();

        foreach (var t in allTexts)
            t.Refresh();
    }
}