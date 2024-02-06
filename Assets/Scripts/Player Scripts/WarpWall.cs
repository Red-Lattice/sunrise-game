using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpWall : MonoBehaviour, IDamageable
{
    private bool capturing = false;
    private BoxCollider wallCollider;
    private MeshRenderer wallMesh;
    private List<string> arr;

    void Awake()
    {
        wallCollider = GetComponent<BoxCollider>();
        wallMesh = GetComponent<MeshRenderer>();
        wallCollider.enabled = false;
        wallMesh.enabled = false;
        arr = new List<string>();
    }

    void Update()
    {
        capturing = Input.GetMouseButton(1);

        if (Input.GetMouseButtonDown(1))
        {
            wallCollider.enabled = true;
            wallMesh.enabled = true;
            return;
        }
        if (Input.GetMouseButtonUp(1))
        {
            wallCollider.enabled = false;
            wallMesh.enabled = false;
        }
    }

    public void DealDamage(float damage, string bulletType, GameObject dealer)
    {
        arr.Add(bulletType);
    }

    private void Release()
    {
        foreach (string bullet in arr)
        {
            
        }
        arr.Clear();
    }

    private static void Fire(Transform firer)
    {
    }
}