using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("References")]
    public TrafficLightController trafficLight;
    public TriggerZone carTriggerZone;

    // 🌟 The REAL gas button blink script from UI
    public GasButtonBlink gasButtonBlink;

    void Start()
    {
        // bind blink logic to this simulation's trigger zone
        // Example for SimulationManager
        if (gasButtonBlink != null)
        {
            gasButtonBlink.triggerZones = new List<TriggerZone> { carTriggerZone }; // single zone
        }

    }

    void Update()
    {
        // no UI logic here anymore — gas button handles itself
        HandleTrafficLogic();
    }

    // 🚦 traffic logic stays same
    void HandleTrafficLogic()
    {
        // Example: If you want pedestrians or other logic later
        // keep it here — UI blinking handled elsewhere
    }
}
