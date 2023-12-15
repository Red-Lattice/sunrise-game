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
    [SerializeField] private SafelyLinkedList<I_Action> actionQueue;
    private SafelyLinkedList<I_Goal> goalQueue;

    private Coroutine moveToCoroutine;
    private NavMeshAgent pathfinder;
    private EnemyAwareness senses;
    private I_Action currentlyRunningAction;
    private GameObject target;
    [SerializeField] private LayerMask dlm;

    void Awake()
    {
        actionQueue = new SafelyLinkedList<I_Action>(new Action_Idle());
        goalQueue = new SafelyLinkedList<I_Goal>(new Goal_Idle());
        pathfinder = transform.gameObject.GetComponent<NavMeshAgent>();
        senses = transform.gameObject.GetComponent<EnemyAwareness>();

        actionQueue.Add(new Action_Wander(this));
    }

    void Update()
    {
        //senses.Tick();

        UpdateGoals();
        
        if (!actionQueue.Head.data.IsExecuting() && actionQueue.Head.data.CanExecute())
        {
            if (currentlyRunningAction != null) {currentlyRunningAction.HaltAction();}
            actionQueue.Head.data.ExecuteAction();
            currentlyRunningAction = actionQueue.Head.data;
            Debug.Log(currentlyRunningAction);
        }
        if (UnityEngine.Random.Range(0, 1000) == 500)
        {
            actionQueue.Add(new Action_Wander(this));
        }
    }

    private void UpdateGoals()
    {
        //senses.directVisionConeColliders
    }

    private void ProcessSenses()
    {

    }

    public void Move(Vector3 location, I_Action caller)
    {
        moveToCoroutine = StartCoroutine(moveTo(location, caller));
    }

    public void StopMove(I_Action caller)
    {
        if (moveToCoroutine != null) {StopCoroutine(moveToCoroutine);}
    }

    private IEnumerator moveTo(Vector3 location, I_Action caller) 
    {
        pathfinder.SetDestination(location);
        while ((location - transform.position).magnitude > 0.1f)
        {
            if (pathfinder.path.status != NavMeshPathStatus.PathComplete)
            {
                Debug.Log("Path fail");
                break;
            }
            yield return null;
        }
        pathfinder.ResetPath();
        actionQueue.Remove();
    }
}
