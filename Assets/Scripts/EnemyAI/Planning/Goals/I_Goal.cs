using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Goal
{
    public abstract float CalculatePriority();
    public abstract bool SubGoalsCompleted();
    public abstract bool IsCompleted();
    public abstract void UpdateGoal();
    public abstract I_Goal[] GetSubgoals();
    public abstract I_Action[] GetActions();
    public abstract bool IsRunning();
    public abstract void BeginExecution();
    public abstract void HaltExecution();
}
