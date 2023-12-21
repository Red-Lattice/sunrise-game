using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public Transform gunTip, playerCamera, player;
    private float maxDistance;
    private SpringJoint joint;
    private Rigidbody playerRB;

    void Awake() {
        lr = GetComponent<LineRenderer>();
        maxDistance = 75f;
        playerRB = transform.parent.parent.GetComponent<Rigidbody>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(2)) {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(2)) {
            StopGrapple();
        }

        if (joint) {joint.maxDistance -= Time.deltaTime * 25f;}
        if (joint && playerRB.velocity.magnitude > 20f)
        {
            playerRB.AddForce(-playerRB.velocity * Time.deltaTime * 5f, ForceMode.VelocityChange);
        }
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance)) {
            Debug.Log(hit.transform.gameObject.name);
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = 0;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    void StopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
