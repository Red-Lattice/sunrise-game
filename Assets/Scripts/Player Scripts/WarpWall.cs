using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletType;

public class WarpWall : MonoBehaviour, IDamageable
{
    public bool capturing = false;
    private MeshRenderer wallMesh;
    private List<BulletType> arr;
    private BoxCollider trigger;
    [SerializeField] private ProjectileScriptableObjects projectiles;

    void Awake()
    {
        wallMesh = GetComponent<MeshRenderer>();
        wallMesh.enabled = false;
        trigger = GetComponent<BoxCollider>();
        trigger.enabled = false;
        arr = new List<BulletType>();
    }

    void Update()
    {
        capturing = Input.GetMouseButton(1);

        if (Input.GetMouseButtonDown(1))
        {
            wallMesh.enabled = true;
            trigger.enabled = true;
            capturing = true;
            return;
        }
        if (Input.GetMouseButtonUp(1))
        {
            wallMesh.enabled = false;
            trigger.enabled = false;
            capturing = false;
            Release();
        }
    }

    public void DealDamage(float damage, string bulletType, GameObject dealer, Vector3 hitPos) 
    {
        AddBullet(BulletSingleton.StringToBulletType(bulletType));
    }

    public void AddBullet(BulletType damageType)
    {
        arr.Add(damageType);
    }

    private void Release()
    {
        foreach (BulletType bullet in arr)
        {
            FireBullet(transform, bullet, projectiles);
        }
        arr.Clear();
    }

    private static void FireBullet(Transform firer, BulletType bulletType, ProjectileScriptableObjects pso)
    {
        switch (bulletType)
        {
            case Plasma_Pistol_Round:
                float angle = Random.Range(-180f, 180f);
                float distance = Random.Range(0f, 0.5f);
                var location = (Quaternion.Euler(0, angle, 0) * firer.right * distance);
                GameObject bullet = BulletSingleton.instance.GetBullet(bulletType);
                bullet.transform.position = firer.position + location;
                bullet.transform.rotation = firer.parent.rotation;
                bullet.GetComponent<PlasmaBullet>().initialization(firer.parent.parent.gameObject);
                return;
            default:
                break;
        }
    }
}