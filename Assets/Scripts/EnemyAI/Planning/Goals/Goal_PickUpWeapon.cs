using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_PickUpWeapon : I_Goal
{
    private bool holdingWeapon;

    public Goal_PickUpWeapon()
    {
        holdingWeapon = false;
    }

    public float CalculatePriority()
    {
        throw new System.NotImplementedException();
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
