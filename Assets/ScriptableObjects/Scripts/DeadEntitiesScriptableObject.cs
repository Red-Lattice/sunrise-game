using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* This class stores all of the prefabs for the different ragdolls for things elsewhere to reference.
*/
[CreateAssetMenu(fileName = "DeadEntitiesScripableObject", menuName = "Scriptable Objects/DeadEntitiesScripableObject")] 
public class DeadEntitiesScriptableObject : ScriptableObject
{
    public GameObject deadDefaultEnemy;

    public GameObject getDeadEntityPrefab(string entity)
    {
        switch (entity)
        {
            case "Enemy":
                return deadDefaultEnemy;
            default:
                return null;
        }
    }
}
