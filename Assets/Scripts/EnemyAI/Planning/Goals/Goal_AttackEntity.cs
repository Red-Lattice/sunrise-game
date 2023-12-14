using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_AttackEntity : I_Goal
{
    public GameObject target {get; private set;}
    private float damageDoneByTarget;
    private bool completed;
    private I_Goal[] subgoals;

    public Goal_AttackEntity()
    {
        subgoals = new I_Goal[1] {new Goal_PickUpWeapon()};
    }

    /// <summary>
    /// This is to help determine which entity the enemy should attack
    /// </summary>
    /// <returns>
    /// A value based on the amount of damage a specific enemy has done
    /// </returns>
    public float CalculatePriority()
    {
        return 1f + damageDoneByTarget;
    }

    public bool IsCompleted()
    {
        return completed;
    }

    public bool SubGoalsCompleted()
    {
        foreach (I_Goal goal in subgoals)
        {
            if (!goal.IsCompleted()) {return false;}
        }
        return true;
    }

    /// <summary>
    /// Runs when the enemy dies or is otherwise forgotten about
    /// </summary>
    public void UpdateGoal()
    {
        completed = true;
    }
}
