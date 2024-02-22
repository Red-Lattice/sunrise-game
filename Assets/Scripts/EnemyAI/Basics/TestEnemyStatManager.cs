/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyStatManager : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    [SerializeField] private float maxShield;
    [SerializeField] private float shield;
    [SerializeField] private Animator shieldAnimator;
    [SerializeField] private GameObject deadEnemyPrefab;
    [SerializeField] private Transform enemyTransform;
    private StateMachine stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100f;
        health = maxHealth;
        maxShield = 100f;
        shield = maxShield;
        stateMachine = GetComponent<StateMachine>();
    }

    public void DealDamage(float damage)
    {
        if (shield > 0)
        {
            shield -= damage;
            shieldAnimator.SetTrigger("ShieldFlash");
        }
        else
        {
            health -= damage;
        }
        if (health < 0f)
        {
            Instantiate(deadEnemyPrefab, enemyTransform.position, Quaternion.identity);
            this.gameObject.SetActive(false);
        }
    }

    void kill()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}*/
