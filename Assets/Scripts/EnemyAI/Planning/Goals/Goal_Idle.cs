using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This goal is meant to serve as a placeholder for the base goal of an enemy
/// </summary>
public class Goal_Idle : I_Goal
{
    public float CalculatePriority()
    {
        return 0f;
    }

    public bool IsCompleted()
    {
        return false; // This goal is never completable
    }

    public bool SubGoalsCompleted()
    {
        return true; // This goal has no sub goals
    }

    public void UpdateGoal() {}
}
