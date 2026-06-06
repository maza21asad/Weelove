using System.Collections;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [Header("Light Mesh Renderers")]
    public MeshRenderer redLight;
    public MeshRenderer yellowLight;
    public MeshRenderer greenLight;

    [Header("Active Colors (HDR)")]
    public Color redOn = Color.red * 5f;
    public Color yellowOn = Color.yellow * 5f;
    public Color greenOn = Color.green * 5f;

    [Header("Inactive Color")]
    public Color offColor = new Color(0.1f, 0.1f, 0.1f);

    [Header("Timings")]
    public float greenTime = 5f;
    public float yellowTime = 2f;
    public float redTime = 5f;

    [Header("Zone Reference")]
    public SimulationZone carZone;
    public float delayBeforeGreen = 10f;

    [Header("State (Read Only)")]
    public bool isRed;
    public bool isYellow;
    public bool isGreen;

    Material redMat, yellowMat, greenMat;
    Coroutine delayedGreenRoutine;

    void Start()
    {
        redMat = redLight.material;
        yellowMat = yellowLight.material;
        greenMat = greenLight.material;
        SetRed();
        StartCoroutine(TrafficLightRoutine());
    }

    void Update()
    {
        if (carZone.isPlayerInside && delayedGreenRoutine == null)
            delayedGreenRoutine = StartCoroutine(TurnGreenAfterDelay());

        if (!carZone.isPlayerInside && delayedGreenRoutine != null)
        {
            StopCoroutine(delayedGreenRoutine);
            delayedGreenRoutine = null;
        }
    }

    IEnumerator TurnGreenAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGreen);
        if (!carZone.isPlayerInside) yield break;
        SetYellow();
        yield return new WaitForSeconds(1f);
        SetGreen();
    }

    IEnumerator TrafficLightRoutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => isGreen);
            yield return new WaitForSeconds(greenTime);
            SetYellow();
            yield return new WaitForSeconds(yellowTime);
            SetRed();
            yield return new WaitForSeconds(redTime);
        }
    }

    void SetRed() { isRed = true; isYellow = false; isGreen = false; Activate(redMat, redOn); Deactivate(yellowMat); Deactivate(greenMat); }
    void SetYellow() { isRed = false; isYellow = true; isGreen = false; Deactivate(redMat); Activate(yellowMat, yellowOn); Deactivate(greenMat); }
    void SetGreen() { isRed = false; isYellow = false; isGreen = true; Deactivate(redMat); Deactivate(yellowMat); Activate(greenMat, greenOn); }

    void Activate(Material mat, Color emission)
    {
        mat.color = Color.white;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emission);
    }

    void Deactivate(Material mat)
    {
        mat.color = offColor;
        mat.SetColor("_EmissionColor", Color.black);
    }

    public bool IsRedLight() => isRed;
    public bool IsYellowLight() => isYellow;
    public bool IsGreenLight() => isGreen;
}