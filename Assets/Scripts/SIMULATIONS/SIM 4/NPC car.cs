using UnityEngine;

public class NPCCarMover : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float moveSpeed = 5f;

    private bool hasMoved = false;
    private bool isMoving = false;

    private void Start()
    {
        transform.position = startPoint.position;
    }

    private void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, endPoint.position) < 0.1f)
        {
            isMoving = false;
            hasMoved = true;
            // ✅ Notify manager that this NPC finished crossing
            CrossRoadManager.Instance.OnSimulationComplete();
        }
    }

    public void StartCrossing()
    {
        isMoving = true;
        hasMoved = false;
    }

    public void ResetToStart()
    {
        isMoving = false;
        hasMoved = false;
        transform.position = startPoint.position;
        Debug.Log($"{gameObject.name} reset to start ✅");
    }

    public bool HasFinished() => hasMoved;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            CrossRoadManager.Instance.Fail();
    }
}