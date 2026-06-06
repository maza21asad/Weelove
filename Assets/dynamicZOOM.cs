using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraSwapper : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera thirdPersonCam;
    public CinemachineVirtualCamera firstPersonCam;

    [Header("References")]
    public CinemachineSmoothPath path;
    public Transform car;
    public Car carController;

    [Header("Default Offset (Straight Road)")]
    public Vector3 straightOffset = new Vector3(0f, 3f, -6f);

    [Header("Turn Camera Settings")]
    public float turnSideShift = 1.5f;
    public float turnPullBack = 1.5f;
    public float turnRaiseHeight = 0.8f;

    [Header("Turn Detection")]
    public float checkDistance = 2f;
    public float sharpTurnThreshold = 25f; // ← only very sharp turns trigger shift
    public float turnConfirmTime = 0.8f;   // ← must be in turn for this long before camera shifts

    [Header("Speed Effects")]
    public float maxSpeedForEffect = 20f;
    public float speedPullBack = 4f;
    public float speedLowering = 0.5f;

    [Header("Smoothing")]
    public float offsetSmoothSpeed = 0.15f;  // ← Y and Z smooth speed
    public float sideShiftSmoothSpeed = 0.08f; // ← X side shift very slow

    [Header("Manual Override")]
    private bool isFirstPerson = false;

    [Header("Intro Zoom")]
    public float introDuration = 2.5f;
    public Vector3 introStartOffset = new Vector3(0f, 15f, -25f);
    public bool playIntroOnStart = true;

    private CinemachineTransposer thirdPersonTransposer;
    private bool introComplete = false;

    // ← Turn confirmation tracking
    private float turnTimer = 0f;
    private float confirmedTurnDirection = 0f; // ← only updates after turnConfirmTime
    private float smoothedSideShift = 0f;      // ← current smooth side shift value




    void Start()
    {
        thirdPersonTransposer = thirdPersonCam.GetCinemachineComponent<CinemachineTransposer>();

        if (thirdPersonTransposer != null)
            thirdPersonTransposer.m_FollowOffset = straightOffset;

        SetThirdPerson();

        if (playIntroOnStart)
            StartCoroutine(PlayIntroZoom());
        else
            introComplete = true;
    }

    void Update()
    {
        if (!introComplete) return;
        if (path == null || car == null) return;
        if (isFirstPerson || thirdPersonTransposer == null) return;

        float speed = carController != null ? carController.CurrentSpeed : 0f;
        float speedFactor = Mathf.Clamp01(speed / maxSpeedForEffect);

        // ← Detect turn at car position
        float nearest = path.FindClosestPoint(car.position, 0, -1, 10);
        Vector3 forward1 = path.EvaluateTangent(nearest);
        Vector3 forward2 = path.EvaluateTangent(nearest + checkDistance);

        float angle = Vector3.Angle(forward1, forward2);
        Vector3 cross = Vector3.Cross(forward1.normalized, forward2.normalized);
        float rawTurnDirection = cross.y;

        // ← Only count as turn if angle is above sharp threshold
        bool isSharpTurn = angle > sharpTurnThreshold;

        if (isSharpTurn)
        {
            // ← Accumulate time in turn
            turnTimer += Time.deltaTime;

            // ← Only confirm turn direction after car has been turning for turnConfirmTime
            if (turnTimer >= turnConfirmTime)
                confirmedTurnDirection = rawTurnDirection;
        }
        else
        {
            // ← Reset — straight road
            turnTimer = 0f;
            confirmedTurnDirection = 0f;
        }

        // ← Calculate target side shift based on CONFIRMED turn only
        float targetSideShift = -confirmedTurnDirection * turnSideShift;

        // ← Smooth side shift very slowly
        smoothedSideShift = Mathf.Lerp(
            smoothedSideShift,
            targetSideShift,
            Time.deltaTime * sideShiftSmoothSpeed * 10f
        );

        // ← Base offset
        Vector3 targetOffset = straightOffset;
        targetOffset.z -= speedPullBack * speedFactor;
        targetOffset.y -= speedLowering * speedFactor;

        // ← Apply confirmed side shift + raise/pullback only when confirmed
        float turnIntensity = Mathf.Clamp01(turnTimer - turnConfirmTime);
        targetOffset.x = smoothedSideShift;
        targetOffset.y += turnRaiseHeight * Mathf.Clamp01(confirmedTurnDirection != 0 ? 1f : 0f);
        targetOffset.z -= turnPullBack * Mathf.Clamp01(confirmedTurnDirection != 0 ? 1f : 0f);

        // ← Apply smooth — X very slow, Y and Z normal
        float newX = Mathf.Lerp(
            thirdPersonTransposer.m_FollowOffset.x,
            targetOffset.x,
            Time.deltaTime * sideShiftSmoothSpeed * 10f
        );

        float newY = Mathf.Lerp(
            thirdPersonTransposer.m_FollowOffset.y,
            targetOffset.y,
            Time.deltaTime * (1f / offsetSmoothSpeed)
        );

        float newZ = Mathf.Lerp(
            thirdPersonTransposer.m_FollowOffset.z,
            targetOffset.z,
            Time.deltaTime * (1f / offsetSmoothSpeed)
        );

        thirdPersonTransposer.m_FollowOffset = new Vector3(newX, newY, newZ);
    }

    public void ToggleCamera()
    {
        if (isFirstPerson) SetThirdPerson();
        else SetFirstPerson();
    }

    void SetFirstPerson()
    {
        isFirstPerson = true;
        firstPersonCam.Priority = 20;
        thirdPersonCam.Priority = 10;
    }

    void SetThirdPerson()
    {
        isFirstPerson = false;
        thirdPersonCam.Priority = 20;
        firstPersonCam.Priority = 10;
    }

    IEnumerator PlayIntroZoom()
    {
        introComplete = false;

        if (thirdPersonTransposer != null)
            thirdPersonTransposer.m_FollowOffset = introStartOffset;

        float elapsed = 0f;

        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, elapsed / introDuration);

            if (thirdPersonTransposer != null)
                thirdPersonTransposer.m_FollowOffset = Vector3.Lerp(
                    introStartOffset,
                    straightOffset,
                    smoothT
                );

            yield return null;
        }

        if (thirdPersonTransposer != null)
            thirdPersonTransposer.m_FollowOffset = straightOffset;

        introComplete = true;
    }
}