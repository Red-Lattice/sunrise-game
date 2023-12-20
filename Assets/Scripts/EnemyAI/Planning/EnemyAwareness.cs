using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{
    private const float awarenessRadius = 20f;
    private const float maxSightlineAngle = 60f;
    
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private LayerMask friendlyEntityLayerMask;
    [SerializeField] private LayerMask blockerLayerMask;
    [SerializeField] private Transform head;
    public List<Collider> unobstructedColliders;
    public List<Collider> directVisionConeColliders;
    public List<Collider> potentialTargets;

    void Start()
    {
        unobstructedColliders = new List<Collider>();
        directVisionConeColliders = new List<Collider>();
        enemyTransform = head;
    }

    /// <summary>
    /// Runs awareness checks to make sure the checks are done before brain calculations
    /// </summary>
    public void Tick()
    {
        checkAwareness();
    }

    void checkAwareness()
    {
        unobstructedColliders.Clear();
        directVisionConeColliders.Clear();
        potentialTargets.Clear();

        Vector3 center = head.position;
        Collider[] awareColliders = Physics.OverlapSphere(center, awarenessRadius, friendlyEntityLayerMask);

        foreach (Collider hitCol in awareColliders)
        {
            if (!Physics.Raycast(head.position, (hitCol.gameObject.transform.position - head.position), (hitCol.gameObject.transform.position - transform.position).magnitude, blockerLayerMask)) 
            {
                unobstructedColliders.Add(hitCol);
            }
        }
        foreach (Collider unobCol in unobstructedColliders)
        {
            if (Mathf.Abs(Vector3.Angle(head.forward, (unobCol.gameObject.transform.position - head.position))) < maxSightlineAngle)
            {
                directVisionConeColliders.Add(unobCol);
            }
        }
        foreach (Collider dvcCol in directVisionConeColliders)
        {
            if (dvcCol.gameObject.layer == LayerMask.NameToLayer("FriendlyEntity"))
            {
                potentialTargets.Add(dvcCol);
            }
        }
    }
}
