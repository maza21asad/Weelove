using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GasButtonBlink : MonoBehaviour
{
    [Header("References")]
    public Image targetImage;            // real gas button image
    public Sprite greenSprite;           // normal
    public Sprite redSprite;             // stop
    public List<TriggerZone> triggerZones; // 🌟 multiple trigger zones

    [Header("Blink Settings")]
    public float blinkSpeed = 0.5f;
    public float dimAlpha = 0.3f;
    public float brightAlpha = 1f;
    public float blinkDuration = 5f;

    private Coroutine blinkRoutine;

    void Update()
    {
        if (triggerZones == null || triggerZones.Count == 0 || targetImage == null)
            return;

        // check if car is inside ANY of the trigger zones
        bool isInsideAnyZone = false;
        foreach (var zone in triggerZones)
        {
            if (zone != null && zone.isTriggered)
            {
                isInsideAnyZone = true;
                break;
            }
        }

        if (isInsideAnyZone)
        {
            // set red sprite
            targetImage.sprite = redSprite;

            // start blinking once
            if (blinkRoutine == null)
            {
                blinkRoutine = StartCoroutine(BlinkRed());
            }
        }
        else
        {
            // 🚗 outside all zones
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
                blinkRoutine = null;
            }

            // go back to green solid
            targetImage.sprite = greenSprite;
            targetImage.color = Color.white;
        }
    }

    IEnumerator BlinkRed()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            // bright red
            targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
            yield return new WaitForSeconds(blinkSpeed);

            // dim red
            targetImage.color = new Color(1f, 1f, 1f, dimAlpha);
            yield return new WaitForSeconds(blinkSpeed);

            timer += blinkSpeed * 2f;
        }

        // after blinking → solid red
        blinkRoutine = null;
        targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
    }
}
