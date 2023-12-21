using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MoveTo : I_Action
{
    private Vector3 position;
    private EnemyBrain brain;
    public Action_MoveTo(EnemyBrain brain, Vector3 position)
    {
        this.position = position;
        this.brain = brain;
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
        throw new System.NotImplementedException();
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
