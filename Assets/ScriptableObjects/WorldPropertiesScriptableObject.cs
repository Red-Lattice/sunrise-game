using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldPropertiesScriptableObject", menuName = "ScriptableObjects/WorldProperties")] 
public class WorldPropertiesScriptableObject : ScriptableObject
{
    [SerializeField] private float gravity;

    public float getGravity()
    {
        return gravity;
    }
}
