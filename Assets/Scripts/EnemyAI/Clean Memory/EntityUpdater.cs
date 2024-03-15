using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityUpdater : MonoBehaviour
{
    public static EntityUpdater instance;
    public List<CleanAI> aiList;
    void Awake() {
        if (instance != null) {
            Destroy(this);
            return;
        }
        instance = this;
        aiList = new List<CleanAI>();
    }

    public static void Subscribe(CleanAI ai) {
        instance.aiList.Add(ai);
    }

    unsafe void FixedUpdate() {
        foreach (CleanAI ai in aiList) {
            if (Weapon.NotNull(ai.weaponPtr) && (*ai.weaponPtr).cooldown > 0f) {
                ai.UpdateCooldowns();
            }
            if (ai.needToBeUpdated) {ai.UpdateThis();}
        }
    }
}
