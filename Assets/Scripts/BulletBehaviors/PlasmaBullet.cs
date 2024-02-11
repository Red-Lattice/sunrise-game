using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletSingleton;

public class PlasmaBullet : MonoBehaviour, ICapturable
{
    private const float bulletSpeed = 50f;
    private float bulletLifeTime = 3f;
    private Vector3 bulletDirection;
    private bool initialized = false;
    public GameObject shooter;
    private float factor;

    void FixedUpdate()
    {
        if (!initialized) {return;}
        
        transform.position += bulletDirection * factor;
        bulletLifeTime -= Time.fixedDeltaTime;

        if (AttemptAttack() || bulletLifeTime < 0f) {DestroyBullet();}
    }

    void DestroyBullet() {
        this.gameObject.SetActive(false);
        initialized = false;
        bulletLifeTime = 3f;
    }

    private bool AttemptAttack() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, bulletDirection, out hit, factor, shootableLayers))
        {
            Debug.Log("Detected: " + hit.transform.gameObject);
            if (!Guards(hit.transform)) {return false;}
            IDamageable damageable; 
            if (hit.transform.TryGetComponent(out damageable))
            {
                damageable.DealDamage(30f, "Plasma_Pistol_Round", shooter, hit.point);
            }
            return true;
        }
        return false;
    }

    private bool Guards(Transform hitObject) {
        return ((hitObject.gameObject.layer & shooter.layer) > 0); // Return if they're on different layers
    }

    public void initialization(GameObject shooter)
    {
        factor = Time.fixedDeltaTime * bulletSpeed;
        this.shooter = shooter;
        initialized = true;
        bulletDirection = gameObject.transform.forward;
    }

    public void Release(GameObject shooter)
    {
        this.shooter = shooter;
    }
}
