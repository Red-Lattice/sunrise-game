using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObjects", menuName = "Scriptable Objects/ProjectileScriptableObjects")] 
public class ProjectileScriptableObjects : ScriptableObject
{
    public GameObject plasmaBall;

    public GameObject[] bullets = new GameObject[4];
}
