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
    private WeaponStruct weaponStructDontGrabThis;
    ref WeaponStruct weapon => ref weaponStructDontGrabThis;
    public GunType defaultGun;
    public BrainData aiData;
    public Team team;
    public Transform weaponHoldPoint;
    [SerializeField] private Animator shieldAnimator;
    public bool hasWeapon => Weapon.NotNull(ref weapon);
    public bool strafe = false;
    public byte boredom;
    public bool fireTrigger = false;
    static readonly ProfilerMarker staggeredUpdateCall = new ProfilerMarker("StaggeredUpdate()");

    void Start() {
        #if UNITY_EDITOR 
            if (existsOnDefaultLayer) {Debug.LogWarning("Entity exists on default layer. Potentially unwanted side effects!");} 
        #endif
        agent = GetComponent<NavMeshAgent>();
        targets = new Target[5];
        weaponStructDontGrabThis = Weapon.BuildWeaponStruct(defaultGun, 100, 100);

        aiData = new BrainData(transform.position, Quaternion.identity);

        EntityUpdater.Subscribe(this);
        if (hasWeapon) {InitializeWeapon();}
        InitializeStats();
        BeginMoving();
        //StartDebug();
    }
    public Animator gun;
    private void InitializeWeapon() {
        gun = Instantiate(
                scriptableObjects.WeaponTemplates[(int)weapon.gunType].prefab,
                weaponHoldPoint
            ).GetComponent<Animator>();
    }
    private ScriptableObjectHoarder scriptableObjects => ScriptableObjectHoarder.instance;

    /// <summary>
    /// Called by fixed update
    /// </summary>
    public void UpdateCooldowns() {
        //UpdateDebug();
        DecrementIfNonZero(ref weapon.cooldown);
        DecrementIfNonZero(ref fireCooldown);
        DecrementIfNonZero(ref entityStun);
    }

    public static void DecrementIfNonZero(ref float value) {
        if (value <= 0f) {return;}
        value -= Time.fixedDeltaTime;
    }

    #if UNITY_EDITOR
        public bool existsOnDefaultLayer => transform.gameObject.layer == 0;
    #endif

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
    public LayerMask blockingLayerMask => 1 << 0 | 1 << 8 | 1 << 9;//TeamToLayer();

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

    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        //Gizmos.color = Color.red;
        //Vector3 direction = weaponHoldPoint.forward * DistanceToTarget();
        //Gizmos.DrawRay(weaponHoldPoint.position, direction);
    }

    public float DistanceToTarget() {
        return Vector3.Distance(transform.position, targetPosition);
    }
}

#region coroutines
public partial class CleanAI : MonoBehaviour {
    private NavMeshAgent agent;
    private Coroutine coroutineContainer;

    public void BeginMoving() {
        aiData = new BrainData(transform.position, aiData.lookAtRot);
        if (coroutineContainer == null) {coroutineContainer = StartCoroutine(BigCoroutine());}
    }

    private IEnumerator BigCoroutine() {
        while (true) {
            if (entityStun > 0f) {agent.ResetPath(); goto STOP;}
            if (agent.destination != aiData.moveToLocation) {
                agent.SetDestination(aiData.moveToLocation);
            }

            if (aiData.lookAtRot != Quaternion.identity) {
                agent.updateRotation = false;
                RotationHelper(aiData.lookAtRot);
            }
            else {agent.updateRotation = true;}

            if (fireTrigger) {Fire();}
            STOP:
            yield return null;
        }
    }
    private byte timesShot;
    private float fireCooldown = 0f;
    private void Fire() {
        if (DistanceToTarget() < 2f) {MeleeAttack(); MeleeStun(); return;}
        if (fireCooldown > 0f) {return;}
        if (!Weapon.Fire(gameObject, ref weapon, weaponHoldPoint)) {return;}

        SO_WeaponTemplate weaponTemplate = scriptableObjects.WeaponTemplates[(int)weapon.gunType];
        if (weaponTemplate.countShots) {
            timesShot++;
        }
        if (timesShot >= weaponTemplate.maxShotsInRow) {
            timesShot = 0;
            fireCooldown = weaponTemplate.entityFiringCooldown;
        }
    }
    [field: SerializeField] private float entityStun = 0f;
    public float stunValue => entityStun; // Lmao
    private const float stunLength = 1.5f;
    private void MeleeAttack() {
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 3f)) {return;}

        if (hit.transform.TryGetComponent(out IDamageable damageable)) {
            damageable.DealDamage(
                30f,
                BulletType.Melee,
                transform.gameObject,
                hit.point
            );
        }
    }

    private void MeleeStun() {
        entityStun = stunLength;
        DecisionMaker.Stun(this);
    }

    private void RotationHelper(Quaternion rotation)
    {
        AimBodyAndHead(rotation);
        //headTransform.rotation = Quaternion.RotateTowards(headTransform.rotation, headRotation, 20 * Time.fixedDeltaTime);
        AimGun(rotation);
    }
    private static float turnSpeed => 20 * Time.fixedDeltaTime;

    private void AimBodyAndHead(Quaternion rotation) {
        Quaternion lookRotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

        Quaternion headRotation = Quaternion.Euler(rotation.eulerAngles.x, 
            rotation.eulerAngles.y, Mathf.Clamp(rotation.eulerAngles.z, -150f, 150f));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed);
    }

    private void AimGun(Quaternion rotation) {
        if (hasTarget) {Quaternion.RotateTowards(weaponHoldPoint.rotation, Quaternion.LookRotation(weaponHoldPoint.position - targetPosition), turnSpeed); return;}

        Quaternion headRotation = Quaternion.Euler(rotation.eulerAngles.x, 
            rotation.eulerAngles.y, Mathf.Clamp(rotation.eulerAngles.z, -150f, 150f));

        weaponHoldPoint.rotation = Quaternion.RotateTowards(weaponHoldPoint.rotation, headRotation, turnSpeed);
    }

    /// <summary>
    /// Returns true if the ai is at a given position
    /// </summary>
    public bool DistanceCheck(Vector3 position) {
        return Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(position.x, 0, position.z)
        ) < 0.25f;
    }

    public bool AcceptableAngleToTarget() {
        return Vector3.Angle(
            targetPosition - transform.position,
            weaponHoldPoint.forward
        ) < 25f;
    }
    public bool CanHitTarget() {
        return !Physics.Raycast(weaponHoldPoint.position, 
            weaponHoldPoint.forward, 
            DistanceToTarget(), 
            blockingLayerMask & (~(1 << target.targetTransform.gameObject.layer)));
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
        if (dealer.layer != gameObject.layer) {UpdateTarget(dealer, damage);}
        DamageStats(damage, bulletType);
    }

    private bool hasShieldAnimator => shieldAnimator != null;

    private void DamageStats(float damage, BulletType bulletType) {
        DamageType damageType = Damage.BulletTypeToDamageType(bulletType);
        if (shield > 0f) {
            shield = Math.Clamp(shield - damage, 0f, EntityUpdater.instance.aIList.List[(int)entityType].DefaultShield);
            if (!hasShieldAnimator) {return;}
            if (shield <= 0f) {shieldAnimator.Play("ShieldPop", -1, 0f);} else {shieldAnimator.Play("Flash", -1, 0f);}
            return;
        }
        if (health > 0f) {
            health = Math.Clamp(health - damage, 0f, EntityUpdater.instance.aIList.List[(int)entityType].DefaultHealth);
        }
        if (health <= 0f) Die();
    }

    public Team GetTeam() {return team;}

    /// <summary>
    /// These are the layers that the AI will consider "blocking their view"
    /// If a friendly (to the player) steps in an enemy's path to shoot the player, they'll continue shooting
    /// </summary>
    public int TeamToLayer() {
        switch (team) {
            case Team.Red:
                return 1 << 0 | 1 << 9;
            case Team.Green:
                return 1 << 0 | 1 << 8;
            case Team.Blue:
            case Team.None:
                break;
        }
        return 1 << 0;
    }

    private void Die() {
        Instantiate(EntityUpdater.instance.aIList.List[(int)entityType].deadPrefab, transform.position, transform.rotation);
        StopAllCoroutines();
        EntityUpdater.instance.aiList.Remove(this);
        Destroy(gameObject);
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