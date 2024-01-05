using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Pistol : Weapon
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask shootableLayers;
    private float cooldown;
    
    void Start()
    {
        cooldown = 0f;
        cam = this.transform.parent.GetComponentInChildren<Camera>();
        shootableLayers = LayerMask.GetMask("Default", "whatIsWall", "whatIsGround", "Enemy");
        weaponName = "Pistol";
    }

    public override bool triggerWeapon()
    {
        if (cooldown > 0f) {return false;}
        cooldown = 0.15f;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, shootableLayers))
        {
            StatManager statManager; 
            if (hit.transform.TryGetComponent<StatManager>(out statManager))
            {
                statManager.dealDamage(30f, "Physical", cam.transform.parent.gameObject);
            }
        }
        return true;
    }

    void Update()
    {
        if (cooldown >= 0f)
        {
            cooldown -= Time.deltaTime;
        }
    }

    public override bool punch()
    {
        return true;
    }
}
