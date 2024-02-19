using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletType;

public class Weapon_PlasmaPulser : Weapon
{
    [SerializeField] private Camera cam;
    private float cooldown;
    private Quaternion rot;
    private GameObject shooter;
    
    void Start()
    {
        cooldown = 0f;
        PlayerGunHandler pgh = this.transform.parent.GetComponent<PlayerGunHandler>();
        
        weaponName = "Plasma Pulser";

        cam = this.transform.root.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (cooldown >= 0f)
        {
            cooldown -= Time.deltaTime;
        }

        rot = (cam != null) ? cam.transform.rotation : transform.rotation;
    }

    public override bool triggerWeapon()
    {
        if (cooldown < 0)
        {
            PlasmaBullet instantiatedBolt = BulletSingleton.instance.GetBullet(Plasma_Pistol_Round).GetComponent<PlasmaBullet>();
            instantiatedBolt.transform.position = transform.position;
            instantiatedBolt.transform.rotation = rot;
            instantiatedBolt.initialization(shooter);
            cooldown = 0.15f;
            return true;
        }
        return false;
    }

    public override bool triggerWeapon(Transform firerTransform)
    {
        if (cooldown < 0)
        {
            PlasmaBullet instantiatedBolt = BulletSingleton.instance.GetBullet(Plasma_Pistol_Round).GetComponent<PlasmaBullet>();
            instantiatedBolt.transform.position = firerTransform.position;
            instantiatedBolt.transform.rotation = rot;
            instantiatedBolt.initialization(shooter);
            cooldown = 0.15f;
            return true;
        }
        return false;
    }

    public override bool punch()
    {
        return true;
    }

    public override Weapon SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
        return this;
    }
}
