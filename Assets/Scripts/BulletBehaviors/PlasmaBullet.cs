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
        if (!initialized) {return;}
        GameObject otherGO = other.gameObject;
        string otherTag = otherGO.tag;

        //Returning before the GetComponent call if it's the shooter
        if (otherGO.transform.root.name == shooter) {return;}
        if (otherTag == "Projectile") {return;}
        StatManager otherStatManager = otherGO.GetComponent<StatManager>();
        if (otherStatManager != null)
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
