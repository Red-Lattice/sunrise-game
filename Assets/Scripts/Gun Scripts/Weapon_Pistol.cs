using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon_Pistol : Weapon
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask shootableLayers;
    private float cooldown;
    private GameObject shooter;
    
    void Start()
    {
        cooldown = 0f;
        cam = this.transform.root.GetComponentInChildren<Camera>();
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
            IDamageable damageableComponent; 
            if (hit.transform.TryGetComponent(out damageableComponent))
            {
                damageableComponent.DealDamage(30f, "Kinetic_Small", shooter, hit.transform.position);
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

    public override Weapon SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
        return this;
    }
}
