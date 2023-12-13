using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// This script is attached to enemies, and manages all of the goals and actions.
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    private SafelyLinkedList<I_Action> actionQueue;

    private Coroutine moveToCoroutine;
    private NavMeshAgent pathfinder;
    private I_Action currentlyRunningAction;

    void Awake()
    {
        actionQueue = new SafelyLinkedList<I_Action>(new Action_Idle());
        pathfinder = transform.gameObject.GetComponent<NavMeshAgent>();
        actionQueue.Add(new Action_Wander(transform.gameObject, this));
    }

    void Update()
    {
        Debug.Log(actionQueue.Head.data);
        if (!actionQueue.Head.data.IsExecuting() && actionQueue.Head.data.CanExecute())
        {
            if (currentlyRunningAction != null) {currentlyRunningAction.HaltAction();}
            if (moveToCoroutine != null) {StopCoroutine(moveToCoroutine);}
            actionQueue.Head.data.ExecuteAction();
            currentlyRunningAction = actionQueue.Head.data;
        }
        if (UnityEngine.Random.Range(0, 1000) == 500)
        {
            actionQueue.Add(new Action_Wander(transform.gameObject, this));
        }
    }

    public void Move(Vector3 location, I_Action caller)
    {
        moveToCoroutine = StartCoroutine(moveTo(location, caller));
    }

    private IEnumerator moveTo(Vector3 location, I_Action caller) 
    {
        pathfinder.SetDestination(location);
        while ((location - transform.position).magnitude > 0.1f)
        {
            yield return null;
        }
        pathfinder.ResetPath();
        actionQueue.Remove();
    }
}
