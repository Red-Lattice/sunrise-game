using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static DamageType;

public class PlayerStatManager : MonoBehaviour, IDamageable
{
    private const float maxHealth = 100f;
    private const float maxShield = 100f;
    [SerializeField] private DeadEntitiesScriptableObject deso;
    [SerializeField] private Animator OptUIFlash;
    [SerializeField] private WarpWall wall;
    [SerializeField] private Transform camTransform;
    private DeathManager deathManager;
    private float health;
    private float shield;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        shield = maxShield;
        string name = transform.gameObject.name;
        deathManager = new DeathManager(name, deso, this.transform.gameObject);
    }

    public void DealDamage(float damage, string bulletType, GameObject dealer, Vector3 hitPos)
    {
        DamageType damageType = Damage.bulletToDamageType(bulletType);
        if (wall.capturing && CaptureCheck(damageType, hitPos)) {
            wall.AddBullet(bulletType);
            return;
        }
        if (shield > 0)
        {
            damageShield(damage, damageType);
            if (OptUIFlash != null)
            {
                OptUIFlash.Play("UIShieldFlash");
            }
        }
        else
        {
            health -= (damageType == Kinetic) ? damage * 5 : damage;
            if (OptUIFlash != null)
            {
                OptUIFlash.Play("UIHurtFlash");
            }
        }
        if (health < 0f)
        {
            deathManager.kill();
            if (OptUIFlash != null)
            {
                OptUIFlash.Play("UIHurtFlash");
            }
        }
    }

    private bool CaptureCheck(DamageType damageType, Vector3 position)
    {
        if (damageType == Physical || damageType == Explosion) {return false;}
        Debug.Log(Vector3.Angle(camTransform.InverseTransformPoint(position), camTransform.forward));
        return (Mathf.Abs(Vector3.Angle(camTransform.InverseTransformPoint(position), camTransform.forward)) < 90f);
    }

    void damageShield(float damage, DamageType damageType)
    {
        shield -= (damageType == Energy) ? damage * 2 : damage;
    }
}
