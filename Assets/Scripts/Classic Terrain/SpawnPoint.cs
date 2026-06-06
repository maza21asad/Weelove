using UnityEngine;
using Cinemachine;

public class SpawnPoint : MonoBehaviour
{
    public CinemachineSmoothPath path;

    // Which waypoint number to spawn at
    public int waypointIndex = 0;

    void Start()
    {
        MoveCarToWaypoint();
    }

    public void MoveCarToWaypoint()
    {
        if (path == null || path.m_Waypoints == null || path.m_Waypoints.Length == 0)
            return;

        // Clamp index to valid waypoint count
        waypointIndex = Mathf.Clamp(waypointIndex, 0, path.m_Waypoints.Length - 1);

        // Get waypoint position **in world space**
        Vector3 worldPos = path.transform.TransformPoint(path.m_Waypoints[waypointIndex].position);
        transform.position = worldPos;

        // Face forward along path
        Quaternion rot = path.EvaluateOrientationAtUnit(waypointIndex, CinemachinePathBase.PositionUnits.PathUnits);
        transform.rotation = rot;
    }
}
