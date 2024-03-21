using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityUpdater : MonoBehaviour
{
    public static EntityUpdater instance;
    [HideInInspector] public List<CleanAI> aiList;
    public SO_AIList aIList;
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
    private byte timeSinceUpdate = 0;
    private const byte updateTime = 5;
    void FixedUpdate() {
        ++timeSinceUpdate;
        if (timeSinceUpdate >= updateTime) {timeSinceUpdate = 0;}

        for (int i = timeSinceUpdate; i < aiList.Count; i += updateTime) {
            aiList[i].StaggeredUpdate();
        }
        foreach (CleanAI ai in aiList) {
            //ai.StaggeredUpdate();
            if (Weapon.NotNull(ai.HeldWeapon()) && ai.HeldWeapon().cooldown > 0f) {
                ai.UpdateCooldowns();
            }
        }
    }
}
