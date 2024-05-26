using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_animation_handler : MonoBehaviour
{
    private Animator mAnimator;
    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            if(true)
            {
                mAnimator.SetTrigger("TrCombat");
            }

            if (Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                mAnimator.SetTrigger("TrAttack");
            }

        }
    }
}
