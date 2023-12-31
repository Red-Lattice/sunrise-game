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
    private GameObject target;
    private Transform targetTransform;

    
    public Action_Attack(EnemyBrain executor, GameObject target)
    {
        this.pathfinder = executor.transform.gameObject.GetComponent<NavMeshAgent>();
        finishedExecuting = false;
        this.executor = executor;
        this.target = target;
        targetTransform = target.transform;
    }

    public bool CanExecute()
    {
        return !executor.weaponsNeededCheck();
    }

    public void ExecuteAction()
    {
        if (SwapCheck()) {return;}
        executor.Attack(this);
    }

    private bool SwapCheck()
    {
        /*if (executor.weaponsNeededCheck() && executor.GetSmartObjectList().Count == 0) 
        {
            executor.Replace(this, new Action_MeleeAttack(executor, target));
            return true;
        }*/
        return false;
    }

    public void HaltAction()
    {
        running = false;
        executor.StopAttack(this);
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
