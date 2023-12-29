using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBoundingBox : MonoBehaviour
{
    private EnemyBrain brain;

    void Start()
    {
        brain = GetComponentInParent<EnemyBrain>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherGO = other.gameObject;
        if (otherGO.layer == LayerMask.NameToLayer("objects"))
        {
            switch (otherGO.tag)
            {
                case "Weapon":
                    addWeapon(otherGO);
                    break;
                default:
                    break;
            }
        }
    }
    void addWeapon(GameObject go)
    {
        string foundWeapon = go.name;
        if (brain.weaponsNeededCheck())
        {
            brain.setWeapon(foundWeapon);
            Destroy(go);
        }
    }
}
