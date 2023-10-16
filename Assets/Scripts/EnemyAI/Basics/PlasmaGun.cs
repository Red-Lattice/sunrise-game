using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaGun : MonoBehaviour
{
    [SerializeField] private GameObject myPrefab;
    [SerializeField] private Transform spawnPoint;
    public bool firing;
    private float firingTimer;
    [SerializeField] private float maxFiringTimer;

    void Update()
    {
        if (firingTimer >= 0f)
        {
            firingTimer -= Time.deltaTime;
        }

        //For debugging purposes only
        if (firing)
        {
            Fire(Quaternion.identity);
            firing = false;
        }
    }
    public void Fire(Quaternion direction)
    {
        if (firingTimer <= 0)
        {
            firingTimer = maxFiringTimer;
            // Instantiate at position (0, 0, 0) and zero rotation.
            Instantiate(myPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
