using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Action_Attack : I_Action
{
    private NavMeshAgent pathfinder;
    private bool finishedExecuting;
    private EnemyBrain executor;
    private bool running;
    
    public Action_Attack(EnemyBrain executor)
    {
        this.pathfinder = executor.transform.gameObject.GetComponent<NavMeshAgent>();
        finishedExecuting = false;
        this.executor = executor;
    }

    public bool CanExecute()
    {
        throw new System.NotImplementedException();
    }

    public void ExecuteAction()
    {
        throw new System.NotImplementedException();
    }

    public void HaltAction()
    {
        throw new System.NotImplementedException();
    }

    public bool IsExecuted()
    {
        return executor.target == null;
    }

    public bool IsExecuting()
    {
        return running;
    }

    public void MarkCompleteness(bool status)
    {
        finishedExecuting = status;
    }
}
