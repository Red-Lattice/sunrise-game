using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Wander : Goal_Base
{
    [SerializeField] int MinPriority = 0;
    [SerializeField] int MaxPriority = 30;
    [SerializeField] float PriorityBuildRate = 1f;
    [SerializeField] float PriorityDecayRate = 0.1f;
    float CurrentPriority = 0f;

    public override int CalculatePriority()
    {
        return Mathf.FloorToInt(CurrentPriority);
    }

    public override void OnGoalActivated()
    {
        CurrentPriority = MaxPriority;
    }

    public override bool CanRun()
    {
        // Wander can always run
        return true;
    }

    public override void OnTickGoal()
    {
        // if (Agent.IsMoving)
        //    currentPriority -= PriorityDecayRate * Time.deltaTime;
        // else
        //    currentPriority += PriorityBuildRate * Time.deltaTime;
    }
}
