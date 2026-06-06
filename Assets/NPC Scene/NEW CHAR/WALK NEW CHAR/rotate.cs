using UnityEngine;
using System.Collections;

public class NPCTurnAround : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float turnInterval = 10f;   // Time before turning

    private float lockedY;             // Stored Y position

    void Start()
    {
        // Store initial Y position
        lockedY = transform.position.y;

        StartCoroutine(TurnRoutine());
    }

    void LateUpdate()
    {
        // Lock Y position every frame
        Vector3 pos = transform.position;
        pos.y = lockedY;
        transform.position = pos;
    }

    IEnumerator TurnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(turnInterval);

            // Rotate NPC 180 degrees on Y axis
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
