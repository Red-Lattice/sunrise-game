using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MoveTo : I_Action
{
    private I_Action[] actions;
    
    public Action_MoveTo()
    {
        actions = new I_Action[0];
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
