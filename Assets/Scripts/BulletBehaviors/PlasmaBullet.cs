using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifeTime = 10f;
    [SerializeField] private Vector3 bulletDirection;
    [SerializeField] private Rigidbody bulletRB;
    private bool initialized = false;
    private string shooter;
    // Update is called once per frame

    void Start()
    {
        bulletSpeed = 50f;
        bulletLifeTime = 4f;
        bulletDirection = gameObject.transform.forward;
    }
    void Update()
    {
        transform.position += (bulletDirection * Time.deltaTime * bulletSpeed);

        bulletLifeTime -= Time.deltaTime;
        if (bulletLifeTime < 0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        GameObject otherGO = other.gameObject;
        
        // Guards
        if (!initialized) {return;}
        if (otherGO.transform.root.name == shooter) {return;}
        if (otherGO.tag == "Projectile") {return;}

        StatManager otherStatManager;
        if (otherGO.TryGetComponent<StatManager>(out otherStatManager))
        {
            otherStatManager.dealDamage(30f, "Energy");
        }
        Destroy(this.gameObject);
    }

    public void initialization(string shooter)
    {
        this.shooter = shooter;
        initialized = true;
    }
}
