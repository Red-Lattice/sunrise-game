using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifeTime = 10f;
    [SerializeField] private Vector3 bulletDirection;
    [SerializeField] private Rigidbody bulletRB;
    // Update is called once per frame

    void Start()
    {
        bulletSpeed = 10f;
        bulletLifeTime = 10f;
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
        if (other.gameObject.tag != "Projectile" && other.gameObject.tag != "Gun" && other.gameObject.tag != "Debug")
        {
            Destroy(this.gameObject);
        }
    }
}
