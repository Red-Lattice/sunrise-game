using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static DamageType;

public class StatManager : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxShield;
    [SerializeField] private DeadEntitiesScriptableObject deso;
    [SerializeField] private Animator OptUIFlash;
    private ShieldAnimator shieldAnimator;
    private DeathManager deathManager;
    private float health;
    private float shield;
    private EnemyBrain optionalBrain;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        shield = maxShield;
        shieldAnimator = transform.GetComponentInChildren<ShieldAnimator>(false);
        string name = transform.gameObject.name;
        deathManager = new DeathManager(name, deso, this.transform.gameObject);
        optionalBrain = GetComponent<EnemyBrain>();
    }

    public void DealDamage(float damage, string bulletType, GameObject dealer)
    {
        DamageType damageType = Damage.bulletToDamageType(bulletType);
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
        if (optionalBrain != null)
        {
            optionalBrain.InformOfDamage(dealer, (damageType == Kinetic) ? damage * 5 : damage);
        }
    }

    void damageShield(float damage, DamageType damageType)
    {
        shield -= (damageType == Energy) ? damage * 2 : damage;

        if (shieldAnimator == null) {return;}

        if (shield > 0f)
        {
            shieldAnimator.ShieldFlash();
        }
        else
        {
            shieldAnimator.ShieldBreak();
        }
    }
}
