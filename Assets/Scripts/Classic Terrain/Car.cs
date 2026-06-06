using UnityEngine;
using Cinemachine;

public class Car : MonoBehaviour
{
    [Header("Movement Settings")]
    public CinemachinePathBase roadPath;

    [Tooltip("Max forward speed in m/s along the path")]
    public float maxSpeed = 15f;
    public float acceleration = 5f;
    public float deceleration = 5f;

    private float pathPosition = 0f;

    [SerializeField]
    private float currentSpeed = 0f;

    private Rigidbody rb;
    private bool isGasPressed = false;
    public static bool isCarMoving = false;

    public float CurrentSpeed => currentSpeed;

    [Header("Start Position")]
    public int startWaypointIndex = 0;

    private CinemachineSmoothPath smoothPath;

    [Header("Level 5 Auto Drive")]
    public bool autoDriveLevel5 = false;
    public float autoDriveSpeed = 20f;

    private bool isForcedStopped = false;



    [Header("Wheels")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    [Header("Wheel Settings")]
    public float wheelRadius = 0.3f; // ← set to match your wheel size

    private float wheelRotationX = 0f;

    [Header("Wheel Steering")]
    public float maxSteerAngle = 30f;
    public float steerSmoothSpeed = 5f;

    private float currentSteerAngle = 0f;
    public float turnangle = 30f;

    // =========================
    // START
    // =========================

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        smoothPath = roadPath as CinemachineSmoothPath;

        if (smoothPath == null)
        {
            Debug.LogError("Road path must be CinemachineSmoothPath.");
            return;
        }

        startWaypointIndex = Mathf.Clamp(
            startWaypointIndex,
            0,
            smoothPath.m_Waypoints.Length - 1
        );

        SpawnAtWaypoint(startWaypointIndex);
    }

    // =========================
    // OPTIMIZED UPDATE
    // =========================

    private void LateUpdate()
    {
        float targetSpeed = 0f;

        // LEVEL 5 AUTO DRIVE
        if (autoDriveLevel5 && !isForcedStopped)
        {
            targetSpeed = autoDriveSpeed;
        }
        else if (!autoDriveLevel5)
        {
            // NORMAL CONTROL
            targetSpeed = isGasPressed ? maxSpeed : 0f;
        }

        // MOVE SPEED TOWARDS TARGET
        float rate = (targetSpeed > currentSpeed) ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            rate * Time.deltaTime
        );

        // MOVING STATE
        isCarMoving = currentSpeed > 0.01f;

        // PATH MOVEMENT
        pathPosition += currentSpeed * Time.deltaTime;
        pathPosition = Mathf.Clamp(pathPosition, 0f, roadPath.PathLength);

        SetCarToPathPosition();
        RotateWheels(); // ← add this
    }

    // =========================
    // CONTROLS
    // =========================

    public void GasPressed() => isGasPressed = true;
    public void GasReleased() => isGasPressed = false;

    public float GetPathPosition() => pathPosition;
    public bool IsGasPressed() => isGasPressed;

    public void ResumeDriving()
    {
        isForcedStopped = false;
        isCarMoving = true;
    }

    public void PauseCar()
    {
        isForcedStopped = true;
        isGasPressed = false;
        currentSpeed = 0f;
        isCarMoving = false;
    }

    public void RespawnAtStart()
    {
        pathPosition = 0f;
        currentSpeed *= 0.2f;
        isGasPressed = false;
        isCarMoving = false;
        SetCarToPathPosition();
    }

    // =========================
    // PUNISHMENTS
    // =========================

    public void MoveBackByWaypoints(int count)
    {
        if (smoothPath == null)
            smoothPath = roadPath as CinemachineSmoothPath;

        int total = smoothPath.m_Waypoints.Length;

        int currentIndex = Mathf.RoundToInt(
            (pathPosition / roadPath.PathLength) * (total - 1)
        );

        currentIndex -= count;
        currentIndex = Mathf.Clamp(currentIndex, 0, total - 1);

        float t = (float)currentIndex / (total - 1);
        pathPosition = t * roadPath.PathLength;

        currentSpeed = 0f;
        SetCarToPathPosition();
    }

    public void MoveBackTenWaypoints()
    {
        MoveBackByWaypoints(10);
    }

    // =========================
    // INTERNAL
    // =========================

    private void SetCarToPathPosition()
    {
        transform.position = roadPath.EvaluatePositionAtUnit(
            pathPosition,
            CinemachinePathBase.PositionUnits.Distance
        );

        transform.rotation = roadPath.EvaluateOrientationAtUnit(
            pathPosition,
            CinemachinePathBase.PositionUnits.Distance
        );
    }

    private void SpawnAtWaypoint(int index)
    {
        int total = smoothPath.m_Waypoints.Length;

        index = Mathf.Clamp(index, 0, total - 1);

        float t = (float)index / (total - 1);
        pathPosition = t * roadPath.PathLength;

        SetCarToPathPosition();
    }

    private void RotateWheels()
    {
        if (currentSpeed < 0.01f)
        {
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, 0f, Time.deltaTime * steerSmoothSpeed);
            ApplyWheelRotations();
            return;
        }

        // ← Speed based roll
        float degreesPerSecond = (currentSpeed / (2f * Mathf.PI * wheelRadius)) * 360f;
        wheelRotationX += degreesPerSecond * Time.deltaTime;

        // ← Detect turn from path
        float prevPos = Mathf.Max(pathPosition - 0.5f, 0f);
        Vector3 forwardPrev = roadPath.EvaluateTangentAtUnit(prevPos, CinemachinePathBase.PositionUnits.Distance);
        Vector3 forwardNow = roadPath.EvaluateTangentAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Distance);
        float turnAngle = Vector3.SignedAngle(forwardPrev, forwardNow, Vector3.up);

        // ← Smooth steer angle
        float targetSteer = Mathf.Clamp(turnAngle * turnangle, -maxSteerAngle, maxSteerAngle);
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteer, Time.deltaTime * steerSmoothSpeed);

        ApplyWheelRotations();
    }

    private void ApplyWheelRotations()
    {
        // ← Right wheels — normal
        Quaternion rightRoll = Quaternion.Euler(wheelRotationX, 0f, 0f);
        // ← Left wheels — 180 Y offset fix
        Quaternion leftRoll = Quaternion.Euler(wheelRotationX, 180f, 0f);

        // ← Front wheels add steering on Y
        Quaternion frontRightSteer = Quaternion.Euler(wheelRotationX, currentSteerAngle, 0f);
        Quaternion frontLeftSteer = Quaternion.Euler(wheelRotationX, 180f + currentSteerAngle, 0f);

        if (frontLeftWheel != null) frontLeftWheel.localRotation = frontLeftSteer;
        if (frontRightWheel != null) frontRightWheel.localRotation = frontRightSteer;
        if (rearLeftWheel != null) rearLeftWheel.localRotation = leftRoll;
        if (rearRightWheel != null) rearRightWheel.localRotation = rightRoll;
    }

}
