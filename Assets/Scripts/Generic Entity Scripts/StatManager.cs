using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxShield;
    private ShieldAnimator shieldAnimator;
    private DeathManager deathManager;
    [SerializeField] private DeadEntitiesScriptableObject deso;
    private float health;
    private float shield;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        shield = maxShield;
        shieldAnimator = transform.GetComponentInChildren<ShieldAnimator>(false);
        string name = transform.gameObject.name;
        deathManager = new DeathManager(name, deso, this.transform.gameObject);
    }

    public void dealDamage(float damage, string damageType)
    {
        if (shield > 0)
        {
            shield -= (damageType == "Plasma") ? damage * 3 : damage;
            if (shieldAnimator != null)
            {
                shieldAnimator.ShieldFlash();
            }
        }
        else
        {
            health -= (damageType == "Physical") ? damage * 5 : damage;
        }
        if (health < 0f)
        {
            deathManager.kill();
        }
    }
}
