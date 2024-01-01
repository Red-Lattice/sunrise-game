using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Action_MeleeAttack : I_Action
{
    private NavMeshAgent pathfinder;
    private bool finishedExecuting;
    private EnemyBrain executor;
    private bool running;
    private Transform enemyTransform;
    private string currentStep;
    
    public Action_MeleeAttack(EnemyBrain executor, GameObject target)
    {
        this.pathfinder = executor.transform.gameObject.GetComponent<NavMeshAgent>();
        finishedExecuting = false;
        this.executor = executor;
        enemyTransform = executor.transform;
    }

    public bool CanExecute()
    {
        return pathfinder != null;
    }

    public void ExecuteAction()
    {
        Transform targetTransform = executor.target.transform;
        if (Vector3.Distance(enemyTransform.position, targetTransform.position) > 0.1f)
        {
            executor.MoveToTarget(this);
        }
    }

    public void HaltAction()
    {
        executor.StopMove(this);
        executor.StopAttack(this);
        running = false;
    }

    public bool IsExecuted()
    {
        return finishedExecuting;
    }

    public bool IsExecuting()
    {
        throw new System.NotImplementedException();
    }

    public void MarkCompleteness(bool status)
    {
        if (executor.target != null) // We've got more work to do
        {
            if ((executor.target.transform.position - executor.transform.position).magnitude > 1f)
            {
                if (currentStep == "Moving") {return;}
                currentStep = "Moving";
                executor.MoveToTarget(this);
            }
            else
            {
                if (currentStep == "Attacking") {return;}
                currentStep = "Attacking";
                executor.MeleeAttack(this);
            }
        }
    }
}
