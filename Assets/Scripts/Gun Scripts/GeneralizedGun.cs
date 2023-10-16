using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralizedGun : MonoBehaviour
{
    enum Mode
    {
        Pistol
    }
    [SerializeField] Mode mode = Mode.Pistol;
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask shootableLayers;

    void Start()
    {
        gunAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (mode)
            {
                case Mode.Pistol:
                    gunAnimator.SetTrigger("FireGun");
                    FirePistol();
                    break;
            }
        }
    }

    private void FirePistol()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, shootableLayers))
        {
            Debug.Log(hit.transform.name);
            TestEnemyStatManager hitEnemy = hit.transform.GetComponent<TestEnemyStatManager>();
            if (hitEnemy != null)
            {
                hitEnemy.DealDamage(30f);
            }
        }

    }
}