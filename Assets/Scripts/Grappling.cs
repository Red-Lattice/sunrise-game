using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    //private playerMovementGrappling pm;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask physicsObjects;

    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;

    private Vector3 grapplePoint;

    [Header("Input")]
    [SerializeField] private bool grappling;

    void Start()
    {
        
    }

    void Update()
    {
        //if (Input.GetKeyDown(grappleKey)) StartGrapple();
    }

    private void StartGrapple()
    {
        /*if (grapplingCdTimer > 0) return;

        grappling = true;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }*/
    }

    private void ExecuteGrapple()
    {

    }

    private void StopGrapple()
    {
        //grappling = false;

        //grapplingCdTimer = grapplingCd;
    }
}
