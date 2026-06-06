using UnityEngine;

public class SlipperyRoadManager : MonoBehaviour
{
    [Header("References")]
    public TriggerZoneSpriteHandler slipperyZone;
    public Car car;
    public Renderer roadRenderer; // Drag your 3D road object here

    [Header("Materials")]
    public Material failMaterial;    // Default / failure material
    public Material successMaterial; // Material to apply on success

    [Header("Rules")]
    public float maxAllowedSpeed = 10f;
    public int penaltyWaypoints = 3;

    private bool carInside = false;
    private bool speedWasValid = true;

    void Start()
    {
        // Make sure road starts with the fail/default material
        if (roadRenderer != null && failMaterial != null)
            roadRenderer.material = failMaterial;
    }

    void Update()
    {
        if (slipperyZone == null || car == null)
            return;

        bool isTriggered = slipperyZone.GetComponent<Collider>().bounds
            .Contains(car.transform.position);

        if (isTriggered && !carInside)
            OnCarEntered();
        else if (!isTriggered && carInside)
            OnCarExited();

        if (carInside)
        {
            if (car.CurrentSpeed > maxAllowedSpeed)
                speedWasValid = false;
        }
    }

    void OnCarEntered()
    {
        carInside = true;
        speedWasValid = true;
    }

    void OnCarExited()
    {
        carInside = false;

        if (speedWasValid)
        {
            Debug.Log("✅ Slippery Road Passed Successfully");

            // ✅ Swap to success material
            if (roadRenderer != null && successMaterial != null)
                roadRenderer.material = successMaterial;
        }
        else
        {
            Debug.Log("❌ Slippery Road Failed — Speed too high");
            car.PauseCar();
            car.MoveBackByWaypoints(penaltyWaypoints);
            car.ResumeDriving();
        }
    }
}