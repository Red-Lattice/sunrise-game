using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_PlasmaPulser : Weapon
{
    [SerializeField] private GameObject plasmaBolt;
    [SerializeField] private Camera cam;
    private float cooldown;
    private Quaternion rot;
    
    void Start()
    {
        cooldown = 0f;
        PlayerGunHandler pgh = this.transform.parent.GetComponent<PlayerGunHandler>();
        
        if (pgh != null)
        {
            plasmaBolt = pgh.getProjectileGetter().plasmaBall;
        }
        weaponName = "Plasma Pulser";

        cam = this.transform.parent.GetComponentInChildren<Camera>();
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
            PlasmaBullet instantiatedBolt = Instantiate(plasmaBolt, transform.position, rot).GetComponent<PlasmaBullet>();
            instantiatedBolt.initialization(this.transform.parent.gameObject);
            cooldown = 0.15f;
            return true;
        }
        return false;
    }

    public override bool triggerWeapon(Transform firerTransform)
    {
        if (cooldown < 0)
        {
            PlasmaBullet instantiatedBolt = Instantiate(plasmaBolt, firerTransform.position, rot).GetComponent<PlasmaBullet>();
            instantiatedBolt.initialization(this.transform.root.gameObject);
            cooldown = 0.15f;
            return true;
        }
        return false;
    }

    public override bool punch()
    {
        return true;
    }
}
