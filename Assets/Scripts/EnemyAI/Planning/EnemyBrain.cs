using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// This script is attached to enemies, and manages all of the goals and actions.
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    private SafelyLinkedList<I_Action> actionQueue;
    private SafelyLinkedList<I_Goal> goalQueue;

    private Coroutine moveToCoroutine;
    private NavMeshAgent pathfinder;
    private EnemyAwareness senses;
    private I_Action currentlyRunningAction;
    [SerializeField] private GameObject target;
    [SerializeField] private LayerMask dlm;
    private Dictionary<GameObject, Goal_AttackEntity> attackGoalSet;

    void Awake()
    {
        actionQueue = new SafelyLinkedList<I_Action>(new Action_Idle());
        goalQueue = new SafelyLinkedList<I_Goal>(new Goal_Idle());
        pathfinder = transform.gameObject.GetComponent<NavMeshAgent>();
        senses = transform.gameObject.GetComponent<EnemyAwareness>();
        attackGoalSet = new Dictionary<GameObject, Goal_AttackEntity>();

        actionQueue.Add(new Action_Wander(this));
    }

    void Update()
    {
        senses.Tick();

        UpdateGoals();
        
        Plan();
    }

    private void UpdateGoals()
    {
        foreach (Collider entityCol in senses.directVisionConeColliders)
        {
            Goal_AttackEntity attackScript;
            if (attackGoalSet.TryGetValue(entityCol.gameObject, out attackScript))
            {
                continue;
            }
            attackScript = new Goal_AttackEntity(entityCol.gameObject);
            InsertIntoGoals(attackScript);
        }
    }

    /// <summary>
    /// Develops the enemy's current series of actions in order to complete
    /// the current priority goal
    /// </summary>
    private void Plan()
    {
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
        if (goalQueue.Head.data.GetType().Name == "Goal_AttackEntity")
        {
            I_Goal script = goalQueue.Head.data;
            Goal_AttackEntity convertedScript = (Goal_AttackEntity)script; // lol
            target = convertedScript.target;
        }
    }

    /// <summary>
    /// If I was smart I would have put this into the safely linked list
    /// class with a comparator. Unfortunately I am not smart.
    /// </summary>
    /// <param name="goal"></param>
    private void InsertIntoGoals(I_Goal goal)
    {
        SafelyLinkedList<I_Goal>.Node currentNode = goalQueue.Head;
        int index = 0;
        while (goal.CalculatePriority() < currentNode.data.CalculatePriority())
        {
            currentNode = currentNode.nextNode;
            index++;
        }
        goalQueue.Add(index, goal);
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
