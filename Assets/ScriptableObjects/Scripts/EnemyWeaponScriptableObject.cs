using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWeaponScriptableObjects", menuName = "Scriptable Objects/EnemyWeaponScriptableObject")] 
public class EnemyWeaponScriptableObject : ScriptableObject
{
    public GameObject plasmaPulser;

    public GameObject getObject(string weaponName)
    {
        switch (weaponName)
        {
            case "Plasma Pulser":
                return plasmaPulser;
            default:
                return null;
        }
    }
}
