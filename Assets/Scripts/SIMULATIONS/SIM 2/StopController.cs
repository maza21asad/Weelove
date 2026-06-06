using UnityEngine;

public class StopSimulationController : MonoBehaviour
{
    [Header("References")]
    public Car car;
    public SimulationZone stopArea;        // ← changed from TriggerZone
    public GasButtonBlink gasButtonBlink;

    [Header("Stop Settings")]
    public float stopThreshold = 0.05f;

    private bool hasStoppedInside = false;
    private bool resultEvaluated = false;

    void Update()
    {
        if (stopArea == null || car == null) return;

        // Car is inside zone
        if (stopArea.isPlayerInside)
        {
            resultEvaluated = false; // reset so we can evaluate on exit

            if (car.CurrentSpeed <= stopThreshold)
            {
                hasStoppedInside = true;
                Debug.Log("Car stopped inside zone ✅");
            }
        }

        // Car just exited — evaluate ONCE
        if (stopArea.justExited && !resultEvaluated)
        {
            stopArea.ClearExitFlag();
            resultEvaluated = true;

            if (hasStoppedInside)
            {
                Debug.Log("Successfully stopped at the stop sign 👍");
            }
            else
            {
                Debug.Log("Rule broken — car did NOT stop ❌");
                car.MoveBackByWaypoints(3);
            }

            // Reset for next attempt
            hasStoppedInside = false;
            resultEvaluated = false;
        }
    }
}