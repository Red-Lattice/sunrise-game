using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using static BulletType;

public class WarpWall : MonoBehaviour, IDamageable
{
    public bool capturing = false;
    private MeshRenderer wallMesh;
    private List<CapturedBullet> capturedBullets;
    private BoxCollider trigger;
    [SerializeField] private Transform WarpWallCenter;
    [SerializeField] private Transform cameraTransform;

    public Team GetTeam() {return Team.Green;}

    void Awake()
    {
        wallMesh = GetComponent<MeshRenderer>();
        wallMesh.enabled = false;
        trigger = GetComponent<BoxCollider>();
        trigger.enabled = false;
        capturedBullets = new List<CapturedBullet>();
    }

    void Update()
    {
        capturing = Input.GetMouseButton(1);
        if (capturing) {
            RotateCenter(transform);
        }

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
    private static void RotateCenter(Transform center) {
        center.RotateAround(center.position, center.forward, 250f * Time.deltaTime);
    }

    public void DealDamage(float damage, BulletType bulletType, GameObject dealer, Vector3 hitPos) 
    {
        AddBullet(bulletType);
    }

    public void AddBullet(BulletType damageType)
    {
        CapturedBullet bullet = new(BulletSingleton.instance.GetCapturedBullet(damageType), damageType);
        capturedBullets.Add(bullet);

        float angle = Random.Range(-180f, 180f);
        float distance = Random.Range(0f, 0.5f);
        var location = Quaternion.Euler(0, angle, 0) * WarpWallCenter.right * distance;

        bullet.bulletTransform.position = WarpWallCenter.position + location;
        bullet.bulletTransform.rotation = transform.rotation;

        bullet.bulletTransform.SetParent(WarpWallCenter);
    }

    private void Release()
    {
        foreach (CapturedBullet bullet in capturedBullets)
        {
            FireBullet(bullet.bulletTransform, bullet.capturedBulletType, cameraTransform.parent, cameraTransform.rotation);
            bullet.bullet.SetActive(false);
        }
        capturedBullets.Clear();
    }

    private static void FireBullet(Transform bulletTransform, BulletType bulletType, Transform firer, Quaternion rotation)
    {
        WeaponStruct wepStruct = new WeaponStruct {
            gunType = GunType.None,
            bulletType = bulletType, 
            range = 100f, 
            damage = Weapon.Bullet(bulletType).damage,
            cooldown = 0f, 
            ammo = 1,
            reserveAmmo = 1,
        };
        bulletTransform.rotation = rotation;
        Weapon.Fire(firer.gameObject, ref wepStruct, bulletTransform);
    }

    struct CapturedBullet {
        public GameObject bullet;
        public Transform bulletTransform;
        public BulletType capturedBulletType;

        public CapturedBullet(GameObject bullet, BulletType capturedBulletType) {
            this.bullet = bullet;
            this.bulletTransform = bullet.transform;
            this.capturedBulletType = capturedBulletType;
        }
    }
}