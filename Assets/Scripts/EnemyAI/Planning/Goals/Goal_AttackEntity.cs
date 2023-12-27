using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_AttackEntity : I_Goal, I_Swappable
{
    public GameObject target {get; private set;}
    private float damageDoneByTarget;
    private bool completed;
    private I_Goal[] subgoals;
    private I_Action[] actions;
    private bool running;

    public Goal_AttackEntity(GameObject target, EnemyBrain goalee)
    {
        actions = new I_Action[1] {new Action_Attack(goalee, target)};
        this.target = target;

        if (goalee.weaponsNeededCheck() && goalee.GetSmartObjectList().Count != 0)
        {
            subgoals = new I_Goal[1] {
                new Goal_PickUpWeapon(goalee, 
                goalee.GetSmartObjectList()[0].transform.position)
                };
            return;
        }
        subgoals = new I_Goal[0];
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

    public I_Goal[] GetSubgoals()
    {
        return subgoals;
    }

    public I_Action[] GetActions()
    {
        return actions;
    }

    public bool IsRunning()
    {
        return running;
    }

    public void BeginExecution()
    {
        running = true;
    }

    public void HaltExecution()
    {
        running = false;
    }

    public void SwapIndex(int index, I_Action newAction)
    {
        actions[index] = newAction;
    }
}
