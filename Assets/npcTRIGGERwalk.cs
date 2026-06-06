using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaTriggeredNPCWalker : MonoBehaviour
{
    [Header("NPC Settings")]
    public Transform npc;                 // NPC transform
    public Animator npcAnimator;           // NPC animator
    public float walkSpeed = 1.5f;
    public float walkDuration = 14f;

    [Header("Movement Direction")]
    public Vector3 moveDirection = Vector3.forward;

    private bool isWalking = false;
    private float lockedY;

    void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        if (npc != null)
            lockedY = npc.position.y;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isWalking) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(WalkRoutine());
        }
    }

    IEnumerator WalkRoutine()
    {
        isWalking = true;

        if (npcAnimator != null)
            npcAnimator.SetBool("IsWalking", true);

        float timer = 0f;

        while (timer < walkDuration)
        {
            // Move NPC
            npc.position += moveDirection.normalized * walkSpeed * Time.deltaTime;

            // Lock Y position
            Vector3 pos = npc.position;
            pos.y = lockedY;
            npc.position = pos;

            timer += Time.deltaTime;
            yield return null;
        }

        // Stop walking
        if (npcAnimator != null)
            npcAnimator.SetBool("IsWalking", false);
    }
}
