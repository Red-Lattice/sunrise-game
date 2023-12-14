using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float awarenessRadius;
    [SerializeField] private float maxSightlineAngle;
    
    [Header("Debug")]
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private LayerMask friendlyEntityLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    private List<Collider> unobstructedColliders;
    public List<Collider> directVisionConeColliders {get; private set;}

    void Start()
    {
        enemyTransform = this.GetComponent<Transform>();
        awarenessRadius = 20f;
        maxSightlineAngle = 45f;
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

        Vector3 center = transform.position;
        Collider[] awareColliders = Physics.OverlapSphere(center, awarenessRadius, friendlyEntityLayerMask);

        foreach (Collider hitCol in awareColliders)
        {
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
}
