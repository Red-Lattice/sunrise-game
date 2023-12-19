using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the default action for an enemy.
/// </summary>
public class Action_Idle : I_Action
{
    public bool isExecuting {get; private set;}
    public Action_Idle(){}

    public bool CanExecute()
    {
        return true; // This action can always run.
    }

    public void ExecuteAction()
    {
        isExecuting = true;
    }

    public bool IsExecuted()
    {
        return false; // This action is never to be "completed". It's the default action.
    }

    public void HaltAction()
    {
        isExecuting = false;
    }

    public bool IsExecuting()
    {
        return isExecuting;
    }

    public void MarkCompleteness(bool status)
    {
        // This will not do anything
    }
}
