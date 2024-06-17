using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{


    //AUS DEM PLAYERSTATES SKRIPTHOLEN




    public WaypointManager waypointManager;
    public float speed = 2.0f;
    public float reachThreshold = 0.1f;
    private int currentWaypointIndex = 0;
    private bool isMoving = true; // Flag to control movement
    
    void Update()
    {
        if (!isMoving || waypointManager.waypoints.Length == 0)
            return;

        Transform targetWaypoint = waypointManager.waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < reachThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypointManager.waypoints.Length)
            {
                isMoving = false; // Stop moving when the last waypoint is reached
            }
        }
    }
}



