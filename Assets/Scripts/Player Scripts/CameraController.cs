using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Initialization")]
    public Camera mainCamera;
    public Camera weaponCamera;
    [SerializeField] Rigidbody rb;

    [Header("Values")]
    [SerializeField] private float sensX = 1f;
    [SerializeField] private float sensY = 1f;
    [SerializeField] private float baseFov = 90f;
    [SerializeField] private float maxFov = 140f;
    [SerializeField] private float wallRunTilt = 15f;

    [SerializeField] private float wishTilt = 0;
    [SerializeField] private float curTilt = 0;
    [SerializeField] private float interpValue = 0.1f;
    [SerializeField] Vector2 currentLook;
    Vector2 sway = Vector3.zero;
    float fov;

    public GameObject marker;
    public GameObject pt1;
    public GameObject pt2;
    public Vector3 point => lockedTarget.position;
    public bool locked {get; private set;}

    public Transform lockedTarget;
    

    void Start()
    {
        curTilt = transform.localEulerAngles.z;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {locked = !locked;
        if (!locked) {
            Debug.Log("Uh");
            transform.LookAt(lockedTarget);
        }}
        GetLockTarget();
        RotateMainCamera();
    }

    private LayerMask targets = 1 << 9;
    void GetLockTarget() {
        RaycastHit hit;
        Vector3 p1 = transform.position;

        pt1.transform.position = p1;

        // Cast character controller shape 10 meters forward to see if it is about to hit anything.
        if (Physics.CapsuleCast(p1, p1, 1f, transform.forward, out hit, 25f, targets)) {
            marker.transform.position = hit.transform.position;
            //point = hit.transform.position;
            lockedTarget = hit.transform;
        }
            
    }

    void FixedUpdate()
    {
        float addedFov = rb.velocity.magnitude - 3.44f;
        fov = Mathf.Lerp(fov, baseFov + addedFov, 0.5f);
        fov = Mathf.Clamp(fov, baseFov, maxFov);
        mainCamera.fieldOfView = fov;
        weaponCamera.fieldOfView = fov;

        currentLook = Vector2.Lerp(currentLook, currentLook + sway, 0.8f);
        curTilt = Mathf.LerpAngle(curTilt, wishTilt * wallRunTilt, interpValue);

        sway = Vector2.Lerp(sway, Vector2.zero, 0.2f);
    }
    private Vector3 toLockPoint => point - transform.position;
    [SerializeField] private float spd;
    private Quaternion lookTo => Quaternion.LookRotation(toLockPoint);
    private Quaternion parentRot => transform.parent.rotation;
    private Quaternion flatLookTo => Quaternion.LookRotation(new Vector3(toLockPoint.x, 0, toLockPoint.z));
    
    void RotateMainCamera()
    {
        if (locked) {
            //var Rot = Quaternion.LookRotation(point);
            //transform.localRotation = Rot;
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, curTilt);
            var Quat = Quaternion.Lerp(transform.rotation, lookTo, Time.deltaTime * 8f);
            //Vector3 lerpedVec2 = Vector3.Lerp(transform.forward,point,Time.deltaTime * spd);
            transform.rotation = Quat;
            //transform.LookAt(lockedTarget.position);
            // I want to rotate the parent so the transform.forward points towards 
            Vector3 thing = Vector3.Lerp(transform.parent.forward, toLockPoint.normalized, 0.2f * Time.deltaTime);
            //float val = Vector3.Lerp(Vector3.SignedAngle(point - transform.position, transform.parent.forward, transform.parent.up));
            //transform.parent.RotateAround(transform.parent.position, transform.parent.up, val);
            //transform.parent.LookAt(lockedTarget.position);
            transform.parent.rotation = Quaternion.Lerp(parentRot, flatLookTo, Time.deltaTime * 8f); 
            //currentLook = new(transform.localEulerAngles.x, transform.localEulerAngles.y);

            currentLook.x = transform.parent.eulerAngles.y;
            currentLook.y = Mathf.Clamp(transform.rotation.y, -90, 90);
            return;
        }
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseInput.x *= sensX;
        mouseInput.y *= sensY;

        currentLook.x += mouseInput.x;
        currentLook.y = Mathf.Clamp(currentLook.y += mouseInput.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-currentLook.y, Vector3.right);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, curTilt);
        transform.parent.transform.localRotation = Quaternion.Euler(0, currentLook.x, 0);
    }

    public void Punch(Vector2 dir)
    {
        sway += dir;
    }

    public void SetTilt(float newVal)
    {
        wishTilt = newVal;
    }
}
