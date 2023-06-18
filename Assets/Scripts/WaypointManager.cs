using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Waypoint[] waypoints;
    public static WaypointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    } 

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;
        
        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        Gizmos.color = Color.yellow;
        foreach (Waypoint waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint.position, 0.2f);
        }
    }

    public Waypoint[] GetWaypoints()
    {
        return waypoints;
    }
}
