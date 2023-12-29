using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Action_MoveTo : I_Action
{
    private Vector3 position;
    private EnemyBrain brain;
    private bool finishedExecuting;
    private bool isExecuting;
    private NavMeshAgent pathfinder;

    public Action_MoveTo(EnemyBrain brain, Vector3 position)
    {
        this.position = position;
        this.brain = brain;
        this.pathfinder = brain.transform.gameObject.GetComponent<NavMeshAgent>();
    }

    public bool CanExecute()
    {
        return pathfinder != null;
    }

    public void ExecuteAction()
    {
        isExecuting = true;

        brain.Move(position, this);
    }

    public void HaltAction()
    {
        isExecuting = false;
        brain.StopMove(this);
    }

    public bool IsExecuted()
    {
        return finishedExecuting;
    }

    public bool IsExecuting()
    {
        return isExecuting;
    }

    public void MarkCompleteness(bool status)
    {
        finishedExecuting = status;
    }
}
