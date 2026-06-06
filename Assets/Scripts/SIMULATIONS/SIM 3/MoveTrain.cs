using UnityEngine;

public class MoveOnX : MonoBehaviour
{
    public float moveSpeed = 2f;
    public bool isMoving = false;

    void Update()
    {
        if (!isMoving) return;
        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }
}