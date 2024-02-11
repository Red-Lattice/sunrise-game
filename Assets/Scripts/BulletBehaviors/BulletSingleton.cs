using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletType;

public enum BulletType {
    Melee,
    Kinetic_Small,
    Plasma_Pistol_Round,
    Grenade,
}

public class BulletSingleton : MonoBehaviour {
    public static BulletSingleton instance { get; private set; }

    [SerializeField] private ProjectileScriptableObjects pso;
    public static List<GameObject>[] pooledObjects = new List<GameObject>[4]; // The index corresponds to the enum index
    public static LayerMask shootableLayers = 1 | 256 | 512 | 2048;
    
    public static BulletType StringToBulletType(string bulletType)
    {
        switch (bulletType) {
            case "Plasma_Pistol_Round":
                return Plasma_Pistol_Round;
            default:
                return Melee;
        }
    }
    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton()
    {
        if (instance != null) {Destroy(gameObject); return;}
        instance = this;
        pooledObjects[(int)Plasma_Pistol_Round] = new List<GameObject>();
        for (int i = 0; i < 4; i++) {
            AddNewBulletToPool(pooledObjects[(int)Plasma_Pistol_Round], Plasma_Pistol_Round);
        }
    }

    public GameObject GetBullet(BulletType bulletType) {
        return FindValidBullet(pooledObjects[(int)bulletType], bulletType);
    }

    private GameObject FindValidBullet(List<GameObject> objs, BulletType bulletType) {
        foreach (GameObject bullet in objs) {
            if (!bullet.activeInHierarchy) {
                return ActivateBullet(bullet);
            }
        }
        return ActivateBullet(AddNewBulletToPool(objs, bulletType));
    }

    private static GameObject ActivateBullet(GameObject go) {
        go.SetActive(true);
        return go;
    }

    private GameObject AddNewBulletToPool(List<GameObject> objList, BulletType bulletType) {
        GameObject go = Instantiate(FindBulletPrefabFromType(bulletType));
        objList.Add(go);
        go.SetActive(false);
        return go;
    }
    private GameObject FindBulletPrefabFromType(BulletType bulletType) {
        return pso.bullets[(int)bulletType];
    }
}