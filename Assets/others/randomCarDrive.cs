using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Meshes (Optional for visuals)")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Car Settings")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;

    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private float currentBrakeForce;
    private bool isBraking;

    void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // A,D
        verticalInput = Input.GetAxis("Vertical");   // W,S
        isBraking = Input.GetKey(KeyCode.Space);     // Space = Brake
    }

    void HandleMotor()
    {
        // Apply motor torque to rear wheels
        rearLeftCollider.motorTorque = verticalInput * motorForce;
        rearRightCollider.motorTorque = verticalInput * motorForce;

        // Apply braking force
        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    void ApplyBraking()
    {
        frontLeftCollider.brakeTorque = currentBrakeForce;
        frontRightCollider.brakeTorque = currentBrakeForce;
        rearLeftCollider.brakeTorque = currentBrakeForce;
        rearRightCollider.brakeTorque = currentBrakeForce;
    }

    void HandleSteering()
    {
        steerAngle = maxSteerAngle * horizontalInput;
        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;
    }

    void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftCollider, frontLeftMesh);
        UpdateSingleWheel(frontRightCollider, frontRightMesh);
        UpdateSingleWheel(rearLeftCollider, rearLeftMesh);
        UpdateSingleWheel(rearRightCollider, rearRightMesh);
    }

    void UpdateSingleWheel(WheelCollider collider, Transform mesh)
    {
        if (mesh == null) return;

        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
