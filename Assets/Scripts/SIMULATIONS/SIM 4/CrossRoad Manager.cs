using System.Collections;
using UnityEngine;

public class CrossRoadManager : MonoBehaviour
{
    public static CrossRoadManager Instance;

    [Header("References")]
    public SimulationZone playerAreaZone;
    public NPCCarMover npcCar1;
    public NPCCarMover npcCar2;

    [Header("Reset Delay")]
    public float resetDelay = 2f;

    private bool carsAlreadyStarted = false;
    private bool isResetting = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!carsAlreadyStarted && !isResetting && playerAreaZone.isPlayerInside)
        {
            StartNPCMovement();
        }
    }

    void StartNPCMovement()
    {
        carsAlreadyStarted = true;
        if (npcCar1 != null) npcCar1.StartCrossing();
        if (npcCar2 != null) npcCar2.StartCrossing();
        Debug.Log("NPC cars are crossing ✅");
    }

    public void Fail()
    {
        Debug.Log("FAILED: Player hit NPC car ❌");
        StartCoroutine(ResetAfterDelay());
    }

    // Call this from outside when player successfully passes too
    public void OnSimulationComplete()
    {
        StartCoroutine(ResetAfterDelay());
    }

    IEnumerator ResetAfterDelay()
    {
        if (isResetting) yield break;
        isResetting = true;

        yield return new WaitForSeconds(resetDelay);

        if (npcCar1 != null) npcCar1.ResetToStart();
        if (npcCar2 != null) npcCar2.ResetToStart();

        carsAlreadyStarted = false;
        isResetting = false;

        Debug.Log("Crossroad simulation reset 🔄");
    }
}