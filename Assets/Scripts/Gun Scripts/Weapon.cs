using System;
using UnityEngine;
using static BulletType;
using static GunType;
/* public enum BulletType {
    Melee,
    Kinetic_Small,
    Plasma_Pistol_Round,
    Grenade,
}*/
public struct WeaponStructTemplate {
    public GunType gunType;
    public BulletType bulletType;
    public float range;
    public float damage;
}

public struct WeaponStruct {
    public GunType gunType;
    public BulletType bulletType;
    public float range;
    public float damage;
    public float cooldown;
    public ushort ammo;
    public ushort reserveAmmo;
}

public enum GunType {
    None,
    Pistol,
    PlasmaPulser,
}


public static class Weapon
{
    // METHODS
    public static void Fire(GameObject firer, WeaponStruct weaponInfo, Transform firePosition) {
        switch (weaponInfo.bulletType) {
            case Melee:
                FireRangedHitscan(firer, weaponInfo, firePosition);
                break;
            case Kinetic_Small:
                FireHitscan(firer, weaponInfo, firePosition);
                break;
            case Plasma_Pistol_Round:
                FireProjectile(firer, weaponInfo, firePosition);
                break;
            default:
                Debug.LogError("Case not handled: " + weaponInfo.bulletType);
                break;
        }
        // Cooldown is YOUR responsibility to handle. The static class doesn't like it :(
    }

    public static void FireHitscan(GameObject firer, WeaponStruct weaponInfo, Transform firePosition) {
        LayerMask layer = BulletSingleton.shootableLayers & ~(1 << firer.layer);
        RaycastHit hit;
        if (Physics.Raycast(firePosition.position, firePosition.forward, out hit, 100f, layer))
        {
            IDamageable damageableComponent;
            if (hit.transform.TryGetComponent(out damageableComponent))
            {
                damageableComponent.DealDamage(
                    weaponInfo.damage, 
                    weaponInfo.bulletType, 
                    firer, 
                    hit.transform.position);
            }
        }
    }

    public static void FireRangedHitscan(GameObject firer, WeaponStruct weaponInfo, Transform firePosition) {
        LayerMask layer = BulletSingleton.shootableLayers & (~firer.layer);
        RaycastHit hit;
        if (Physics.Raycast(firePosition.position, firePosition.forward, out hit, weaponInfo.range, layer))
        {
            IDamageable damageableComponent;
            if (hit.transform.TryGetComponent(out damageableComponent))
            {
                damageableComponent.DealDamage(
                    weaponInfo.damage, 
                    weaponInfo.bulletType, 
                    firer, 
                    hit.transform.position);
            }
        }
    }

    public static void FireProjectile(GameObject firer, WeaponStruct weaponInfo, Transform firePosition) {
        GameObject bullet = BulletSingleton.instance.GetBullet(weaponInfo.bulletType);
        IInitializable initializableScript = bullet.GetComponent<IInitializable>();

        bullet.transform.position = firePosition.position;
        bullet.transform.rotation = firePosition.rotation;

        initializableScript.Initialize(firer);
    }
    
    public static WeaponStruct BuildWeaponStruct(GunType gunType, ushort ammo, ushort reserveAmmo) {
        if (ScriptableObjectHoarder.instance == null) {return new WeaponStruct{};}
        SO_WeaponTemplate template = WeaponTemplate(gunType);
        SO_Bullet bullet = Bullet(template.bulletType);
        return new WeaponStruct {
            gunType = template.gunType,
            bulletType = template.bulletType, 
            range = template.range, 
            damage = bullet.damage,
            cooldown = 0f, 
            ammo = ammo,
            reserveAmmo = reserveAmmo,
        };
    }

    public static WeaponStruct WeaponStructFromName(string weaponName) {
        if (ScriptableObjectHoarder.instance == null) {return new WeaponStruct{};}
        switch (weaponName) {
            case "Pistol":
                return BuildWeaponStruct(Pistol, 100, 100);
            case "Plasma Pulser":
                return BuildWeaponStruct(PlasmaPulser, 100, 100);
            default:
                throw new Exception("Weapon Struct not associated with weapon name: " + weaponName);
        }
    }

    public static SO_WeaponTemplate WeaponTemplate(GunType gunType) {
        return ScriptableObjectHoarder.instance.WeaponTemplates[(int)gunType];
    }

    public static SO_Bullet Bullet(BulletType bulletType) {
        return ScriptableObjectHoarder.instance.Bullets[(int)bulletType];
    }

    public static bool NotNull(WeaponStruct weapon) {return weapon.gunType != None;}
    public static bool NotNull(ref WeaponStruct weapon) {return weapon.gunType != None;}
    public static unsafe bool NotNull(WeaponStruct* weapon) {return (*weapon).gunType != None;}
}