using UnityEngine;

public class SimulationZone : MonoBehaviour
{
    public bool isPlayerInside = false;
    public bool justExited = false;

    private void Awake()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null) box.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInside = true;
        justExited = false;
        Debug.Log("SimulationZone: Player Entered");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInside = false;
        justExited = true;
        Debug.Log("SimulationZone: Player Exited");
    }

    // Call this after reading justExited in the same frame
    public void ClearExitFlag() => justExited = false;
}