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
    }

    public override bool triggerWeapon()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, shootableLayers))
        {
            TestEnemyStatManager hitEnemy = hit.transform.GetComponent<TestEnemyStatManager>();
            if (hitEnemy != null)
            {
                hitEnemy.DealDamage(30f);
            }
        }
        return true;
    }
}
