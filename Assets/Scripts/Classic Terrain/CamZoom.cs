using UnityEngine;
using Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Vector3 zoomOutOffset = new Vector3(0, 4, -16); // default zoom-out
    public Vector3 zoomInOffset = new Vector3(0, 3, -10);   // closer zoom
    public float zoomSpeed = 3f;

    private Cinemachine3rdPersonFollow follow;
    private Car carController;
    private bool hasStartedDriving = false;

    void Start()
    {
        follow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        carController = FindObjectOfType<Car>(); // find your car
        follow.ShoulderOffset = zoomOutOffset; // start zoomed out
    }

    void Update()
    {
        if (carController == null) return;

        // If gas is pressed, mark driving as started
        if (carControllerIsPressingGas())
            hasStartedDriving = true;

        // Only start zoom logic after driving has started
        if (hasStartedDriving)
        {
            Vector3 targetOffset = carControllerIsPressingGas() ? zoomInOffset : zoomOutOffset;
            follow.ShoulderOffset = Vector3.Lerp(
                follow.ShoulderOffset,
                targetOffset,
                Time.unscaledDeltaTime * zoomSpeed
            );
        }
    }

    private bool carControllerIsPressingGas()
    {
        // Directly access your AICarController's gas state
        var field = carController.GetType().GetField("isGasPressed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (bool)field.GetValue(carController);
    }



}



