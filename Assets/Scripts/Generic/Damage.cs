using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DamageType;
using static BulletType;

public enum DamageType
{
    Physical,
    Kinetic,
    Energy,
    Explosion
}

public class Damage : MonoBehaviour
{
    public static DamageType BulletTypeToDamageType(BulletType bulletType)
    {
        switch (bulletType)
        {
            case Melee:
                return Physical;
            case Kinetic_Small: 
                return Kinetic;
            case Plasma_Pistol_Round:
                return Energy;
            case Grenade:
                return Explosion;
            default:
                #if UNITY_EDITOR
                    throw new System.Exception("Bullet type not found: " + bulletType);
                #else
                    return Physical;
                #endif
        }
    }
}
