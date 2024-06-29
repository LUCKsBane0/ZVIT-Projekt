using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    private PlayerStates playerStates;
    // Start is called before the first frame update
    void Start()
    {
        playerStates = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<PlayerStates>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BossTrigger")){
            playerStates.hasPassedBoss = true;
        }
    }
}
