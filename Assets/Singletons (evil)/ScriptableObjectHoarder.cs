using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectHoarder : MonoBehaviour
{
    public List<SO_WeaponTemplate> WeaponTemplates;
    public List<SO_Bullet> Bullets;

    public static ScriptableObjectHoarder instance;
    void Awake() {
        if (instance != null) {
            Destroy(this);
            return;
        }
        instance = this;
    }
}
