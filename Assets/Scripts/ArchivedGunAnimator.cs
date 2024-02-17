using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchivedGunAnimator : MonoBehaviour
{
    /*private Animator animator;
    private string currentAnimState;
    private string currentPlayerState;

    [SerializeField] private string currentContinuousState;
    [SerializeField] private string queuedAction;

    [SerializeField] private bool actionPlaying;
    private bool blockActions;

    void Start()
    {
        queuedAction = nameof(Enter);
        actionPlaying = false;
        animator = this.transform.GetComponent<Animator>();
    }

    #region Animation_State_Changers
    /// This method changes the animation state, but has a guard in place to make sure
    /// the animation doesn't get overridden
    public void changeAnimState(string newAnimState)
    {
        if (currentAnimState == newAnimState) {return;}

        animator.Play(newAnimState);

        currentAnimState = newAnimState;
    }

    /// This method is postly the same as changeAnimState() but doesn't have the guard in place.
    public void changeAnimStateOverride(string newAnimState)
    {
        animator.Play(newAnimState);

        currentAnimState = newAnimState;

        queuedAction = "";
    }

    /// Creates a smooth transition between two animations (ideally)
    public void crossfadeAnimState(string newAnimState)
    {
        try
        {
            if (currentAnimState == newAnimState) {return;}

            animator.CrossFadeInFixedTime(newAnimState, 0.05f);

            currentAnimState = newAnimState;
        }
        catch (NullReferenceException)
        {
            return;
        }
        queuedAction = "";
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

    public void fire()
    {
        setActionState("Fire");
    }

    public bool punch()
    {
        if (!blockActions)
        {
            setActionState("Punch");
            blockActions = true;
            return true;
        }
        return false;
    }

    #endregion

    #region New_Anim_State_Changers
    
    public void setContinuousState(string newAnimState)
    {
        currentContinuousState = newAnimState;
    }
    public void setActionState(string newAnimState)
    {
        queuedAction = newAnimState;
    }

    #endregion

    void Update()
    {
        if (!actionPlaying && queuedAction == "")
        {
            changeAnimState(currentContinuousState);
            return;
        }

        if (queuedAction == "") {return;}
        if (actionPlaying && blockActions) {return;}

        if (actionPlaying)
        {
            changeAnimStateOverride(queuedAction);
        }
        else
        {
            crossfadeAnimState(queuedAction);
        }
        actionPlaying = true;
        queuedAction = "";
    }

    public void actionStateFinished()
    {
        actionPlaying = false;
        blockActions = false;
    }

    // Returns whether actions like shooting are blocked or not
    public bool getActionsBlocked()
    {
        return blockActions;
    }*/
}
