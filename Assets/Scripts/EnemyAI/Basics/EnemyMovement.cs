using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private AwarenessSystem enemyAwareness;
    private GameObject target;
    [SerializeField] private float attackRange;
    [SerializeField] private float turnSpeed;
    [SerializeField] private bool isChasing;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody enemyRB;
    [SerializeField] private Weapon plasmaGun;
    private float defaultSpeed;

    private Coroutine moveCoroutine;
    private Coroutine lookCoroutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); 
        enemyAwareness = GetComponent<AwarenessSystem>();
        enemyRB = GetComponent<Rigidbody>();

        defaultSpeed = agent.speed;

        attackRange = 1f;
    }

    void Update()
    {
        //enemyAwareness.Tick();
        target = enemyAwareness.getTarget();

        if (target != null)
        {
            if (!isChasing)
            {
                if (!enemyAwareness.unobstructedFrom(target))
                {
                    //isChasing = true;
                    //moveCoroutine = StartCoroutine(moveToTarget());
                }
                else
                {
                    /*Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                    lookRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 10 * Time.fixedDeltaTime);*/
                }
            }
            if (enemyAwareness.hasLineOfSight())
            {
                plasmaGun.triggerWeapon();
            }
        }
    }

    public void goToTarget()
    {
        isChasing = true;
        moveCoroutine = StartCoroutine(moveToTarget());
    }

    private IEnumerator moveToTarget() 
    {
        while (target != null && Vector3.Distance(transform.position, target.transform.position) > attackRange && !enemyAwareness.unobstructedFrom(target)) 
        {
            agent.SetDestination(target.transform.position);
            yield return null;
        }
        isChasing = false;
        agent.ResetPath();
    }

    public void turnToTarget(Vector3 targetPos)
    {
        Quaternion lookRotation = Quaternion.LookRotation(targetPos - transform.position);
        lookRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.fixedDeltaTime);
    }

    public void moveBackwards()
    {
        //transform.position += Vector3.back;
    }
}

