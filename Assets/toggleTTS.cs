//using UnityEngine;
//using UnityEngine.UI;

//public class TTSToggleButton : MonoBehaviour
//{
//    [Header("Sprites")]
//    public Sprite onSprite;   // ← assign TTS ON sprite
//    public Sprite offSprite;  // ← assign TTS OFF sprite

//    [Header("Image")]
//    public Image targetImage; // ← assign the button Image component

//    private bool isTTSOn = true;

//    void Start()
//    {
//        // Load saved state
//        isTTSOn = PlayerPrefs.GetInt("TTSEnabled", 1) == 1;

//        // Just set state, don't speak yet
//        if (AndroidTTS.instance != null)
//            AndroidTTS.instance.SetEnabled(isTTSOn);

//        UpdateSprite();
//    }

//    public void OnButtonPressed()
//    {
//        isTTSOn = !isTTSOn;

//        PlayerPrefs.SetInt("TTSEnabled", isTTSOn ? 1 : 0);
//        PlayerPrefs.Save();

//        if (AndroidTTS.instance != null)
//        {
//            if (isTTSOn)
//                AndroidTTS.instance.EnableTTS();
//            else
//                AndroidTTS.instance.DisableTTS();
//        }

//        UpdateSprite();
//    }

//    void UpdateSprite()
//    {
//        if (targetImage == null) return;
//        if (onSprite == null || offSprite == null) return;

//        targetImage.sprite = isTTSOn ? onSprite : offSprite;
//        targetImage.enabled = false;
//        targetImage.enabled = true;
//    }
//}

using UnityEngine;
using UnityEngine.UI;

public class TTSToggleButton : MonoBehaviour
{
    private bool isTTSOn = true;
    private ImageSwapper imageSwapper;

    void Start()
    {
        imageSwapper = GetComponent<ImageSwapper>();

        // Load saved state
        isTTSOn = PlayerPrefs.GetInt("TTSEnabled", 1) == 1;

        // Sync ImageSwapper to match saved state
        if (imageSwapper != null)
            imageSwapper.SetState(isTTSOn);

        // Just set TTS state, don't speak yet
        if (AndroidTTS.instance != null)
            AndroidTTS.instance.SetEnabled(isTTSOn);
    }

    public void OnButtonPressed()
    {
        isTTSOn = !isTTSOn;

        PlayerPrefs.SetInt("TTSEnabled", isTTSOn ? 1 : 0);
        PlayerPrefs.Save();

        if (AndroidTTS.instance != null)
        {
            if (isTTSOn)
                AndroidTTS.instance.EnableTTS();
            else
                AndroidTTS.instance.DisableTTS();
        }
    }
}