using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_PlasmaPulser : Weapon
{
    [SerializeField] private GameObject plasmaBolt;
    private float cooldown;
    
    void Start()
    {
        cooldown = 0f;
        PlayerGunHandler pgh = this.transform.parent.GetComponent<PlayerGunHandler>();
        if (pgh != null)
        {
            plasmaBolt = pgh.getProjectileGetter().plasmaBall;
        }
        weaponName = "Plasma Pulser";
    }

    void Update()
    {
        if (cooldown >= 0f)
        {
            cooldown -= Time.deltaTime;
        }
    }

    public override bool triggerWeapon()
    {
        if (cooldown < 0)
        {
            PlasmaBullet instantiatedBolt = Instantiate(plasmaBolt, transform.position, transform.rotation).GetComponent<PlasmaBullet>();
            instantiatedBolt.initialization(this.transform.root.gameObject.name);
            cooldown = 0.3f;
            return true;
        }
        return false;
    }
}
