using UnityEngine;

public class TrafficRuleChecker : MonoBehaviour
{
    public TrafficLightController trafficLight;
    public SimulationZone carZone;
    public Car car;

    private bool enteredOnRed = false;
    private bool greenReached = false;

    void Update()
    {
        // Car enters zone during RED
        if (carZone.isPlayerInside && trafficLight.isRed && !enteredOnRed)
        {
            enteredOnRed = true;
            greenReached = false;
            Debug.Log("Car entered on RED");
        }

        // Light turns GREEN while car is waiting inside
        if (enteredOnRed && carZone.isPlayerInside && trafficLight.isGreen)
        {
            greenReached = true;
            Debug.Log("Green reached while waiting ✅");
        }

        // Car exits zone — evaluate ONCE using justExited
        if (carZone.justExited && enteredOnRed)
        {
            carZone.ClearExitFlag(); // consume the flag immediately

            if (greenReached && trafficLight.isGreen)
            {
                Debug.Log("Valid crossing ✅");
            }
            else
            {
                Debug.Log("Invalid crossing ❌ — punishing");
                car.MoveBackByWaypoints(3);
            }

            enteredOnRed = false;
            greenReached = false;



        }
    }
}