using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeaponTemplate", menuName = "Object Templates/Weapon Template")] 
public class SO_WeaponTemplate : ScriptableObject
{
    [field: Header("Prefabs")]
    [field: SerializeField] public GameObject prefab {get; private set;}
    [field: Header("Enums")]
    [field: SerializeField] public GunType gunType {get; private set;}
    [field: SerializeField] public BulletType bulletType {get; private set;}
    [field: SerializeField] public float cooldown {get; private set;}
    [field: SerializeField] public float range {get; private set;}
    [field: SerializeField] public uint maxAmmo {get; private set;}
    [field: SerializeField] public uint maxReserveAmmo {get; private set;}

    [field: Header("For Entities Only (Not Players)")]
    [field: SerializeField] public bool countShots {get; private set;}
    [field: SerializeField] public byte maxShotsInRow {get; private set;}
    [field: SerializeField] public float entityFiringCooldown {get; private set;}
}
