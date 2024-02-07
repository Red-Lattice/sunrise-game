using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpWall : MonoBehaviour
{
    public bool capturing = false;
    private MeshRenderer wallMesh;
    private List<string> arr;

    void Awake()
    {
        wallMesh = GetComponent<MeshRenderer>();
        wallMesh.enabled = false;
        arr = new List<string>();
    }

    void Update()
    {
        capturing = Input.GetMouseButton(1);

        if (Input.GetMouseButtonDown(1))
        {
            wallMesh.enabled = true;
            capturing = true;
            return;
        }
        if (Input.GetMouseButtonUp(1))
        {
            wallMesh.enabled = false;
            capturing = false;
        }
    }

    public void AddBullet()
    {

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