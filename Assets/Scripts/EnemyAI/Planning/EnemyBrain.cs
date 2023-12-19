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

    private int unstucktime;
    private Coroutine moveToCoroutine;
    private NavMeshAgent pathfinder;
    private EnemyAwareness senses;
    private I_Action currentlyRunningAction;
    private I_Goal activeGoal;
    [SerializeField] private GameObject target;
    private Dictionary<GameObject, Goal_AttackEntity> attackGoalSet;
    [SerializeField] private Weapon weapon;
    [SerializeField] private string[] LLVisualizer;

    void Awake()
    {
        LLVisualizer = new string[5];
        goalQueue = new SafelyLinkedList<I_Goal>(new Goal_Idle(this));
        actionQueue = new SafelyLinkedList<I_Action>(goalQueue.Head.data.GetActions()[0]);

        pathfinder = transform.gameObject.GetComponent<NavMeshAgent>();
        senses = transform.gameObject.GetComponent<EnemyAwareness>();
        attackGoalSet = new Dictionary<GameObject, Goal_AttackEntity>();

        weapon = transform.gameObject.GetComponentInChildren<Weapon>();
    }

    void Update()
    {
        senses.Tick();

        ProcessSenses();

        if (activeGoal != null && activeGoal.GetType().Name == "Goal_Idle")
        {
            ((Goal_Idle)goalQueue.Head.data).UpdateGoal();
        }
        
        Plan();

        ExecuteActions();

        debugVisualizer();
    }

    private void debugVisualizer()
    {
        SafelyLinkedList<I_Action>.Node currentNode = actionQueue.Head;
        int i = 0;
        while (currentNode != null)
        {
            LLVisualizer[i] = currentNode.data.GetType().Name;
            i++;
            currentNode = currentNode.nextNode;
        }
        while (i < 5)
        {
            LLVisualizer[i] = "";
            i++;
        }
    }

    /// <summary>
    /// Develops the enemy's current series of actions in order to complete
    /// the current priority goal
    /// </summary>
    private void Plan()
    {
        // Once a new goal is created, the previous goal's action queue is destroyed
        I_Goal[] headSubGoals = goalQueue.Head.data.GetSubgoals();
        if (activeGoal != goalQueue.Head.data || !headSubGoals.Contains(activeGoal))
        {
            foreach (I_Goal subgoal in headSubGoals)
            {
                if (!subgoal.IsCompleted())
                {
                    activeGoal = subgoal;
                    actionQueue.Clear();
                    actionPlan();
                    return;
                }
            }
            activeGoal = goalQueue.Head.data;
            actionQueue.Clear();
            actionPlan();
            return;
        }
        actionPlan();
    }

    private void actionPlan()
    {
        I_Action[] actionList = activeGoal.GetActions();
        foreach (I_Action action in actionList)
        {
            if (!action.IsExecuted() && action.CanExecute() && !actionQueue.Contains(action))
            {
                actionQueue.Add(action);
            }
        }
    }

    private void ExecuteActions()
    {
        if (actionQueue.Head.data != currentlyRunningAction)
        {
            if (currentlyRunningAction != null) {currentlyRunningAction.HaltAction();}
            actionQueue.Head.data.ExecuteAction();
            currentlyRunningAction = actionQueue.Head.data;
        }
        if (activeGoal.GetType().Name == "Goal_AttackEntity")
        {
            target = ((Goal_AttackEntity)activeGoal).target;
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
        foreach (Collider entityCol in senses.potentialTargets)
        {
            Goal_AttackEntity attackScript;
            if (attackGoalSet.TryGetValue(entityCol.gameObject, out attackScript))
            {
                continue;
            }
            attackScript = new Goal_AttackEntity(entityCol.gameObject, this);
            InsertIntoGoals(attackScript);
            attackGoalSet.Add(entityCol.gameObject, attackScript);
        }
    }

    public Coroutine Move(Vector3 location, I_Action caller)
    {
        return moveToCoroutine = StartCoroutine(moveTo(location, caller));
    }

    public void StopMove(I_Action caller)
    {
        if (moveToCoroutine != null) {StopCoroutine(moveToCoroutine);}
    }

    public bool weaponsNeededCheck()
    {
        return weapon == null;
    }
    public void setWeapon(string weaponName)
    {

    }

    private IEnumerator moveTo(Vector3 location, I_Action caller) 
    {
        int unstuckTimer = 0;
        float unstuckPosition = transform.position.magnitude;
        pathfinder.SetDestination(location);
        while ((location - transform.position).magnitude > 0.1f)
        {
            unstuckTimer++;
            if (unstuckTimer >= 50)
            {
                if (Mathf.Abs(unstuckPosition - transform.position.magnitude) < 0.001f)
                {
                    Debug.Log("Path fail");
                    caller.MarkCompleteness(true);
                    pathfinder.ResetPath();
                    actionQueue.Remove();
                    yield break;
                }
                else
                {
                    unstuckTimer = 0;
                    unstuckPosition = transform.position.magnitude;
                }
            }
            //Debug.Log(location);
            yield return null;
        }
        caller.MarkCompleteness(true);
        pathfinder.ResetPath();
        actionQueue.Remove();
    }
}
