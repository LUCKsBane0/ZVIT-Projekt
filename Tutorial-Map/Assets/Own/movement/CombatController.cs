using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    private WaypointFollower waypointFollower;
    private bool inCombat = false;
    public bool enableAlternating = true; // Flag to enable/disable alternating combat state for testing

    void Start()
    {
        waypointFollower = FindObjectOfType<WaypointFollower>();
        StartCoroutine(AlternateCombatState());
    }

    IEnumerator AlternateCombatState()
    {
        while (true)
        {
            if (enableAlternating)
            {
                yield return new WaitForSeconds(5f);

                if (inCombat)
                {
                    ExitCombat();
                }
                else
                {
                    EnterCombat();
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    void EnterCombat()
    {
        inCombat = true;
        waypointFollower.EnterCombat();
    }

    void ExitCombat()
    {
        inCombat = false;
        waypointFollower.ExitCombat();
    }

    public bool InCombat()
    {
        return inCombat;
    }

}

