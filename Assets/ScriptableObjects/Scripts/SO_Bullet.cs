using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DamageType;

[CreateAssetMenu(fileName = "SO_Bullet", menuName = "Object Templates/Bullet")] 
public class SO_Bullet : ScriptableObject
{
    [field: SerializeField] public BulletType bulletType {get; private set;}
    [field: SerializeField] public DamageType damageType {get; private set;}
    [field: SerializeField] public float damage {get; private set;}
    [field: SerializeField] public GameObject prefab {get; private set;}
}
