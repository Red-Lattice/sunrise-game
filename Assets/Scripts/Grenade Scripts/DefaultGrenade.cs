using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultGrenade : MonoBehaviour, Interface_Grenade
{
    void Start()
    {
        StartCoroutine(WaitToExplode());
    }

    public void Explode()
    {
        Collider[] exploded = Physics.OverlapSphere(transform.position, 5f);

        foreach (Collider hitCol in exploded)
        {
            if (hitCol.transform.gameObject == transform.gameObject) {continue;}
            Rigidbody entityRB;
            Vector3 dir = (transform.position - hitCol.transform.position);
            if (hitCol.TryGetComponent<Rigidbody>(out entityRB))
            {
                if (dir.magnitude == 0f)
                {
                    Debug.LogError("Divide by 0 error!");
                }
                else
                {
                    entityRB.AddForce(-dir * (5 / dir.magnitude), ForceMode.VelocityChange);
                }
            }
            StatManager entityStats;
            if (hitCol.TryGetComponent(out entityStats))
            {
                if (dir.magnitude == 0f)
                {
                    Debug.LogError("Divide by 0 error!");
                }
                else
                {
                    entityStats.DealDamage((100 / dir.magnitude) + 30, "Grenade", null);
                }
            }
        }
        Destroy(transform.gameObject);
    }

    private IEnumerator WaitToExplode()
    {
        yield return new WaitForSeconds(2.5f);
        Explode();
    }
}
