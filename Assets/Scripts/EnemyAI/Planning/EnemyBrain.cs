using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Unity.Profiling;
//using static AiType;

/// <summary>
/// This enum defines how aggressive a given AI should be
/// </summary>
/*enum AiType{
    Aggressive,
    Defensive,
}*/

/// <summary>
/// This script is attached to enemies, and manages all of the goals and actions.
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    private I_Action[] actionQueue;
    [SerializeField] private byte actionCount;
    private SafelyLinkedList<I_Goal> goalQueue;
    private NavMeshAgent pathfinder;
    private EnemyAwareness senses;
    private I_Action currentlyRunningAction;
    private I_Goal activeGoal;
    [SerializeField] private Transform headTransform;
    public GameObject target;
    private const float meleeCooldown = 2f;
    private Dictionary<GameObject, Goal_AttackEntity> attackGoalSet;
    [SerializeField] private string[] LLVisualizer;
    [SerializeField] private string[] GoalVisualizer;
    [SerializeField] private EnemyWeaponScriptableObject gunGetter;
    private Vector3 targetLKP; //Last Known Position

    static readonly ProfilerMarker s_PreparePerfMarker = new ProfilerMarker("MySystem.Prepare");

    //[SerializeField] private AiType behavior;
    //public Enum GetAiType() {return behavior;}

    void Awake()
    {
        LLVisualizer = new string[5];
        GoalVisualizer = new string[5];
        goalQueue = new SafelyLinkedList<I_Goal>(new Goal_Idle(this));
        actionQueue = new I_Action[10];
        actionQueue[0] = goalQueue.Head.data.GetActions()[0];
        actionCount = 1;

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

        UpdateGoals();

        debugVisualizer();
    }

    void UpdateGoals()
    {
        if (activeGoal == null) {return;}
        if (activeGoal.GetType().Name == "Goal_Idle")
        {
            ((Goal_Idle)goalQueue.Head.data).UpdateGoal();
            return;
        }
        if (activeGoal.GetType().Name == "Goal_AttackEntity")
        {
            ((Goal_AttackEntity)goalQueue.Head.data).UpdateGoal();
        }
        if (activeGoal.IsCompleted())
        {
            goalQueue.Remove();
        }
    }

    private void debugVisualizer()
    {
        int i = 0;
        while (i < 5)
        {
            if (actionQueue[i] == null) {LLVisualizer[i] = "";}
            else {LLVisualizer[i] = actionQueue[i].GetType().Name;}
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
        s_PreparePerfMarker.Begin();
        // Once a new goal is created, the previous goal's action queue is destroyed
        I_Goal[] headSubGoals = goalQueue.Head.data.GetSubgoals();

        foreach (I_Goal subgoal in headSubGoals)
        {
            if (!subgoal.IsCompleted())
            {
                activeGoal = subgoal;
                ClearActions();
                actionPlan(activeGoal);
                return;
            }
        }

        if (activeGoal != goalQueue.Head.data || !headSubGoals.Contains(activeGoal))
        {
            if (goalQueue.Head.data.GetType().Name == "Goal_AttackEntity")
            {
                target = ((Goal_AttackEntity)goalQueue.Head.data).target;
            }
            foreach (I_Goal subgoal in headSubGoals)
            {
                if (!subgoal.IsCompleted())
                {
                    activeGoal = subgoal;
                    ClearActions();
                    actionPlan(activeGoal);
                    return;
                }
            }
            activeGoal = goalQueue.Head.data;
            ClearActions();
            actionPlan(activeGoal);
            return;
        }
        actionPlan(activeGoal);
        s_PreparePerfMarker.End();
    }

    private void ClearActions()
    {
        int i = 1;
        while (actionQueue[i] != null)
        {
            actionQueue[i] = null;
            actionCount--;
        }
    }

    private void actionPlan(I_Goal goal)
    {
        I_Action[] actionList = goal.GetActions();
        foreach (I_Action action in actionList)
        {
            if (!action.IsExecuted() && action.CanExecute() && !actionQueue.Contains(action))
            {
                actionQueue[actionCount] = action;
                actionCount++;
            }
        }
    }

    private void ExecuteActions()
    {
        I_Action item = actionQueue[actionCount - 1];
        if (item != currentlyRunningAction)
        {
            if (currentlyRunningAction != null) {currentlyRunningAction.HaltAction();}
            item.ExecuteAction();
            currentlyRunningAction = item;
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
        if (target != null 
            && (senses.potentialTargets.Contains(target) 
            || Vector3.Distance(target.transform.position, transform.position) < 10f))
        {
            targetLKP = target.transform.position;
        }
        foreach (GameObject entity in senses.potentialTargets)
        {
            if (attackGoalSet.ContainsKey(entity))
            {
                continue;
            }
            Goal_AttackEntity attackScript = new Goal_AttackEntity(entity, this);
            InsertIntoGoals(attackScript);
            attackGoalSet.Add(entity, attackScript);
        }
    }

    public void Move(Vector3 location, I_Action caller)
    {
        if (moveToCoroutine != null) {StopCoroutine(moveToCoroutine);}
        moveToCoroutine = StartCoroutine(moveTo(location, caller));
    }

    public void Strafe(Vector3 location, I_Action caller)
    {
        if (moveToCoroutine != null) {StopCoroutine(moveToCoroutine);}
        moveToCoroutine = StartCoroutine(strafeTo(location, caller));
    }

    public void StopMove(I_Action caller)
    {
        if (moveToCoroutine != null) {StopCoroutine(moveToCoroutine);}
    }

    public void MoveToTarget(I_Action caller)
    {
        if (moveToCoroutine != null) {StopCoroutine(moveToCoroutine);}
        moveToCoroutine = StartCoroutine(moveToTarget(caller));
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

    public List<GameObject> GetSmartObjectList()
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

            actionQueue[Array.IndexOf(actionQueue, swappableCaller)] = newAction;
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
                    actionQueue[actionCount - 1] = null;
                    actionCount--;
                    yield break;
                }
                unstuckTimer = 0;
                unstuckPosition = transform.position.magnitude;
            }
            yield return null;
        }
        caller.MarkCompleteness(true);
        pathfinder.ResetPath();
        actionQueue[actionCount - 1] = null;
        actionCount--;
    }

    /// Assumptions: This will only get called if there is a target
    private IEnumerator strafeTo(Vector3 location, I_Action caller) 
    {
        pathfinder.SetDestination(location);
        while ((location - transform.position).magnitude > 0.1f)
        {
            RotationHelper();
            yield return null;
        }
        caller.MarkCompleteness(true);
        pathfinder.ResetPath();
    }

    private IEnumerator moveToTarget(I_Action caller) 
    {
        pathfinder.SetDestination(targetLKP);
        while ((targetLKP - transform.position).magnitude > 1.5f)
        {
            pathfinder.SetDestination(targetLKP);
            yield return null;
        }
        caller.MarkCompleteness(true);
        pathfinder.ResetPath();
        actionQueue[actionCount - 1] = null;
        actionCount--;
    }

    private Coroutine attackCoroutine;
    public void Attack(I_Action caller)
    {
        if (attackCoroutine != null) {StopCoroutine(attackCoroutine);}
        attackCoroutine = StartCoroutine(attack(caller));
    }

    public void MeleeAttack(I_Action caller)
    {
        if (attackCoroutine != null) {StopCoroutine(attackCoroutine);}
        attackCoroutine = StartCoroutine(meleeAttack(caller));
    }

    public void StopAttack(I_Action caller)
    {
        if (attackCoroutine != null) {StopCoroutine(attackCoroutine);}
    }

    private IEnumerator attack(I_Action caller) 
    {
        float attackCooldown = 0f;
        byte counter = 0;
        while (target != null)
        {
            attackCooldown -= Time.deltaTime;
            if (senses.potentialTargets.Contains(target))
            {
                RotationHelper();
                if (attackCooldown <= 0f && weapon.triggerWeapon())
                {
                    counter++;
                }
                if (counter >= 4)
                {
                    attackCooldown = 1f;
                    counter = 0;
                }
                yield return null;
            }
            else
            {
                if (senses.unobstructedColliders.Contains(target))
                {
                    RotationHelperOmnipotent();
                    yield return null;
                }
                pathfinder.SetDestination(targetLKP);
                yield return null;
            }
        }
        caller.MarkCompleteness(true);
        actionQueue[actionCount - 1] = null;
        actionCount--;
    }

    private IEnumerator meleeAttack(I_Action caller) 
    {
        float cooldownTimer = -1f;
        while (target != null)
        {
            RotationHelper();
            if (cooldownTimer < 0f)
            {
                RaycastHit hit;
                if (Physics.Raycast(headTransform.position, headTransform.forward, out hit, 3f))
                {
                    StatManager statManager; 
                    if (hit.transform.TryGetComponent(out statManager))
                    {
                        statManager.DealDamage(30f, "Melee", this.gameObject, transform.position);
                    }
                }
                cooldownTimer = meleeCooldown;
                yield return null;
            }
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
        caller.MarkCompleteness(true);
        actionQueue[actionCount - 1] = null;
        actionCount--;
    }

    private void RotationHelper()
    {
        Quaternion toRot = Quaternion.LookRotation(targetLKP - transform.position);
        Quaternion lookRotation = Quaternion.Euler(0, toRot.eulerAngles.y, 0);

        Quaternion headRotation = Quaternion.Euler(toRot.eulerAngles.x, 
            toRot.eulerAngles.y, Mathf.Clamp(toRot.eulerAngles.z, -150f, 150f));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 20 * Time.fixedDeltaTime);
        headTransform.rotation = Quaternion.RotateTowards(headTransform.rotation, headRotation, 20 * Time.fixedDeltaTime);
        if (weapon != null) {weapon.transform.rotation = headTransform.rotation;}
    }

    private void RotationHelperOmnipotent()
    {
        Quaternion toRot = Quaternion.LookRotation(target.transform.position - transform.position);
        Quaternion lookRotation = Quaternion.Euler(0, toRot.eulerAngles.y, 0);

        Quaternion headRotation = Quaternion.Euler(toRot.eulerAngles.x, 
            toRot.eulerAngles.y, Mathf.Clamp(toRot.eulerAngles.z, -150f, 150f));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 20 * Time.fixedDeltaTime);
        headTransform.rotation = Quaternion.RotateTowards(headTransform.rotation, headRotation, 20 * Time.fixedDeltaTime);
        if (weapon != null) {weapon.transform.rotation = headTransform.rotation;}
    }

    public void InformOfDamage(GameObject dealer, float damage)
    {
        if (dealer == null) {return;} // Grenades trigger this.
        if (dealer.layer == LayerMask.NameToLayer("Enemy")) {return;}
        if (attackGoalSet.ContainsKey(dealer))
        {
            Goal_AttackEntity atGoal;
            attackGoalSet.TryGetValue(dealer, out atGoal);
            atGoal.UpdateDamage(damage);
            return;
        }
        Goal_AttackEntity attackScript = new Goal_AttackEntity(dealer, this);
        InsertIntoGoals(attackScript);
        attackGoalSet.Add(dealer, attackScript);
    }
}
