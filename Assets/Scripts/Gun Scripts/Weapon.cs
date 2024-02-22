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

public static class Weapon
{
    public static readonly float[] cooldowns = {0f, 0.2f, 0.2f};
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
                Debug.LogError("Case not handled: " + weaponInfo.bulletType.ToString());
                break;
        }
        // Cooldown is YOUR responsibility to handle. The static class doesn't like it :(
    }

    public static bool Fireable(WeaponStruct weaponInfo) {
        return weaponInfo.cooldown <= 0f && weaponInfo.ammo > 0;
    }

    public static void FireHitscan(GameObject firer, WeaponStruct weaponInfo, Transform firePosition) {
        LayerMask layer = BulletSingleton.shootableLayers & ~(1 << firer.layer);

        if (Physics.Raycast(firePosition.position, firePosition.forward, out RaycastHit hit, 100f, layer))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageableComponent))
            {
                damageableComponent.DealDamage(weaponInfo.damage, weaponInfo.bulletType, firer, hit.transform.position);
            }
        }
    }

    public static void FireRangedHitscan(GameObject firer, WeaponStruct weaponInfo, Transform firePosition) {
        LayerMask layer = BulletSingleton.shootableLayers & (~firer.layer);
        if (Physics.Raycast(firePosition.position, firePosition.forward, out RaycastHit hit, weaponInfo.range, layer))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageableComponent))
            {
                damageableComponent.DealDamage(weaponInfo.damage, weaponInfo.bulletType, firer, hit.transform.position);
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
    public readonly static WeaponStructTemplate[] WeaponStructs = {new WeaponStructTemplate {gunType = None},
        new WeaponStructTemplate {gunType = Pistol, bulletType = Kinetic_Small, range = float.PositiveInfinity, damage = 30f,},
        new WeaponStructTemplate {gunType = PlasmaPulser, bulletType = Plasma_Pistol_Round, range = float.PositiveInfinity, damage = 30f,}
    };

    public static WeaponStruct BuildWeaponStruct(GunType gunType, ushort ammo, ushort reserveAmmo) {
        WeaponStructTemplate template = WeaponStructs[(int)gunType];
        return new WeaponStruct {
            gunType = template.gunType,
            bulletType = template.bulletType, 
            range = template.range, 
            damage = template.damage,
            cooldown = 0f, 
            ammo = ammo,
            reserveAmmo = reserveAmmo,
        };
    }

    public static WeaponStruct WeaponStructFromName(string weaponName) {
        switch (weaponName) {
            case "Pistol":
                return BuildWeaponStruct(Pistol, 100, 100);
            case "Plasma Pulser":
                return BuildWeaponStruct(PlasmaPulser, 100, 100);
            default:
                throw new Exception("Weapon Struct not associated with weapon name: " + weaponName);
        }
    }

    public static bool NotNull(WeaponStruct weapon) {
        return weapon.gunType != None;
    }
}


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