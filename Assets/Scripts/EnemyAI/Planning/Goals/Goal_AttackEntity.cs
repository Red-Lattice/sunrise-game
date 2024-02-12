using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_AttackEntity : I_Goal
{
    public GameObject target {get; private set;}
    private float damageDoneByTarget;
    private float oldDamageDoneByTarget; // This exists to check *when* an enemy recieves damage
    private bool completed;
    private I_Goal[] subgoals;
    private I_Action[] actions;
    private bool running;
    private EnemyBrain executor;

    public Goal_AttackEntity(GameObject target, EnemyBrain goalee)
    {
        executor = goalee;
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
        return (target == null || target.activeInHierarchy == false);
    }

    public bool SubGoalsCompleted()
    {
        foreach (I_Goal goal in subgoals)
        {
            if (!goal.IsCompleted()) {return false;}
        }
        return true;
    }

    public void UpdateGoal()
    {
        SwapChecks();
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

    public void UpdateDamage(float damage)
    {
        damageDoneByTarget += damage;
    }

    /// <summary>
    /// This is a special method for attacking.
    /// It does checks to see if it should swap its attacking actions with
    /// different ones. Example: Swapping melee attacks vs ranged
    /// </summary>
    public void SwapChecks()
    {
        if ((actions[0].GetType().Name != "Action_MeleeAttack")
            && executor.weaponsNeededCheck() && executor.GetSmartObjectList().Count == 0)
        {
            actions[0].HaltAction();
            actions[0] = new Action_MeleeAttack(executor, target);

            return;
        }

        if ((actions[0].GetType().Name != "Action_Attack")
            && !executor.weaponsNeededCheck())
        {
            actions[0].HaltAction();
            actions[0] = new Action_Attack(executor, target);
            return;
        }

        if (damageDoneByTarget - oldDamageDoneByTarget > 10f)
        {
            if (Random.Range(0, 100) < 50)
            {
                Vector3 temp = executor.transform.position - target.transform.position;
                Vector3 normed = (new Vector3(-temp.z, 0, temp.x)).normalized;
                
                executor.Strafe(normed * Random.Range(-3f, 3f) + executor.transform.position, actions[0]);
            }
        }
        else
        {
            if (Random.Range(0, 100) < 20)
            {
                Vector3 temp = executor.transform.position - target.transform.position;
                Vector3 normed = (new Vector3(-temp.z, 0, temp.x)).normalized;
                
                executor.Strafe(normed * Random.Range(-3f, 3f) + executor.transform.position, actions[0]);
            }
        }

        if (Vector3.Distance(executor.target.transform.position, executor.transform.position) < 5f)
        {
            Vector3 temp = executor.transform.position - target.transform.position;
            executor.Strafe(temp * 0.5f + executor.transform.position, actions[0]);
        }
        oldDamageDoneByTarget = damageDoneByTarget;
    }
}
