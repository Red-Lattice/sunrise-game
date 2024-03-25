using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Initialization")]
    private Camera mainCamera;
    public Camera weaponCamera;
    [SerializeField] Rigidbody rb;
    //[SerializeField] private GameObject marker;

    [Header("Values")]
    [SerializeField] private float sensX = 1f;
    [SerializeField] private float sensY = 1f;
    [SerializeField] private float baseFov = 90f;
    [SerializeField] private float maxFov = 140f;
    [SerializeField] private float wallRunTilt = 15f;
    [SerializeField] private float wishTilt = 0;
    [SerializeField] private float curTilt = 0;
    [SerializeField] private float interpValue = 0.1f;
    [SerializeField] private float lerpSpeed = 10f;
    Vector2 currentLook;
    Vector2 sway = Vector3.zero;
    float fov;
    public Vector3 point => (lockedTarget == null) ? Vector3.zero : lockedTarget.position;
    public bool locked {get; private set;}
    public bool hasTarget => lockedTarget != null;
    private Transform lockedTarget;
    

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        curTilt = transform.localEulerAngles.z;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {locked = !locked;}
        if (locked && !hasTarget) {locked = false;}
        if (!locked) {GetLockTarget();}
        //marker.transform.position = point;
        RotateMainCamera();
    }

    private LayerMask targets = 1 << 9;
    void GetLockTarget() {
        RaycastHit hit;
        Vector3 p1 = transform.position;

        // Cast character controller shape 10 meters forward to see if it is about to hit anything.
        if (Physics.CapsuleCast(p1, p1, 1f, transform.forward, out hit, 25f, targets)) {
            //marker.transform.position = hit.transform.position;
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
    private Quaternion lookTo => Quaternion.LookRotation(toLockPoint);
    private Quaternion parentRot => transform.parent.rotation;
    private Quaternion flatLookTo => Quaternion.LookRotation(new Vector3(toLockPoint.x, 0, toLockPoint.z));
    
    void RotateMainCamera()
    {
        if (locked) {LockedRotate(); return;}
        UnlockedRotate();
    }

    private float deltaLerpSpeed => Time.deltaTime * lerpSpeed;
    /// <summary>
    /// Runs when the camera is locked on a target
    /// </summary>
    void LockedRotate() {
        transform.rotation = Quaternion.Lerp(transform.rotation, lookTo, deltaLerpSpeed);
        transform.parent.rotation = Quaternion.Lerp(parentRot, flatLookTo, deltaLerpSpeed);

        currentLook.x = transform.parent.eulerAngles.y;
        currentLook.y = Mathf.Clamp(transform.rotation.y, -90, 90);
    }

    /// <summary>
    /// Runs when the camera is free to rotate
    /// </summary>
    void UnlockedRotate() {
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
