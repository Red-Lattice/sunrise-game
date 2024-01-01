using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Pistol : Weapon
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask shootableLayers;
    
    void Start()
    {
        cam = this.transform.parent.GetComponentInChildren<Camera>();
        shootableLayers = LayerMask.GetMask("Default", "whatIsWall", "whatIsGround", "Enemy");
        weaponName = "Pistol";
    }

    public override bool triggerWeapon()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, shootableLayers))
        {
            StatManager statManager; 
            if (hit.transform.TryGetComponent<StatManager>(out statManager))
            {
                statManager.dealDamage(30f, "Physical", transform.root.gameObject);
            }
        }
        return true;
    }

    public override bool punch()
    {
        return true;
    }
}
