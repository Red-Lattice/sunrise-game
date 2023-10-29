using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    enum State
    {
        Idle,
        Wandering,
        Persuing,
        Attacking,
        //LKP = Last known position
        AttackingLKP,
        Dodging,
        BackingUp,
        Cautious
    }

    [Header("State")]
    [SerializeField] State state = State.Idle;

    [Header("Initialization")]
    [SerializeField] private EnemyMovement movementScript;
    [SerializeField] private AwarenessSystem awarenessScript;
    [SerializeField] private Weapon plasmaGun;

    private GameObject target;
    private Vector3 lastKnownTargetPosition;
    // The position that the enemy should attack
    private Vector3 attackPosition;
    private bool lookingAtTarget;
    private bool previouslySeenTarget;
    private float distanceFromTarget;
    private float randomChanceToSwitchState;
    private float reactionTime;
    // Marks how long the enemy will triggerWeapon at a corner before charging in at the target.
    private float chargeInTimer;

    // Start is called before the first frame update
    void Start()
    {
        awarenessScript = GetComponent<AwarenessSystem>();
        movementScript = GetComponent<EnemyMovement>();
        plasmaGun = GetComponentInChildren<Weapon>();
        randomChanceToSwitchState = 0f;
        reactionTime = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        reactionTime -= Time.deltaTime;
        if (reactionTime < 0)
        {
            awarenessScript.Tick();
            reactionTime = 0.3f;
        }
        target = awarenessScript.getTarget();

        checkTarget();

        randomSwitchStates();

        decideActionFromState();
    }

    void decideActionFromState()
    {
        switch (state)
        {
            case State.Attacking:
                if (target == null)
                {
                    state = State.Cautious;
                }
                movementScript.turnToTarget(target.transform.position);
                if (awarenessScript.canShootTarget())
                {
                    plasmaGun.triggerWeapon();
                    //if (distanceFromTarget < 10f) This script is for if I want the enemy to back up if the player is too close
                    //{
                        //transform.position += Vector3.back;
                    //}
                }
                break;
                
            case State.AttackingLKP:
                Vector3 dirToLastKnownPos = (lastKnownTargetPosition - plasmaGun.gameObject.transform.position);
                Vector3 plasmaGunPos = plasmaGun.gameObject.transform.position;

                if (!Physics.Raycast(plasmaGunPos, (dirToLastKnownPos), (dirToLastKnownPos).magnitude))
                {
                    movementScript.turnToTarget(lastKnownTargetPosition);
                    plasmaGun.triggerWeapon();
                    return;
                }

                Vector3 testVector1 = Quaternion.AngleAxis(-1, Vector3.up) * dirToLastKnownPos;
                if (!Physics.Raycast(plasmaGunPos, (testVector1), (testVector1).magnitude))
                {
                    attackPosition = testVector1;
                    movementScript.turnToTarget(testVector1);
                    plasmaGun.triggerWeapon();
                    return;
                }

                Vector3 testVector2 = Quaternion.AngleAxis(1, Vector3.up) * dirToLastKnownPos;
                if (!Physics.Raycast(plasmaGunPos, (testVector2), (testVector2).magnitude))
                {
                    attackPosition = testVector2;
                    movementScript.turnToTarget(testVector2);
                    plasmaGun.triggerWeapon();
                    return;
                }

                movementScript.turnToTarget(lastKnownTargetPosition);
                plasmaGun.triggerWeapon();
                break;

            case State.Persuing:
                if (awarenessScript.unobstructedFrom(target))
                {
                    movementScript.turnToTarget(target.transform.position);
                }
                break;
            default:
                break;
        }
    }

    void checkTarget()
    {
        if (target != null)
        {
            lookingAtTarget = awarenessScript.hasLineOfSight();

            if (awarenessScript.hasLineOfSight() && awarenessScript.unobstructedFrom(target))
            {
                state = State.Attacking;
                distanceFromTarget = (target.transform.position - transform.position).magnitude;
                
                if (awarenessScript.canShootTarget())
                {
                    lastKnownTargetPosition = target.transform.position;
                }
                return;
            }

            if (!awarenessScript.unobstructedFrom(target) && state != State.Persuing)
            {
                chargeInTimer = (state == State.AttackingLKP) ? chargeInTimer : Random.Range(4, 6);
                state = State.AttackingLKP;
            }

            distanceFromTarget = (target.transform.position - transform.position).magnitude;
            lastKnownTargetPosition = target.transform.position;
        }
    }

    // For certain states I want the enemy to occasionally switch between them to add variety. 
    // This method handles that
    void randomSwitchStates()
    {
        float randomGeneratedNumber = Random.Range(0, 100) / 100;
        /*if (state == State.Idle)
        {
            randomChanceToSwitchState += Time.deltaTime / 10f;
            state = (randomChanceToSwitchState < randomGeneratedNumber) ? state : State.Wandering;
            return;
        }*/

        if (state == State.AttackingLKP)
        {
            chargeInTimer -= Time.deltaTime;
            if (chargeInTimer < 0)
            {
                state = State.Persuing;
                movementScript.goToTarget();
            }
        }

        randomChanceToSwitchState = (state == State.Wandering || state == State.Dodging) ? 0f : randomChanceToSwitchState;
    }
}
