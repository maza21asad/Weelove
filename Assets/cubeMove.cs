using UnityEngine;

public class CubeMover : MonoBehaviour
{
    [Header("Path Points")]
    public Transform[] points;
    public float moveSpeed = 2f;

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    [Header("Animation")]
    public Animator animator;
    public string walkParameter = "isWalking";  // ← bool parameter name in Animator
    public string idleParameter = "isIdle";     // ← optional

    private bool isRedLight = false;
    private int currentPointIndex = 0;
    private int direction = 1;
    private Quaternion targetRotation;

    void Start()
    {
        if (points.Length > 0)
        {
            transform.position = points[0].position;
            targetRotation = transform.rotation;
        }

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ← Set animation state
        if (animator != null)
            animator.SetBool(walkParameter, isRedLight);

        if (!isRedLight) return;
        if (points == null || points.Length == 0) return;

        // ← Smooth rotation
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );

        Vector3 target = points[currentPointIndex].position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            currentPointIndex += direction;

            if (currentPointIndex >= points.Length)
            {
                currentPointIndex = points.Length - 2;
                direction = -1;
                targetRotation *= Quaternion.Euler(0f, 180f, 0f);
            }
            else if (currentPointIndex < 0)
            {
                currentPointIndex = 1;
                direction = 1;
                targetRotation *= Quaternion.Euler(0f, 180f, 0f);
            }
        }
    }

    public void SetRedLight(bool isRed)
    {
        isRedLight = isRed;
    }

    void OnDrawGizmos()
    {
        if (points == null || points.Length < 2) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < points.Length - 1; i++)
        {
            if (points[i] != null && points[i + 1] != null)
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }

        Gizmos.color = Color.yellow;
        foreach (var p in points)
        {
            if (p != null)
                Gizmos.DrawSphere(p.position, 0.2f);
        }
    }
}