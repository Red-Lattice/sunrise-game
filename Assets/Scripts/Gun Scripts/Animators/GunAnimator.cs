using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunAnimator : MonoBehaviour
{
    #region Fields

    private Animator animator;
    private string currentAnimState;
    private string currentPlayerState;

    #endregion

    void Start()
    {
        animator = this.transform.GetComponent<Animator>();
        changeAnimState("Idle");
    }

    #region Animation_State_Changers
    /* This method changes the animation state, but has a guard in place to make sure
    *  the animation doesn't get overridden */
    public void changeAnimState(string newAnimState)
    {
        if (currentAnimState == newAnimState) {return;}

        animator.Play(newAnimState);

        currentAnimState = newAnimState;
    }

    /* This method is postly the same as changeAnimState() but doesn't have the guard in place. */
    public void changeAnimStateOverride(string newAnimState)
    {
        animator.Play(newAnimState);

        currentAnimState = newAnimState;
    }

    /* Creates a smooth transition between two animations (ideally) */
    public void crossfadeAnimState(string newAnimState)
    {
        try
        {
            if (currentAnimState == newAnimState) {return;}

            animator.CrossFadeInFixedTime(newAnimState, 0.3f);

            currentAnimState = newAnimState;
        }
        catch (NullReferenceException nre)
        {
            return;
        }
    }

    public void updateCurrentPlayerState(string newPlayerState)
    {
        currentPlayerState = newPlayerState;

        switch (currentAnimState)
        {
            case "Fire":
                break;
            default:
                crossfadeAnimState(currentPlayerState);
                break;
        }
    }
    #endregion


    #region Special_Methods
    /******************************Special Methods*****************************/

    public void fire()
    {
        changeAnimStateOverride("Fire");
    }

    // Triggered by the gun firing animation upon finishing.
    public void firingFinished()
    {
        changeAnimState(currentPlayerState);
    }
    #endregion
}
