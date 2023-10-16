using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessSystem : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float awarenessRadius;
    [SerializeField] private float maxSightlineAngle;
    
    [Header("Debug")]
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private LayerMask friendlyEntityLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    [SerializeField] private List<Collider> knowledgeColliders;
    [SerializeField] private List<Collider> unobstructedColliders;
    [SerializeField] private List<Collider> directVisionConeColliders;
    [SerializeField] private GameObject currentTarget;
    private Collider currentTargetCollider;

    void Start()
    {
        enemyTransform = this.GetComponent<Transform>();
        awarenessRadius = 20f;
        maxSightlineAngle = 45f;
        currentTarget = null;
    }

    public void Tick()
    {
        checkAwareness();
        pickTarget();
    }

    void checkAwareness()
    {
        knowledgeColliders.Clear();
        unobstructedColliders.Clear();
        directVisionConeColliders.Clear();

        Vector3 center = transform.position;
        Collider[] awareColliders = Physics.OverlapSphere(center, awarenessRadius, friendlyEntityLayerMask);

        foreach (Collider hitCol in awareColliders)
        {
            knowledgeColliders.Add(hitCol);
            if (!Physics.Raycast(transform.position, (hitCol.gameObject.transform.position - transform.position), (hitCol.gameObject.transform.position - transform.position).magnitude, defaultLayerMask)) 
            {
                unobstructedColliders.Add(hitCol);
            }
        }
        foreach (Collider unobCol in unobstructedColliders)
        {
            if (Mathf.Abs(Vector3.Angle(transform.forward, (unobCol.gameObject.transform.position - transform.position))) < maxSightlineAngle)
            {
                directVisionConeColliders.Add(unobCol);
            }
        }
    }

    void pickTarget()
    {
        if (currentTarget == null)
        {
            if (directVisionConeColliders.Count > 0)
            {
                int randomInt = Random.Range(0, directVisionConeColliders.Count);
                currentTarget = directVisionConeColliders[randomInt].gameObject;
                currentTargetCollider = directVisionConeColliders[randomInt];
                return;
            }
            return;
        }
        if (!knowledgeColliders.Contains(currentTargetCollider))
        {
            if (directVisionConeColliders.Count > 0)
            {
                int randomInt = Random.Range(0, directVisionConeColliders.Count);
                currentTarget = directVisionConeColliders[randomInt].gameObject;
                currentTargetCollider = directVisionConeColliders[randomInt];
                return;
            }
            else
            {
                currentTarget = null;
            }
        }
    }

    public GameObject getTarget()
    {
        return currentTarget;
    }

    public bool unobstructedFrom(GameObject lineTarget)
    {
        return (unobstructedColliders.Contains(currentTargetCollider));
    }

    public bool hasLineOfSight()
    {
        return directVisionConeColliders.Contains(currentTargetCollider);
    }

    public bool canShootTarget()
    {
        return (!Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.forward)), (currentTarget.transform.position - transform.position).magnitude));
    }

    public bool aware()
    {
        return true;
    }
}
