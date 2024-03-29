using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWeaponScriptableObjects", menuName = "Scriptable Objects/EnemyWeaponScriptableObject")] 
public class EnemyWeaponScriptableObject : ScriptableObject
{
    public GameObject plasmaPulser;
    public GameObject pistol;

    public GameObject GetObject(GunType weaponType)
    {
        switch (weaponType)
        {
            case GunType.Pistol:
                return pistol;
            case GunType.PlasmaPulser:
                return plasmaPulser;
            default:
                return null;
        }
    }
}
