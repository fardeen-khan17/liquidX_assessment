using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    public Waypoint[] waypoints;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 0.1f;
    public float moveSpeed = 7f;
    public float returnToPatrolDelay = 1f;
    public float targetOffset = 2f;
    public float soundDetectionRadius = 10f;
    public LayerMask guardLayerMask;
    

    private int currentWaypointIndex;
    private Waypoint targetWaypoint;
    //private bool isAlerted = false;
    private GameObject player;
    private NavMeshAgent agent;
    private int lastPatrolWaypointIndex;
    private float detectionRange = 15f;
    private float detectionAngle = 22.5f;
    private Light spotLight;
    private Vector3 lastKnownPosition;
    private float returnToPatrolTimer = 0f;
    private enum GuardState
    {
        Patrol,
        Alerted
    }
    private GuardState currentState;

    private void Start()
    {
        currentState = GuardState.Patrol;
        waypoints = WaypointManager.Instance.GetWaypoints();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        agent.stoppingDistance = stoppingDistance;
        spotLight = transform.GetComponentInChildren<Light>();
        detectionRange = spotLight.range;
        detectionAngle = spotLight.spotAngle / 2f;
        NoiseManager.Instance.OnNoiseMade += DetectNoise;

        if (waypoints.Length > 0)
            SetTargetWaypoint(0);
       
    }

    private void Update()
    {
        PlayerDetection();
        switch (currentState)
        {
            case GuardState.Patrol:
                // Handle patrol behavior
                MoveGuardOnWayPoints();
                break;

            case GuardState.Alerted:
                // Handle alerted behavior
                ChasePlayer();
                break;
        }
    }

    private void MoveGuardOnWayPoints()
    {
        if (targetWaypoint == null)
            return;
        
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        float interpolationFactor = Mathf.Clamp01(moveSpeed * Time.deltaTime / distanceToWaypoint);
        transform.position = Vector3.Lerp(transform.position, targetWaypoint.position, interpolationFactor);
        Quaternion targetRotation = Quaternion.LookRotation(targetWaypoint.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (distanceToWaypoint <= stoppingDistance)
        {            
            SetNextWaypoint();
        }
    }

    /* This Function will allow guard to move on Way points using the Nav Mesh Agent.
    private void MoveGuardOnWayPointsThroughNavMesh()
    {
        if (targetWaypoint == null)
            return;

        if (Vector3.Distance(transform.position, targetWaypoint.position) <= stoppingDistance)
        {
            SetNextWaypoint();
        }
        agent.SetDestination(targetWaypoint.position);
    }*/

    private void SetTargetWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Length)
        {
            currentWaypointIndex = index;
            targetWaypoint = waypoints[currentWaypointIndex];
        }
    }

    private void SetNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Length)
            currentWaypointIndex = 0;
        SetTargetWaypoint(currentWaypointIndex);
    }

    public void AlertGuard(Vector3 playerPosition)
    {
        currentState = GuardState.Alerted;
        agent.isStopped = false;
        returnToPatrolTimer = 0f;

        Vector3 playerDirection = playerPosition - transform.position;
        playerDirection.y = 0f; 
        playerDirection.Normalize();

        //Stops the Guard Near to the player.
        lastKnownPosition = playerPosition - playerDirection * targetOffset;
    }

    public void ReturnToPatrol()
    {
        currentState = GuardState.Patrol;
        returnToPatrolTimer = 0f;
        agent.isStopped = true;
        int nearestWaypointIndex = FindNearestWaypoint();
        if (nearestWaypointIndex != -1)
        {
            currentWaypointIndex = nearestWaypointIndex;
            SetTargetWaypoint(currentWaypointIndex);
        }
    }

    private int FindNearestWaypoint()
    {
        int nearestIndex = -1;
        float nearestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (i != lastPatrolWaypointIndex)
            {
                Vector3 targetPosition = waypoints[i].position;
                Vector3 direction = targetPosition - currentPosition;

                // Check for obstacles along the line of sight
                if (!Physics.Raycast(currentPosition, direction, direction.magnitude))
                {
                    float distance = Vector3.Distance(currentPosition, targetPosition);
                    if (distance < nearestDistance)
                    {
                        nearestIndex = i;
                        nearestDistance = distance;
                    }
                }
            }
        }

        lastPatrolWaypointIndex = currentWaypointIndex;
        return nearestIndex;
    }


    private void PlayerDetection()
    {
        Vector3 playerDirection = player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        float distanceToPlayer = playerDirection.magnitude;

        if (angleToPlayer <= detectionAngle && distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerDirection, out hit, distanceToPlayer, ~guardLayerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("Gaurd Allerted By Vision");
                    AlertGuard(player.transform.position);
                }
            }
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(lastKnownPosition);

        if (Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(lastKnownPosition.x, lastKnownPosition.z)) <= agent.stoppingDistance)
        {
            returnToPatrolTimer += Time.deltaTime;
            if (returnToPatrolTimer >= returnToPatrolDelay)
            {
                ReturnToPatrol();
            }
        }
    }

    public void DetectNoise(Vector3 noisePosition)
    {
        if (currentState == GuardState.Patrol)
        {
            float distanceToNoise = Vector3.Distance(transform.position, noisePosition);
            if (distanceToNoise <= soundDetectionRadius)
            {
                Vector3 directionToNoise = noisePosition - transform.position;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToNoise, out hit, distanceToNoise, ~guardLayerMask))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Gaurd Allerted By Noise");
                        AlertGuard(noisePosition);
                    }
                }
            }
        }
    }


    //For Debugging the angke of the cone.
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;

    //    Vector3 coneDirection = transform.forward;
    //    Vector3 from = Quaternion.AngleAxis(-detectionAngle, Vector3.up) * transform.forward;
    //    Handles.DrawWireArc(transform.position, Vector3.up, from, detectionAngle * 2, detectionRange);

    //    Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-detectionAngle, Vector3.up) * coneDirection * detectionRange);
    //    Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(detectionAngle, Vector3.up) * coneDirection * detectionRange);

    //}
}
