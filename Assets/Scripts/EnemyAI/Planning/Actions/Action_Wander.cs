using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Action_Wander :  I_Action
{
    private NavMeshAgent pathfinder;
    private bool finishedExecuting;
    public bool isExecuting {get; private set;}
    private EnemyBrain executor;

    public Action_Wander(EnemyBrain executor)
    {
        this.pathfinder = executor.transform.gameObject.GetComponent<NavMeshAgent>();
        finishedExecuting = false;
        this.executor = executor;
    }

    public bool CanExecute()
    {
        return pathfinder != null;
    }

    public void ExecuteAction()
    {
        isExecuting = true;
        float angle = Random.Range(-180f, 180f);
        float distance = Random.Range(0f, 3f);
        Vector3 location = (Quaternion.Euler(0, angle, 0) * Vector3.forward * distance)
            + executor.transform.position;

        executor.Move(location, this);
    }

    public void HaltAction()
    {
        isExecuting = false;
        executor.StopMove(this);
    }

    public bool IsExecuted()
    {
        return finishedExecuting;
    }

    public bool IsExecuting()
    {
        return isExecuting;
    }
}
