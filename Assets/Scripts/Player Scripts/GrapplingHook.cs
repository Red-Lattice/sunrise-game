using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    /*private GameObject grapplingHook;
    private GameObject cam;
    private bool extending;
    private Vector3 grapplePoint;
    private bool grappled;
    private float totalFactor = 0;
    [SerializeField] private GameObject testSphere;

    void Awake()
    {
        grapplingHook = transform.gameObject;
        cam = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            grapplePoint = getGrapplePoint();
            testSphere.transform.position = grapplePoint;
            grappled = true;
        }
        extending = (Input.GetMouseButton(2));
        updateLen();
    }

    private Vector3 getGrapplePoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 50f))
        {
            return hit.point;
        }
        return cam.transform.position + cam.transform.forward * 50f;
    }

    void updateLen()
    {
        float factor = 20f * Time.deltaTime * (extending ? 1f : -1f);
        
    }*/

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private Rigidbody playerRB;

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

        if (joint) {joint.maxDistance -= Time.deltaTime * 25f;}
        if (joint && playerRB.velocity.magnitude > 10f)
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
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance)) {
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
