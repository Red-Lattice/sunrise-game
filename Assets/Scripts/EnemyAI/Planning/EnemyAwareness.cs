using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{
    private const float awarenessRadius = 20f;
    private const float maxSightlineAngle = 60f;
    
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private LayerMask entityLayers;
    [SerializeField] private LayerMask blockerLayers;
    [SerializeField] private Transform head;
    public List<Collider> unobstructedColliders;
    public List<Collider> directVisionConeColliders;
    public List<Collider> potentialTargets;
    public List<Collider> smartObjects;

    void Start()
    {
        unobstructedColliders = new List<Collider>();
        directVisionConeColliders = new List<Collider>();
        smartObjects = new List<Collider>();
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
        smartObjects.Clear();

        Vector3 center = head.position;
        Collider[] awareColliders = Physics.OverlapSphere(center, awarenessRadius, entityLayers);

        foreach (Collider hitCol in awareColliders)
        {
            Vector3 dir = hitCol.gameObject.transform.position - head.position;
            if (!Physics.Raycast(head.position, dir, dir.magnitude, blockerLayers)) 
            {
                unobstructedColliders.Add(hitCol);
            }
        }
        foreach (Collider unobCol in unobstructedColliders)
        {
            GameObject go = unobCol.gameObject;
            if (Mathf.Abs(Vector3.Angle(head.forward, go.transform.position - head.position)) < maxSightlineAngle)
            {
                directVisionConeColliders.Add(unobCol);
            }
            if (unobCol.gameObject.layer == LayerMask.NameToLayer("objects"))
            {
                smartObjects.Add(unobCol);
            }
        }
        foreach (Collider dvcCol in directVisionConeColliders)
        {
            if (dvcCol.gameObject.layer == LayerMask.NameToLayer("FriendlyEntity"))
            {
                potentialTargets.Add(dvcCol);
                continue;
            }
        }
    }
}
