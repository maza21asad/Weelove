using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;




//#if UNITY_EDITOR
//using System.Speech.Synthesis;
//#endif


public class AndroidTTS : MonoBehaviour
{
    public static AndroidTTS instance;

    private bool isReady = false;
    private bool isEnabled = true;




#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _IOSSpeak(string text, string languageCode);

    [DllImport("__Internal")]
    private static extern void _IOSStop();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject tts;
    private AndroidJavaObject activity;
#endif

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitTTS();
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        if (!enabled) StopTTSSpeech();
        Debug.Log($"TTS state set to: {enabled}");
    }

    private void InitTTS()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    {
        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }

    tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity,
        new TTSInitListener(() =>
        {
            isReady = true;
            Debug.Log("TTS Ready ✅");
            // ← Auto download all languages on first init
            StartCoroutine(DownloadAllLanguages());
        })
    );
#elif UNITY_IOS && !UNITY_EDITOR
    isReady = true;
#else
        isReady = true;
#endif
    }

    private IEnumerator DownloadAllLanguages()
    {
        yield return new WaitForSeconds(1f);

#if UNITY_ANDROID && !UNITY_EDITOR
    string[][] languages = new string[][]
    {
        new string[] { "ar", "SA" },  // Arabic
        new string[] { "iw", "IL" },  // Hebrew
        new string[] { "ru", "RU" },  // Russian
        new string[] { "am", "ET" },  // Amharic
        new string[] { "en", "US" },  // English
    };

    foreach (var lang in languages)
    {
        using (AndroidJavaObject locale = new AndroidJavaObject(
            "java.util.Locale", lang[0], lang[1]))
        {
            int available = tts.Call<int>("isLanguageAvailable", locale);
            Debug.Log($"Language {lang[0]} available: {available}");

            // -2 = not supported, -1 = missing data, 0 = available, 1 = country available, 2 = exact match
            if (available == -1) // missing data only — trigger download
            {
                Debug.Log($"Downloading voice for {lang[0]}...");
                TriggerVoiceDownload(lang[0], lang[1]);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
#endif
        yield return null;
    }

    private void TriggerVoiceDownload(string langCode, string countryCode)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        // ← This triggers silent background download of voice pack
        using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent"))
        {
            intent.Call<AndroidJavaObject>("setAction", 
                "com.android.settings.TTS_SETTINGS");
            
            // Try Google TTS engine first
            using (AndroidJavaObject googleIntent = new AndroidJavaObject("android.content.Intent",
                "android.speech.tts.engine.INSTALL_TTS_DATA"))
            {
                googleIntent.Call<AndroidJavaObject>("setPackage", 
                    "com.google.android.tts");
                
                try
                {
                    activity.Call("startActivity", googleIntent);
                    Debug.Log($"Google TTS install triggered for {langCode}");
                }
                catch
                {
                    Debug.LogWarning($"Could not trigger Google TTS install for {langCode}");
                }
            }
        }
    }
    catch (System.Exception e)
    {
        Debug.LogWarning($"Voice download failed for {langCode}: {e.Message}");
    }
#endif
    }

    //public bool IsEnabled() => isEnabled;

    public bool IsEnabled()
    {
        return isEnabled;
    }

    // ← Just call these directly from button onClick in Inspector
    //public void EnableTTS()
    //{
    //    isEnabled = true;
    //    Debug.Log("TTS Enabled ✅");
    //    StartCoroutine(SpeakAfterDelay());
    //}
    public void EnableTTS()
    {
        isEnabled = true;
        Debug.Log("TTS Enabled ✅");
        //StartCoroutine(SpeakAfterDelay()); // ← use coroutine with delay
    }

    // private IEnumerator SpeakAfterDelay()
    // {
    //     StopTTSSpeech(); // ← clear any leftover queue first
    //     yield return new WaitForSeconds(0.3f); // ← wait for queue to clear
    //     QuestionManager qm = FindObjectOfType<QuestionManager>();
    //     if (qm != null)
    //         qm.ReadCurrentQuestion();
    // }
    //public void EnableTTS()
    //{
    //    isEnabled = true;
    //    Debug.Log("TTS Enabled ✅");
    //    // ← Don't auto-speak in menu scene
    //    QuestionManager qm = FindObjectOfType<QuestionManager>();
    //    if (qm != null)
    //        qm.ReadCurrentQuestion();
    //}

    public void DisableTTS()
    {
        isEnabled = false;
        // ← Only stop TTS speech, NOT music
        StopTTSSpeech();
        Debug.Log("TTS Disabled 🔇");
    }

    // ← Separate method — only stops TTS voice, not music
    public void StopTTSSpeech()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    tts?.Call("stop");
#elif UNITY_IOS && !UNITY_EDITOR
    _IOSStop();
#endif
    }

    // ← Keep Stop() for coroutines to call
    public void Stop()
    {
        StopTTSSpeech();
    }

    //private IEnumerator SpeakAfterDelay()
    //{
    //    yield return new WaitForSeconds(0.3f);
    //    QuestionManager qm = FindObjectOfType<QuestionManager>();
    //    if (qm != null)
    //        qm.ReadCurrentQuestion();
    //}

    public void Speak(string text)
    {
        if (!isEnabled || !isReady || string.IsNullOrEmpty(text)) return;

#if UNITY_ANDROID && !UNITY_EDITOR
    SetTTSLanguage();

    using (AndroidJavaObject locale = new AndroidJavaObject(
        "java.util.Locale", GetLanguageCode(), GetCountryCode()))
    {
        int langAvailable = tts.Call<int>("isLanguageAvailable", locale);

        if (langAvailable < 0)
        {
            Debug.LogWarning($"TTS language not available: {GetLanguageCode()} — falling back to English");
            using (AndroidJavaObject englishLocale = new AndroidJavaObject("java.util.Locale", "en", "US"))
            {
                tts.Call<int>("setLanguage", englishLocale);
            }
        }
    }

    // ← Use QUEUE_ADD (1) instead of QUEUE_FLUSH (0) to prevent cutting
    tts?.Call<int>("speak", text, 1, null, System.Guid.NewGuid().ToString());

#elif UNITY_IOS && !UNITY_EDITOR
    _IOSSpeak(text, GetIOSLanguageCode());
#else
        Debug.Log($"[TTS Editor] {text}");
        SpeakInEditor(text);
#endif
    }

    //    public void Stop()
    //    {
    //#if UNITY_ANDROID && !UNITY_EDITOR
    //    tts?.Call("stop");
    //    // ← Also flush the queue so next speak starts clean
    //    tts?.Call<int>("speak", "", 0, null, "flush_" + System.Guid.NewGuid().ToString());
    //#elif UNITY_IOS && !UNITY_EDITOR
    //    _IOSStop();
    //#endif
    //    }
//    public void Stop()
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//    tts?.Call("stop"); // ← just this, nothing else
//#elif UNITY_IOS && !UNITY_EDITOR
//    _IOSStop();
//#endif
//    }

    private void SetTTSLanguage()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (tts == null || !isReady) return;
        string langCode = GetLanguageCode();
        string countryCode = GetCountryCode();
        using (AndroidJavaObject locale = new AndroidJavaObject("java.util.Locale", langCode, countryCode))
        {
            tts.Call<int>("setLanguage", locale);
        }
#endif
    }

    private string GetLanguageCode()
    {
        switch (LocalizationManager.currentLanguage)
        {
            case LocalizationManager.Language.Arabic: return "ar";
            case LocalizationManager.Language.Hebrew: return "iw";
            case LocalizationManager.Language.Russian: return "ru";
            case LocalizationManager.Language.Amharic: return "am";
            default: return "en";
        }
    }

    private string GetCountryCode()
    {
        switch (LocalizationManager.currentLanguage)
        {
            case LocalizationManager.Language.Arabic: return "SA";
            case LocalizationManager.Language.Hebrew: return "IL";
            case LocalizationManager.Language.Russian: return "RU";
            case LocalizationManager.Language.Amharic: return "ET";
            default: return "US";
        }
    }

    private string GetIOSLanguageCode()
    {
        switch (LocalizationManager.currentLanguage)
        {
            case LocalizationManager.Language.Arabic: return "ar-SA";
            case LocalizationManager.Language.Hebrew: return "he-IL";
            case LocalizationManager.Language.Russian: return "ru-RU";
            case LocalizationManager.Language.Amharic: return "am-ET";
            default: return "en-US";
        }
    }

    private void SpeakInEditor(string text)
    {
#if UNITY_EDITOR
        try
        {
            string escaped = text.Replace("\"", "").Replace("'", "");
            string psCommand = $"Add-Type -AssemblyName System.Speech; " +
                              $"$s = New-Object System.Speech.Synthesis.SpeechSynthesizer; " +
                              $"$s.Speak('{escaped}');";

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -Command \"{psCommand}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false
            };

            System.Diagnostics.Process.Start(psi);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[TTS Editor] Could not speak: {e.Message}");
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private class TTSInitListener : AndroidJavaProxy
    {
        private System.Action callback;
        public TTSInitListener(System.Action cb)
            : base("android.speech.tts.TextToSpeech$OnInitListener")
        {
            callback = cb;
        }
        public void onInit(int status)
        {
            if (status == 0) callback?.Invoke();
        }
    }
#endif
}

