using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [Header("English")]
    public string englishText;
    [Header("Hebrew")]
    public string hebrewText;
    [Header("Russian")]
    public string russianText;
    [Header("Arabic")]
    public string arabicText;
    [Header("Amharic")]
    public string amharicText;

    [Header("ENG")]
    public string engText;
    [Header("Chineese")]
    public string chineseText;
    [Header("Spanish")]
    public string spanishText;




    [Header("TMP Fonts")]
    public TMP_FontAsset amharicFont;
    public TMP_FontAsset chineseFont;   // ← add this
    public TMP_FontAsset defaultFont;


    [Header("Legacy Fonts")]
    public Font amharicLegacyFont;       // ← Noto Sans Ethiopic legacy .ttf
    public Font chineseLegacyFont;      // ← add this
    public Font defaultLegacyFont;       // ← your normal legacy font

    private TMP_Text tmpText;
    private Text uiText;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
        Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        string display = englishText;
        bool isRTL = false;
        bool isAmharic = LocalizationManager.currentLanguage == LocalizationManager.Language.Amharic;
        bool isChinese = LocalizationManager.currentLanguage == LocalizationManager.Language.Chinese;

        switch (LocalizationManager.currentLanguage)
        {
            case LocalizationManager.Language.Hebrew:
                display = hebrewText;
                isRTL = true;
                break;
            case LocalizationManager.Language.Russian:
                display = russianText;
                break;
            case LocalizationManager.Language.Arabic:
                display = arabicText;
                isRTL = true;
                break;
            case LocalizationManager.Language.Amharic:
                display = amharicText;
                break;
            case LocalizationManager.Language.Chinese:
                display = chineseText;
                break;
            case LocalizationManager.Language.Spanish:
                display = spanishText;
                break;
            case LocalizationManager.Language.ENG:
                display = engText;
                break;
            case LocalizationManager.Language.English:
            default:
                display = englishText;
                break;
        }

        if (tmpText != null)
        {
            // ← Pick correct TMP font
            if (isAmharic && amharicFont != null)
                tmpText.font = amharicFont;
            else if (isChinese && chineseFont != null)
                tmpText.font = chineseFont;
            else if (defaultFont != null)
                tmpText.font = defaultFont;

            tmpText.text = display;
            tmpText.isRightToLeftText = isRTL;
        }
        else if (uiText != null)
        {
            // ← Pick correct legacy font
            if (isAmharic && amharicLegacyFont != null)
                uiText.font = amharicLegacyFont;
            else if (isChinese && chineseLegacyFont != null)
                uiText.font = chineseLegacyFont;
            else if (defaultLegacyFont != null)
                uiText.font = defaultLegacyFont;

            uiText.text = display;
        }
    }
}