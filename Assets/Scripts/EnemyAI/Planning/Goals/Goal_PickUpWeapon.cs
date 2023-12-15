using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_PickUpWeapon : I_Goal
{
    private bool holdingWeapon;
    private I_Action[] actions;
    private I_Goal[] subgoals;

    public Goal_PickUpWeapon()
    {
        holdingWeapon = false;
        actions = new I_Action[1]{new Action_MoveTo()};
        subgoals = new I_Goal[0];
    }

    public float CalculatePriority()
    {
        throw new System.NotImplementedException();
    }

    public I_Action[] GetActions()
    {
        return actions;
    }

    public I_Goal[] GetSubgoals()
    {
        return subgoals;
    }

    public bool IsCompleted()
    {
        return holdingWeapon;
    }

    public bool SubGoalsCompleted()
    {
        return true; // Picking up a weapon has no sub-goals that need to be completed
    }

    /// <summary>
    /// Run this for the pick up weapon goal upon a weapon being collected
    /// </summary>
    public void UpdateGoal()
    {
        holdingWeapon = true;
    }
}
