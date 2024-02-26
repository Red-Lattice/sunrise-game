using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeaponTemplate", menuName = "Object Templates/Weapon Template")] 
public class SO_WeaponTemplate : ScriptableObject
{
    [field: SerializeField] public GunType gunType {get; private set;}
    [field: SerializeField] public BulletType bulletType {get; private set;}
    [field: SerializeField] public float cooldown {get; private set;}
    [field: SerializeField] public float range {get; private set;}
    [field: SerializeField] public uint maxAmmo {get; private set;}
    [field: SerializeField] public uint maxReserveAmmo {get; private set;}
}
