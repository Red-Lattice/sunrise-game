using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_AIList", menuName = "Scriptable Objects/AI List")]
public class SO_AIList : ScriptableObject
{
    public List<SO_AI> List;
}
