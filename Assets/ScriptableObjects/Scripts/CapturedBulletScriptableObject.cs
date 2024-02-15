using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CapturedBulletScriptableObject", menuName = "Scriptable Objects/CapturedBulletScriptableObject")] 
public class CapturedBulletScriptableObject : ScriptableObject
{
    public GameObject[] bulletsArray;
}
