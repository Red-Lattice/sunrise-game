using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This goal is meant to serve as a placeholder for the base goal of an enemy
/// </summary>
public class Goal_Idle : I_Goal
{
    private I_Action[] actions;
    private I_Goal[] subgoals;

    public Goal_Idle()
    {
        actions = new I_Action[1]{new Action_Idle()};
        subgoals = new I_Goal[0];
    }

    public float CalculatePriority()
    {
        return 0f;
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
        return false; // This goal is never completable
    }

    public bool SubGoalsCompleted()
    {
        return true; // This goal has no sub goals
    }

    public void UpdateGoal() {}
}
