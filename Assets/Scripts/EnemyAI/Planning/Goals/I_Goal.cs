using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Goal
{
    public abstract float CalculatePriority();
    public abstract bool SubGoalsCompleted();
    public abstract bool IsCompleted();
    public abstract void UpdateGoal();
}
