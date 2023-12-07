using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrenadesScriptableObject", menuName = "ScriptableObjects/Grenades")] 
public class GrenadesScriptableObject : ScriptableObject
{
    [SerializeField] private GameObject defaultGrenade;

    public GameObject getPrefab(string grenadeName)
    {
        switch (grenadeName)
        {
            case "Default":
                return defaultGrenade;
            default:
                return defaultGrenade;
        }
    }
}
