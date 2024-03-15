using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Profiling;


public partial class CleanAI : MonoBehaviour
{
    private WeaponStruct weapon;
    public unsafe WeaponStruct* weaponPtr;
    public GunType defaultGun;
    public AI_Data aiData;
    public bool needToBeUpdated;
    //static readonly ProfilerMarker val_update = new ProfilerMarker("new_target()");
    
    unsafe void Start() {
        targets = new Target[5];
        weapon = Weapon.BuildWeaponStruct(defaultGun, 100, 100);
        fixed(WeaponStruct* weapon_ptr = &weapon)
        fixed(WeaponStruct* weaponPtr = &weapon)

        aiData = new AI_Data(transform.position, AI_Animation.Idle, ItemType.None, weapon_ptr);

        EntityUpdater.Subscribe(this);
    }

    /// <summary>
    /// Called by fixed update
    /// </summary>
    public void UpdateCooldowns() {
        weapon.cooldown -= Time.fixedDeltaTime;
    }

    public void UpdateThis() {

    }

    
}

#region coroutines
public partial class CleanAI : MonoBehaviour {

}
#endregion
#region targeting_system
public partial class CleanAI : MonoBehaviour {
    private byte targetIndex = 255;
    private Target[] targets;

    private void UpdateTarget(GameObject target, float damage) {
        int index;
        if (HasAsTarget(target.transform, out index)) {
            targets[index].damage += damage;
            if (targetIndex != 255 && targets[targetIndex].damage < targets[index].damage) {targetIndex = (byte)index;}
        }
        if (index == -1) {return;}
        targets[index] = new Target(target.layer, target.transform, damage);
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

#region enums
public enum AI_Animation {
    Idle,
}

public enum ItemType {
    None,
    Weapon,
    Grenade,
}
#endregion
#region structs
/// <summary>
/// This struct holds all data an enemy needs to make decisions.
/// </summary>
public unsafe readonly struct AI_Data {
    public readonly Vector3 moveToLocation;
    public readonly AI_Animation animation;
    public readonly ItemType item;
    public readonly WeaponStruct* weapon;

    public AI_Data(Vector3 moveToLocation, AI_Animation animation, ItemType item, WeaponStruct* weapon) {
        this.moveToLocation = moveToLocation;
        this.animation = animation;
        this.item = item;
        this.weapon = weapon;
    }
}

/// <summary>
/// This struct holds information about a target that it is aware of.
/// </summary>
struct Target {
    public readonly bool initialized;
    public readonly byte layer;
    public readonly Transform targetTransform;
    public float damage;

    public Target(int layer, Transform targetTransform, float damage) {
        this.layer = (byte)layer;
        this.targetTransform = targetTransform;
        this.damage = damage;
        initialized = true;
    }
}
#endregion