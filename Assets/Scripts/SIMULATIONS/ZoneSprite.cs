using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZoneSpriteHandler : MonoBehaviour
{
    [Header("UI References")]
    public Image targetImage;
    public Sprite greenSprite;
    public Sprite middleSprite;
    public Sprite redSprite;

    [Header("Transition Settings")]
    public float middleSpriteDuration = 2f;

    [Header("Blink Settings (Optional)")]
    public bool enableBlink = true;
    public float blinkSpeed = 0.5f;
    public float dimAlpha = 0.3f;
    public float brightAlpha = 1f;

    //[Header("Scale Settings")]
    //public float normalScale = 1f;
    //public float enlargedScale = 1.3f;    // ← size when orange/red
    //public float scaleDuration = 0.3f;    // ← transition speed

    [Header("Scale Settings")]
    public Vector3 normalScale = new Vector3(0.987530112f, 2.13905478f, 0f);
    public Vector3 enlargedScale = new Vector3(1.2f, 2.6f, 0f);   // ← tweak multiplier as needed
    public float scaleDuration = 0.3f;


    private Coroutine blinkRoutine;
    private Coroutine transitionRoutine;

    private void Awake()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        if (targetImage != null)
        {
            // In Awake()
            targetImage.transform.localScale = normalScale;

            // In OnTriggerExit() reset
            targetImage.transform.DOKill();
            targetImage.transform.DOScale(normalScale, scaleDuration)
                .SetEase(Ease.OutBack);

            // In GreenToRedSequence() Step 1
            targetImage.transform.DOKill();
            targetImage.transform.DOScale(enlargedScale, scaleDuration)
                .SetEase(Ease.OutBack);

            // In GreenToRedSequence() Step 2 punch
            targetImage.transform.DOKill();
            targetImage.transform.DOPunchScale(
                new Vector3(0.15f, 0.15f, 0f),
                0.3f,
                5,
                0.5f
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(GreenToRedSequence());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Stop all effects
        if (transitionRoutine != null)
        {
            StopCoroutine(transitionRoutine);
            transitionRoutine = null;
        }

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        // Reset to green + shrink back to normal
        if (targetImage != null)
        {
            targetImage.color = Color.white;
            targetImage.sprite = greenSprite;

            targetImage.transform.DOKill();
            targetImage.transform.DOScale(normalScale, scaleDuration)
                .SetEase(Ease.OutBack);
        }
    }

    private IEnumerator GreenToRedSequence()
    {
        // 🟡 Step 1: show middle sprite + enlarge
        if (targetImage != null && middleSprite != null)
        {
            targetImage.color = Color.white;
            targetImage.sprite = middleSprite;

            targetImage.transform.DOKill();
            targetImage.transform.DOScale(enlargedScale, scaleDuration)
                .SetEase(Ease.OutBack);
        }

        yield return new WaitForSeconds(middleSpriteDuration);

        // 🔴 Step 2: switch to red + keep enlarged
        if (targetImage != null)
        {
            targetImage.sprite = redSprite;

            // Pulse effect on red
            targetImage.transform.DOKill();
            targetImage.transform.DOPunchScale(
                Vector3.one * 0.15f,
                0.3f,
                5,
                0.5f
            );

            if (enableBlink)
                blinkRoutine = StartCoroutine(BlinkRed());
        }
    }

    private IEnumerator BlinkRed()
    {
        while (true)
        {
            targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
            yield return new WaitForSeconds(blinkSpeed);
            targetImage.color = new Color(1f, 1f, 1f, dimAlpha);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}