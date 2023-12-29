using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


/// <summary>
/// This script is attached to enemies, and manages all of the goals and actions.
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    private SafelyLinkedList<I_Action> actionQueue;
    private SafelyLinkedList<I_Goal> goalQueue;
    private NavMeshAgent pathfinder;
    private EnemyAwareness senses;
    private I_Action currentlyRunningAction;
    private I_Goal activeGoal;
    [SerializeField] private Transform headTransform;
    public GameObject target;
    private Dictionary<GameObject, Goal_AttackEntity> attackGoalSet;
    [SerializeField] private string[] LLVisualizer;
    [SerializeField] private string[] GoalVisualizer;
    [SerializeField] private EnemyWeaponScriptableObject gunGetter;

    void Awake()
    {
        LLVisualizer = new string[5];
        GoalVisualizer = new string[5];
        goalQueue = new SafelyLinkedList<I_Goal>(new Goal_Idle(this));
        actionQueue = new SafelyLinkedList<I_Action>(goalQueue.Head.data.GetActions()[0]);

        pathfinder = transform.gameObject.GetComponent<NavMeshAgent>();
        senses = transform.gameObject.GetComponent<EnemyAwareness>();
        attackGoalSet = new Dictionary<GameObject, Goal_AttackEntity>();

        weapon = transform.gameObject.GetComponentInChildren<Weapon>();
    }

    void FixedUpdate()
    {
        senses.Tick();

        ProcessSenses();

        Plan();

        ExecuteActions();

        debugVisualizer();
    }

    void Update()
    {
        if (activeGoal != null && activeGoal.GetType().Name == "Goal_Idle")
        {
            ((Goal_Idle)goalQueue.Head.data).UpdateGoal();
        }
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

        SafelyLinkedList<I_Goal>.Node currentGoalNode = goalQueue.Head;
        int j = 0;
        while (currentGoalNode != null)
        {
            GoalVisualizer[j] = currentGoalNode.data.GetType().Name;
            j++;
            currentGoalNode = currentGoalNode.nextNode;
        }
        while (j < 5)
        {
            GoalVisualizer[j] = "";
            j++;
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
            if (goalQueue.Head.data.GetType().Name == "Goal_AttackEntity")
            {
                target = ((Goal_AttackEntity)goalQueue.Head.data).target;
            }
            foreach (I_Goal subgoal in headSubGoals)
            {
                //Debug.Log(subgoal.IsCompleted());
                if (!subgoal.IsCompleted())
                {
                    activeGoal = subgoal;
                    actionQueue.Clear();
                    actionPlan(activeGoal);
                    return;
                }
            }
            activeGoal = goalQueue.Head.data;
            actionQueue.Clear();
            actionPlan(activeGoal);
            return;
        }
        actionPlan(activeGoal);
    }

    private void actionPlan(I_Goal goal)
    {
        I_Action[] actionList = goal.GetActions();
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
            Debug.Log("Action " + actionQueue.Head.data + " Executed");
            currentlyRunningAction = actionQueue.Head.data;
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
            GameObject go = entityCol.gameObject;
            if (attackGoalSet.ContainsKey(go))
            {
                continue;
            }
            Goal_AttackEntity attackScript = new Goal_AttackEntity(go, this);
            InsertIntoGoals(attackScript);
            attackGoalSet.Add(go, attackScript);
        }
    }

    public void Move(Vector3 location, I_Action caller)
    {
        moveToCoroutine = StartCoroutine(moveTo(location, caller));
    }

    public void StopMove(I_Action caller)
    {
        if (moveToCoroutine != null) {StopCoroutine(moveToCoroutine);}
    }

#region weaponStuff
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform weaponHoldPoint;

    /// <summary>
    /// Returns true if there is no weapon
    /// </summary>
    public bool weaponsNeededCheck()
    {
        return weapon == null;
    }
    public void setWeapon(string weaponName)
    {
        weapon = Instantiate(gunGetter.getObject(weaponName), weaponHoldPoint).GetComponent<Weapon>();
    }

    private Weapon getAddedWeapon(string weaponName)
    {
        // Every time you add a new weapon type, add it to this.
        switch (weaponName)
        {
            case "Pistol":
                return weaponHoldPoint.gameObject.AddComponent<Weapon_Pistol>();
            case "Plasma Pulser":
                return weaponHoldPoint.gameObject.AddComponent<Weapon_PlasmaPulser>();
            default:
                return weaponHoldPoint.gameObject.AddComponent<Weapon>();
        }
    }
#endregion

    public List<Collider> GetSmartObjectList()
    {
        return senses.smartObjects;
    }

    /// <summary>
    /// Replaces the head goal with this current one.
    /// NOTE: The goal calling this method should only ever be the head node of the goal list.
    /// </summary>
    /// <param name="newGoal"></param>
    public void Replace(I_Action swappableCaller, I_Action newAction)
    {
        I_Swappable goalWithSwap = goalQueue.Head.data as I_Swappable;

        if (goalWithSwap != null)
        {
            goalWithSwap.SwapIndex(Array.IndexOf(goalQueue.Head.data.GetActions(), swappableCaller), newAction);

            actionQueue.Replace(swappableCaller, newAction);
        }
    }

    private Coroutine moveToCoroutine;

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
                    caller.MarkCompleteness(true);
                    pathfinder.ResetPath();
                    actionQueue.Remove();
                    yield break;
                }
                unstuckTimer = 0;
                unstuckPosition = transform.position.magnitude;
            }
            yield return null;
        }
        caller.MarkCompleteness(true);
        pathfinder.ResetPath();
        actionQueue.Remove();
    }

    private Coroutine attackCoroutine;

    public void Attack(I_Action caller)
    {
        if (attackCoroutine != null) {StopCoroutine(attackCoroutine);}
        attackCoroutine = StartCoroutine(attack(caller));
    }

    private IEnumerator attack(I_Action caller) 
    {
        while (target != null)
        {
            Quaternion toRot = Quaternion.LookRotation(target.transform.position - transform.position);
            Quaternion lookRotation = Quaternion.Euler(0, toRot.eulerAngles.y, 0);
            Quaternion headRotation = Quaternion.Euler(toRot.eulerAngles.x, toRot.eulerAngles.y, toRot.eulerAngles.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 20 * Time.fixedDeltaTime);
            headTransform.rotation = Quaternion.RotateTowards(headTransform.rotation, headRotation, 20 * Time.fixedDeltaTime);
            weapon.transform.rotation = headTransform.rotation;
            weapon.triggerWeapon();
            yield return null;
        }
        caller.MarkCompleteness(true);
        actionQueue.Remove();
    }
}
