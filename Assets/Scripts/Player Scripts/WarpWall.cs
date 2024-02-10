using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpWall : MonoBehaviour, IDamageable
{
    public bool capturing = false;
    private MeshRenderer wallMesh;
    private List<string> arr;
    [SerializeField] private ProjectileScriptableObjects projectiles;

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
            Release();
        }
    }

    public void DealDamage(float damage, string bulletType, GameObject dealer, Vector3 hitPos) 
    {
        AddBullet(bulletType);
    }

    public void AddBullet(string bulletName)
    {
        arr.Add(bulletName);
    }

    private void Release()
    {
        foreach (string bullet in arr)
        {
            FireBullet(transform, bullet, projectiles);
        }
        arr.Clear();
    }

    private static void FireBullet(Transform firer, string bulletType, ProjectileScriptableObjects pso)
    {
        switch (bulletType)
        {
            case "Plasma_Pistol_Round":
                float angle = Random.Range(-180f, 180f);
                float distance = Random.Range(0f, 1.25f);
                var location = (Quaternion.Euler(0, angle, 0) * firer.right * distance);
                Instantiate(pso.plasmaBall, firer.position + location, firer.parent.rotation).GetComponent<PlasmaBullet>().initialization(firer.parent.parent.gameObject);
                return;
            default:
                break;
        }
    }
}