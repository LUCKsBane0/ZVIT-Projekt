using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
   
    public bool inCombat = false; // Relocate to PlayerStates!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!




    public bool enableAlternating = true; // Flag to enable/disable alternating combat state for testing

    void Start()
    {
        
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
       
    }

    void ExitCombat()
    {
        inCombat = false;
        
    }

    public bool InCombat()
    {
        return inCombat;
    }

}

