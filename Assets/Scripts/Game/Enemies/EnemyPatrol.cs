
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class EnemyPatrol : Enemy
{
    public enum State
    {
        Patrol,
        Seek
    }
    [Header("Enemy Patrol")]
    public State currentState = State.Patrol;
    public float distanceToWaypoint = 1f;
    public float detectionRadius = 5f;
    public Transform waypointParent;

    // Components
    private NavMeshAgent agent;

    // Waypoints
    private Transform target;
    private int currentIndex = 1;
    private Transform[] waypoints;

    #region Editor
    void OnDrawGizmosSelected()
    {
        // If the agent is in Patrol
        if (currentState == State.Patrol)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
    #endregion

    #region Internal
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();
        waypoints = waypointParent.GetComponentsInChildren<Transform>();
    }

    protected void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Seek:
                Seek();
                break;
            default:
                break;
        }
    }

    private void Patrol()
    {
        // Overlap sphere to detect things
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            Player player = hit.GetComponent<Player>();
            if (player)
            {
                target = player.transform;
                currentState = State.Seek;
            }
        }
        // If the currentIndex is out of waypoint range
        if (currentIndex >= waypoints.Length)
        {
            // Go back to "first" (actually second) waypoint
            currentIndex = 1;
        }
        // Set the current waypoint
        Transform point = waypoints[currentIndex];
        // Get distance to waypoint
        float distance = Vector3.Distance(transform.position, point.position);
        // If waypoint is within range
        if (distance <= distanceToWaypoint)
        {
            // Move to next waypoint (Next Frame)
            currentIndex++;
        }
        // Generate path to current waypoint
        agent.SetDestination(point.position);
    }
    private void Seek()
    {
        // Get distance to target
        float distToTarget = Vector3.Distance(transform.position, target.position);
        // If the target is outside detection range
        if (distToTarget >= detectionRadius)
        {
            // Switch to patrol
            currentState = State.Patrol;
        }
        // Update the AI's target position
        agent.SetDestination(target.position);
    }
    #endregion
}
