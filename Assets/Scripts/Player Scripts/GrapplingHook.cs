using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public Transform gunTip, playerCamera, player;
    private const float maxDistance = 75f;
    private Rigidbody playerRB;
    private bool grappling;
    private const float grappleSpeed = 15f;
    [SerializeField] private LepPlayerMovement playerMovement;
    [SerializeField] private LayerMask lm;

    void Awake() {
        lr = GetComponent<LineRenderer>();
        playerRB = transform.parent.parent.GetComponent<Rigidbody>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(2)) {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(2)) {
            StopGrapple();
        }
        if (grappling)
        {
            playerRB.velocity = (grapplePoint - playerRB.transform.position).normalized * grappleSpeed;
        }
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, lm)) {
            playerMovement.GrapplingStart();
            grappling = true;
            grapplePoint = hit.point;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    void StopGrapple() {
        lr.positionCount = 0;
        grappling = false;
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!grappling) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }
}
