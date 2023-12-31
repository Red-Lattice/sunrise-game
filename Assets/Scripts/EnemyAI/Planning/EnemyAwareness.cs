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
    public List<GameObject> unobstructedColliders;
    public List<GameObject> directVisionConeColliders;
    public List<GameObject> potentialTargets;
    public List<GameObject> smartObjects;

    void Start()
    {
        unobstructedColliders = new List<GameObject>();
        directVisionConeColliders = new List<GameObject>();
        smartObjects = new List<GameObject>();
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
                unobstructedColliders.Add(hitCol.gameObject);
            }
        }
        foreach (GameObject entity in unobstructedColliders)
        {
            if (Mathf.Abs(Vector3.Angle(head.forward, entity.transform.position - head.position)) < maxSightlineAngle)
            {
                directVisionConeColliders.Add(entity);
            }
            if (entity.gameObject.layer == LayerMask.NameToLayer("objects"))
            {
                smartObjects.Add(entity);
            }
        }
        foreach (GameObject entity in directVisionConeColliders)
        {
            if (entity.layer == LayerMask.NameToLayer("FriendlyEntity"))
            {
                potentialTargets.Add(entity);
                continue;
            }
        }
    }
}
