using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Profiling;
using UnityEngine.AI;
using UnityEngine.Animations;

public partial class CleanAI : MonoBehaviour
{
    [Header("Standard")]
    public EntityType entityType;
    private WeaponStruct weapon;
    public GunType defaultGun;
    public BrainData aiData;
    public Team team;
    public Transform weaponHoldPoint;
    public bool hasWeapon => Weapon.NotNull(ref weapon);
    public bool strafe = false;
    public byte boredom;
    static readonly ProfilerMarker staggeredUpdateCall = new ProfilerMarker("StaggeredUpdate()");

    void Start() {
        #if UNITY_EDITOR 
            if (existsOnDefaultLayer) {Debug.LogWarning("Entity exists on default layer. Potentially unwanted side effects!");} 
        #endif
        agent = GetComponent<NavMeshAgent>();
        targets = new Target[5];
        weapon = Weapon.BuildWeaponStruct(defaultGun, 100, 100);
        Debug.Log(hasWeapon);

        aiData = new BrainData(transform.position, Quaternion.identity);

        EntityUpdater.Subscribe(this);
        if (hasWeapon) {InitializeWeapon();}
        InitializeStats();
        BeginMoving();
        //StartDebug();
    }
    public GameObject gun;
    private void InitializeWeapon() {
        gun = Instantiate(ScriptableObjectHoarder.instance.WeaponTemplates[(int)weapon.gunType].prefab, weaponHoldPoint);
        //weaponHoldPoint.transform.LookAt(-transform.forward);
    }

    /// <summary>
    /// Called by fixed update
    /// </summary>
    public void UpdateCooldowns() {
        weapon.cooldown -= Time.fixedDeltaTime;
    }

    #if UNITY_EDITOR
        public bool existsOnDefaultLayer => transform.gameObject.layer == 0;
    #endif
    void Update() {
        //MoveTo(new Vector3(1,1,1) * 5f);
        //UpdateDebug();
    }

    public void StaggeredUpdate() {
        staggeredUpdateCall.Begin();
        this.GenerateActions();
        staggeredUpdateCall.End();
    }

    public ref WeaponStruct HeldWeapon() {
        return ref weapon;
    }
    public Vector3 targetPosition => target.targetTransform.position;
    public Quaternion targetRotation => Quaternion.LookRotation(transform.position - targetPosition);
    public LayerMask blockingLayerMask => 1 << 0;

    /// <summary>
    /// Draws a line between this entity and it's target.
    /// If anything intersects this line on a layer with collision it returns false.
    /// If clear, it returns true.
    /// </summary>
    /// <returns>Whether the AI could see the target</returns>
    public bool HasLineOfSightToTarget() {
        return !Physics.Raycast(weaponHoldPoint.position, 
            targetPosition, 
            DistanceToTarget(), 
            blockingLayerMask);
    }

    public float DistanceToTarget() {
        return Vector3.Distance(transform.position, targetPosition);
    }
}

#region coroutines
public partial class CleanAI : MonoBehaviour {
    private NavMeshAgent agent;
    private Coroutine moveToCoroutine;
    private bool moveToCoroutineActive => moveToCoroutine != null;
    #region profiler_markers
    static readonly ProfilerMarker moveToCoroutineMarker = new ProfilerMarker("MoveTo Coroutine");
    static readonly ProfilerMarker makeStruct = new ProfilerMarker("Making new BrainData struct");
    static readonly ProfilerMarker startingCoroutine = new ProfilerMarker("Starting a new Coroutine");
    #endregion

    public void BeginMoving() {
        moveToCoroutineMarker.Begin();
        makeStruct.Begin();
        aiData = new BrainData(transform.position, aiData.lookAtRot);
        makeStruct.End();
        startingCoroutine.Begin();
        if (moveToCoroutine == null) {moveToCoroutine = StartCoroutine(MoveToCoroutine());}
        startingCoroutine.End();
        moveToCoroutineMarker.End();
    }

    private IEnumerator MoveToCoroutine() {
        while (true) {
            if (agent.destination != aiData.moveToLocation) {
                agent.SetDestination(aiData.moveToLocation);
            }
            if (aiData.lookAtRot != Quaternion.identity) {
                agent.updateRotation = false;
                RotationHelper(aiData.lookAtRot);
            } else {agent.updateRotation = true;}
            yield return null;
        }
    }

    private void RotationHelper(Quaternion rotation)
    {
        Quaternion lookRotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

        //Quaternion headRotation = Quaternion.Euler(rotation.eulerAngles.x, 
        //    rotation.eulerAngles.y, Mathf.Clamp(rotation.eulerAngles.z, -150f, 150f));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 20 * Time.fixedDeltaTime);
        //headTransform.rotation = Quaternion.RotateTowards(headTransform.rotation, headRotation, 20 * Time.fixedDeltaTime);
        //weaponHoldPoint.rotation = Quaternion.RotateTowards(headTransform.rotation, headRotation, 20 * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Returns true if the ai is at a given position
    /// </summary>
    public bool DistanceCheck(Vector3 position) {
        return Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(position.x, 0, position.z)) < 0.25f;
    }
}
#endregion
#region stats
public partial class CleanAI : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float shield;
    private void InitializeStats() {
        SO_AI data = EntityUpdater.instance.aIList.List[(int)entityType];
        health = data.DefaultHealth;
        shield = data.DefaultShield;
        team = data.team;
    }
    public void DealDamage(float damage, BulletType bulletType, GameObject dealer, Vector3 hitPos) {
        UpdateTarget(dealer, damage);
        DamageStats(damage, bulletType);
    }

    private void DamageStats(float damage, BulletType bulletType) {
        DamageType damageType = Damage.BulletTypeToDamageType(bulletType);
        if (shield > 0f) {
            shield = Math.Clamp(shield - damage, 0f, EntityUpdater.instance.aIList.List[(int)entityType].DefaultShield);
            return;
        }
        if (health > 0f) {
            health = Math.Clamp(health - damage, 0f, EntityUpdater.instance.aIList.List[(int)entityType].DefaultHealth);
            return;
        }
        Die();
    }

    private void Die() {

    }
}
#endregion
#region targeting_system
public partial class CleanAI : MonoBehaviour {
    public byte targetIndex = 255;
    private Target[] targets;
    private Target target {
        get {return targets[targetIndex % 255];}
    }
    public bool hasTarget => targetIndex != 255;

    private void UpdateTarget(GameObject target, float damage) {
        if (HasAsTarget(target.transform, out int index)) {
            targets[index].damage += damage;
            goto UpdateTarget; // Goto haters seething
        }
        if (index == -1) {return;} // No more room for targets
        targets[index] = new Target(target.layer, target.transform, damage);

        UpdateTarget:
        if (!hasTarget || targets[targetIndex].damage < targets[index].damage) {targetIndex = (byte)index;}
    }

    private bool HasAsTarget(Transform targetTransform, out int index) {
        for (int i = 0; i < targets.Length; i++) {
            index = i;
            if (targets[i].initialized == false) {return false;}
            if (targets[i].targetTransform == targetTransform) {return true;}
        }
        index = -1;
        return false;
    }
}
#endregion
#region debug
public partial class CleanAI : MonoBehaviour {
    static readonly ProfilerMarker debug = new ProfilerMarker("AI Debug (You can ignore this)");
    [Header("Debug")]
    [SerializeField] List<string> targetsDebug;
    [SerializeField] Vector3 goalPosition;
    private void StartDebug() {
        targetsDebug = new List<string>
        {
            targets[0].initialized.ToString() + " | " + targets[0].damage.ToString(),
            targets[1].initialized.ToString() + " | " + targets[1].damage.ToString(),
            targets[2].initialized.ToString() + " | " + targets[2].damage.ToString(),
            targets[3].initialized.ToString() + " | " + targets[3].damage.ToString(),
            targets[4].initialized.ToString() + " | " + targets[4].damage.ToString(),
        };
    }
    private void UpdateDebug() {
        debug.Begin();
        for (int i = 0; i < 5; ++i) {
            targetsDebug[i] = targets[i].initialized.ToString() + " | " + targets[i].damage.ToString();
        }
        goalPosition = aiData.moveToLocation;
        debug.End();
    }
}
#endregion
#region enums

public enum ItemType {
    None,
    Weapon,
    Grenade,
}

public enum EntityType {
    Default,
}

public enum Team {
    None,
    Green,
    Red,
    Blue,
}

public enum LowerBodyAction {
    Stand,
    Crouch,
    Walk,
    CrouchWalk,
}
/*public enum UpperBodyAction {None,Fire,ThrowGrenade,}*/
public enum SpecialActions {
    None,
    Die,
}
public enum AttackAction {
    Fire,
    ThrowGrenade,
}

#endregion
#region structs
/// <summary>
/// This struct holds all data an enemy needs to make decisions.
/// </summary>
public readonly struct BrainData {
    public readonly Vector3 moveToLocation;
    public readonly Quaternion lookAtRot;

    public BrainData(Vector3 moveToLocation, Quaternion lookAtRot) {
        this.moveToLocation = moveToLocation;
        this.lookAtRot = lookAtRot;
    }
}

public readonly struct ActionData {
    public readonly LowerBodyAction lowerBody;
    public readonly AttackAction attack;
    public readonly SpecialActions specialActions; // Overrides all other actions
}

/// <summary>
/// This struct holds information about a target that it is aware of.
/// </summary>
public struct Target {
    public bool initialized {get; private set;}
    public byte layer {get; private set;}
    public Transform targetTransform  {get; private set;}
    public float damage;

    public Target(int layer, Transform targetTransform, float damage) {
        this.layer = (byte)layer;
        this.targetTransform = targetTransform;
        this.damage = damage;
        initialized = true;
    }
    public void Destroy() {
        layer = 0;
        targetTransform = null;
        damage = 0f;
        initialized = false;
    }
}
#endregion