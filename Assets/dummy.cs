//using UnityEngine;
//using UnityEngine.UI;

//public class ImageSwapper : MonoBehaviour
//{
//    public Image targetImage;
//    public Sprite imageA;
//    public Sprite imageB;

//    private bool isA = true;

//    void Start()
//    {
//        if (targetImage == null)
//            targetImage = GetComponent<Image>();

//        targetImage.sprite = imageA;
//    }

//    public void SwapImage()
//    {
//        isA = !isA;
//        targetImage.sprite = isA ? imageA : imageB;
//    }
//}

using UnityEngine;
using UnityEngine.UI;

public class ImageSwapper : MonoBehaviour
{
    public Image targetImage;
    public Sprite imageA;  // ← TTS ON sprite
    public Sprite imageB;  // ← TTS OFF sprite

    private bool isA = true;

    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        targetImage.sprite = imageA;
    }

    public void SwapImage()
    {
        isA = !isA;
        targetImage.sprite = isA ? imageA : imageB;
    }

    // ← NEW: sync state from outside
    public void SetState(bool state)
    {
        isA = state;
        if (targetImage != null)
            targetImage.sprite = isA ? imageA : imageB;
    }
}