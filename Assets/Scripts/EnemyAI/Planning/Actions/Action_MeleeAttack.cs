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
    private Transform targetTransform;
    
    public Action_MeleeAttack(EnemyBrain executor, GameObject target)
    {
        this.pathfinder = executor.transform.gameObject.GetComponent<NavMeshAgent>();
        finishedExecuting = false;
        this.executor = executor;
        enemyTransform = executor.transform;
        targetTransform = target.transform;
    }

    public bool CanExecute()
    {
        return true;
    }

    public void ExecuteAction()
    {
        if (Vector3.Distance(enemyTransform.position, targetTransform.position) > 0.1f)
        {
            executor.Move(targetTransform.position, this);
        }
    }

    public void HaltAction()
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }
}
