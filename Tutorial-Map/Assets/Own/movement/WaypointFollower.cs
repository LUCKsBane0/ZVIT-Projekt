using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    public WaypointManager waypointManager;
    public float speed = 2.0f;
    public float reachThreshold = 0.1f;
    public bool loopWaypoints = true; // Control whether the waypoints should loop

    private int currentWaypointIndex = 0;
    private bool isMoving = true;
    private bool isPaused = false; // Flag to indicate if movement is paused

    void Update()
    {
        if (isPaused || !isMoving || waypointManager.waypoints.Length == 0)
            return;

        Transform targetWaypoint = waypointManager.waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;

        // Move the VR character
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < reachThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypointManager.waypoints.Length)
            {
                if (loopWaypoints)
                {
                    currentWaypointIndex = 0; // Loop back to the first waypoint
                }
                else
                {
                    isMoving = false; // Stop moving if not looping
                }
            }
        }
    }

    public void PauseMovement()
    {
        isPaused = true;
    }


    public void ResumeMovement()
    {
        isPaused = false;
    }

    public void EnterCombat()
    {
        PauseMovement();
        // Additional combat setup code here
    }

    // Method to exit combat
    public void ExitCombat()
    {
        ResumeMovement();
    }
}