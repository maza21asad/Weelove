using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarGas : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Movement Settings")]
    public float maxMotorTorque = 1500f;
    public float acceleration = 800f;
    public float deceleration = 1200f;

    [Header("Speed Limit")]
    public float maxSpeed = 20f; // meters per second

    private Rigidbody rb;
    private float currentTorque = 0f;
    private bool isGasPressed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        // OPTIONAL: Keyboard support (PC testing)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            GasDown();

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space))
            GasUp();
    }

    void FixedUpdate()
    {
        HandleMotor();
        LimitSpeed();
    }

    void HandleMotor()
    {
        // Smooth acceleration / deceleration
        if (isGasPressed)
        {
            currentTorque = Mathf.MoveTowards(
                currentTorque,
                maxMotorTorque,
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            currentTorque = Mathf.MoveTowards(
                currentTorque,
                0f,
                deceleration * Time.fixedDeltaTime
            );
        }

        ApplyMotorTorque(currentTorque);
    }

    void ApplyMotorTorque(float torque)
    {
        frontLeft.motorTorque = torque;
        frontRight.motorTorque = torque;
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    void LimitSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    // =============================
    // UI BUTTON EVENTS (IMPORTANT)
    // =============================

    public void GasDown()
    {
        isGasPressed = true;
    }

    public void GasUp()
    {
        isGasPressed = false;
    }
}
