using System;
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
    private bool running;
    private float boredom;
    private float requiredBoredom;
    private EnemyBrain brain;

    public Goal_Idle(EnemyBrain brain)
    {
        boredom = 0f;
        requiredBoredom = UnityEngine.Random.Range(200f, 1000f);
        this.brain = brain;
        actions = new I_Action[2]{new Action_Idle(), new Action_Wander(brain)};
        subgoals = new I_Goal[0];

        actions[1].MarkCompleteness(true);
    }

    public void BeginExecution()
    {
        running = true;
    }

    public float CalculatePriority()
    {
        return -1f;
    }

    public I_Action[] GetActions()
    {
        return actions;
    }

    public I_Goal[] GetSubgoals()
    {
        return subgoals;
    }

    public void HaltExecution()
    {
        running = false;
    }

    public bool IsCompleted()
    {
        return false; // This goal is never completable
    }

    public bool IsRunning()
    {
        return running;
    }

    public bool SubGoalsCompleted()
    {
        return true; // This goal has no sub goals
    }

    public void UpdateGoal() 
    {
        boredom += Time.deltaTime * 100f;
        if (boredom > requiredBoredom)
        {
            actions[1].MarkCompleteness(false);
            boredom = 0f;
            requiredBoredom = UnityEngine.Random.Range(200f, 1000f);
        }
    }
}
