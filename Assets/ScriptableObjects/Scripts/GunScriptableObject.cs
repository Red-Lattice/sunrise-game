using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* This class stores all of the prefabs for the different guns for things elsewhere to reference.
*/
[CreateAssetMenu(fileName = "GunScriptableObjects", menuName = "Scriptable Objects/GunScriptableObjects")] 
public class GunScriptableObject : ScriptableObject
{
    public GameObject pistol;
    public GameObject plasmaPulser;

    public GameObject getGunPrefab(string gun)
    {
        switch (gun)
        {
            case "Pistol":
                return pistol;
            case "Plasma Pulser":
                return plasmaPulser;
            default:
                return null;
        }
    }
}
