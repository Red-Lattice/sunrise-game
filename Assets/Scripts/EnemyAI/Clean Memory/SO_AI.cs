using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_AI", menuName = "Scriptable Objects/AI")]
public class SO_AI : ScriptableObject
{
    public GameObject prefab;
    public float DefaultHealth;
    public float DefaultShield;
    public Team team;
}

