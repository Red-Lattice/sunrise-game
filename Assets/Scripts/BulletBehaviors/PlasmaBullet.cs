using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBullet : MonoBehaviour, ICapturable
{
    private const float bulletSpeed = 50f;
    private float bulletLifeTime = 3f;
    [SerializeField] private Vector3 bulletDirection;
    [SerializeField] private Rigidbody bulletRB;
    private bool initialized = false;
    private GameObject shooter;
    private bool captured;

    void Update()
    {
        if (captured) {return;}
        if (!initialized) {return;}

        transform.position += bulletDirection * Time.deltaTime * bulletSpeed;

        bulletLifeTime -= Time.deltaTime;
        if (bulletLifeTime < 0f) {DestroyBullet();}
    }

    void DestroyBullet() {
        this.gameObject.SetActive(false);
        initialized = false;
        bulletLifeTime = 3f;
    }

    public void OnCollisionEnter(Collision other)
    {
        GameObject otherGO = other.gameObject;
        // Guards
        if (!initialized) {return;}
        if (otherGO == shooter) {return;}
        if (otherGO.layer == shooter.layer) {return;}
        if (otherGO.tag == "Projectile") {return;}

        IDamageable damageableComponent;
        if (otherGO.TryGetComponent(out damageableComponent))
        {
            damageableComponent.DealDamage(30f, "Plasma_Pistol_Round", shooter, transform.position);
        }
        DestroyBullet();
    }

    public void OnTriggerEnter(Collider other)
    {
        GameObject otherGO = other.gameObject;
        // Guards
        if (!initialized) {return;}
        if (otherGO == shooter) {return;}
        if (otherGO.name == "BoundingBox") {return;}
        if (otherGO.layer == shooter.layer) {return;}
        if (otherGO.tag == "Projectile") {return;}

        IDamageable damageableComponent;
        if (otherGO.TryGetComponent(out damageableComponent))
        {
            damageableComponent.DealDamage(30f, "Plasma_Pistol_Round", shooter, transform.position);
        }
        DestroyBullet();
    }

    public void initialization(GameObject shooter)
    {
        this.shooter = shooter;
        initialized = true;
        bulletDirection = gameObject.transform.forward;
        captured = false;
    }

    public void Release(GameObject shooter)
    {
        this.shooter = shooter;
        captured = false;
    }
}
