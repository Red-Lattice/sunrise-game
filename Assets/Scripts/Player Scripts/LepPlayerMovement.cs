using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static Mode;

enum Mode
{
    Walking,
    Flying,
    Wallruning,
    Sliding
}

public class LepPlayerMovement : MonoBehaviour
{
    /**
    * This code is a modified version of some code originally written by youtube user Leprawel
    */
    #region Fields
    //Ground
    [Header("Ground")]
    [SerializeField] private float groundSpeed = 8f;
    private const float playerSpeed = 8f;
    private const float grAccel = 30f;
    [SerializeField] private float frictionConstant;
    [SerializeField] private float coefficientOfFriction;

    //Air
    [Header("Air")]
    private const float airSpeed = 6f;
    private const float airAccel = 30f;

    //Jump
    //[Header("Jump")]
    private const float jumpUpSpeed = 9.2f;
    private const float dashSpeed = 6f;

    //Wall
    //[Header("Wall Stuff")]
    private const float wallSpeed = 10f;
    private const float wallClimbSpeed = 1f;
    private const float wallAccel = 20f;
    private const float wallRunTime = 3f;
    private const float wallStickiness = 20f;
    private const float wallStickDistance = 1f;
    private const float wallFloorBarrier = 40f;
    private const float wallBanTime = 4f;
    Vector3 bannedGroundNormal;

    //Cooldowns
    bool canJump = true;
    bool canDJump = true;
    float wallBan = 0f;
    float wrTimer = 0f;
    float wallStickTimer = 0f;
    bool slideBoostApplied = false;
    float slideTimer = 0f;

    //States
    bool jump;
    bool crouched;
    bool grounded;
    Mode mode = Flying;

    [Header("Initialization")]
    [SerializeField] private CameraController camCon;

    private CapsuleCollider col;
    private Rigidbody rb;
    Vector3 dir = Vector3.zero;
    Collider ground;
    Vector3 groundNormal = Vector3.up;

    #endregion

    /*void OnGUI()
    {
        GUILayout.Label("Velocity: " + new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude);
        GUILayout.Label("Upward Velocity: " + rb.velocity.y);
    }*/

    void Awake()
    {
        col = transform.gameObject.GetComponent<CapsuleCollider>();
        rb = transform.gameObject.GetComponent<Rigidbody>();
        col.material.dynamicFriction = 0f;
    }

    #region Update
    void Update()
    {
        if (rb.drag > 0.2f) {rb.drag = 0f;}
        if (slideTimer > 0f)
        {
            slideTimer = (!crouched) ? slideTimer - Time.deltaTime : slideTimer;
        }
        else
        {
            slideBoostApplied = false;
        }
        rb.useGravity = mode != Mode.Walking && mode != Mode.Sliding;

        dir = Direction();
        if (dir.magnitude < 0.1f && getPlayerSpeed() < 0.5f) {rb.drag = 11f;}

        crouched = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        jump = Input.GetKeyDown(KeyCode.Space) ? true : jump;
    }

    void FixedUpdate()
    {
        crouched = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

        bannedGroundNormal = (wallStickTimer == 0f && wallBan > 0f) 
            ? groundNormal : Vector3.zero;

        wallStickTimer = Mathf.Max(wallStickTimer - Time.deltaTime, 0f);
        wallBan = Mathf.Max(wallBan - Time.deltaTime, 0f);

        camCon.SetTilt((mode == Wallruning) ? WallrunCameraAngle() : 0);
        switch (mode)
        {
            case Wallruning:
                Wallrun(dir, wallSpeed, wallClimbSpeed, wallAccel);
                if (ground.tag != "InfiniteWallrun") wrTimer = Mathf.Max(wrTimer - Time.deltaTime, 0f);
                break;

            case Walking:
                Walk(dir, groundSpeed, grAccel);
                break;

            case Sliding:
                Slide(dir, groundSpeed, grAccel);
                break;

            case Flying:
                AirMove(dir, airSpeed, airAccel);
                break;
        }

        if (crouched)
        {
            if (0.6f < col.height - Time.deltaTime)
            {
                col.height = Mathf.Max(0.6f, col.height - Time.deltaTime * 10f);
                if (mode != Flying)
                    col.transform.position += new Vector3(0, -Time.deltaTime * 2f, 0);
            }
            groundSpeed = 0.3f * playerSpeed;
        }
        else
        {
            col.height = Mathf.Min(1.5f, col.height + Time.deltaTime * 5f);
            groundSpeed = playerSpeed;
        }

        jump = false;
    }

    #endregion

    private Vector3 Direction()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hAxis, 0, vAxis);
        return rb.transform.TransformDirection(direction);
    }

    #region Collisions
    void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount == 0) {return;}
        float angle;

        foreach (ContactPoint contact in collision.contacts)
        {
            angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle >= wallFloorBarrier) {continue;}

            if (crouched)
            {
                EnterSliding();
            }
            else
            {
                EnterWalking();
            }
            
            grounded = true;
            groundNormal = contact.normal;
            ground = contact.otherCollider;
            return;
        }

        if (VectorToGround().magnitude > 0.2f)
        {
            grounded = false;
        }

        if (grounded) {return;}
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.tag == "NoWallrun" || mode == Walking)
            {
                continue;
            }

            angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle > wallFloorBarrier && angle < 120f)
            {
                grounded = true;
                groundNormal = contact.normal;
                ground = contact.otherCollider;
                EnterWallrun();
                return;
            }
        }
    }

    void OnCollisionExit()
    {
        //if (collision.contactCount == 0)
        //{
            EnterFlying();
        //}
    }
    #endregion



    #region Entering States
    void EnterWalking()
    {
        if (mode == Walking || !canJump) {return;}

        if (mode == Flying && crouched)
        {
            rb.AddForce(-rb.velocity.normalized, ForceMode.VelocityChange);
        }
        //StartCoroutine(bHopCoroutine(bhopLeniency));
        mode = Walking;
    }

    void EnterSliding()
    {
        if (mode != Sliding && canJump)
        {
            mode = Sliding;
        }
    }

    void EnterFlying(bool wishFly = false)
    {
        grounded = false;
        if ((mode == Wallruning && VectorToWall().magnitude < wallStickDistance && !wishFly) 
            || (mode == Flying))
        {
            return;
        }
        wallBan = wallBanTime;
        canDJump = true;
        mode = Flying;
    }

    void EnterWallrun()
    {
        if (mode == Wallruning) {return;}

        if (VectorToGround().magnitude > 0.2f && CanRunOnThisWall(bannedGroundNormal) && wallStickTimer == 0f)
        {
            wrTimer = wallRunTime;
            canDJump = true;
            mode = Wallruning;
            return;
        }
        EnterFlying(true);
    }
    #endregion



    #region Movement Types
    void Walk(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        //wishDir is just the direction vector that the player wants to go in.
        if (jump && canJump)
        {
            Jump();
        }
        else
        {
            if (crouched)
            {
                slideBoost();
            }
            wishDir = wishDir.normalized;
            Vector3 spid = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (spid.magnitude > maxSpeed) acceleration *= spid.magnitude / maxSpeed;
            Vector3 direction = wishDir * maxSpeed - spid;
            // direction = -spid
            if (direction.magnitude < 0.5f)
            {
                //acceleration = acceleration * (-spid / 0.5f)
                acceleration *= direction.magnitude / 0.5f;
            }
            direction = direction.normalized * acceleration;
            float magn = direction.magnitude;
            direction = direction.normalized;
            direction *= magn;

            direction = Vector3.ProjectOnPlane(direction, groundNormal);
            rb.AddForce(direction, ForceMode.Acceleration);
        }

        if (mode != Wallruning && !(Input.GetMouseButtonDown(2)))
        {
            if (rb.velocity.magnitude < coefficientOfFriction)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.AddForce(-1f * rb.velocity * frictionConstant, ForceMode.Acceleration);
            }
        }
    }

    void Slide(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        float slideFrictionConstant = 1f;
        if (jump && canJump)
        {
            Jump();
            return;
        }

        slideBoost();

        rb.AddForce(-1f * rb.velocity * slideFrictionConstant, ForceMode.Acceleration);
    }

    void slideBoost()
    {
        if (!slideBoostApplied)
        {
            Vector3 direction = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            direction = Vector3.ProjectOnPlane(direction, groundNormal);
            rb.AddForce(direction * 0.75f, ForceMode.VelocityChange);
            slideBoostApplied = true;
            slideTimer = 2f;
        }
    }

    void AirMove(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        if (jump)
        {
            DoubleJump(wishDir);
        }

        float projVel = Vector3.Dot(new Vector3(rb.velocity.x, 0f, rb.velocity.z), wishDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = acceleration * Time.deltaTime; // Accelerated velocity in direction of movment

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        if (projVel + accelVel > maxSpeed)
            accelVel = Mathf.Max(0f, maxSpeed - projVel);

        rb.AddForce(wishDir.normalized * accelVel, ForceMode.VelocityChange);
    }

    void Wallrun(Vector3 wishDir, float maxSpeed, float climbSpeed, float acceleration)
    {
        if (!grounded)
        {
            wallStickTimer = 0.2f;
            EnterFlying();
        }
        if (jump)
        {
            //Vertical
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

            //Horizontal
            Vector3 jumpOffWall = groundNormal.normalized;
            jumpOffWall *= dashSpeed;
            jumpOffWall.y = 0;
            rb.AddForce(jumpOffWall, ForceMode.VelocityChange);
            wrTimer = 0f;
            EnterFlying(true);
            return;
        }
        if (wrTimer == 0f || crouched)
        {
            rb.AddForce(groundNormal * 3f, ForceMode.VelocityChange);
            EnterFlying(true);
            return;
        }

        //Horizontal
        Vector3 distance = VectorToWall();
        wishDir = RotateToPlane(wishDir, -distance.normalized) * maxSpeed;
        wishDir.y = Mathf.Clamp(wishDir.y, -climbSpeed, climbSpeed);
        Vector3 wallrunForce = wishDir - rb.velocity;
        if (wallrunForce.magnitude > 0.2f) {wallrunForce = wallrunForce.normalized * acceleration;}

        //Vertical
        if (rb.velocity.y < 0f && wishDir.y > 0f) {wallrunForce.y = 2f * acceleration;}

        //Anti-gravity force
        Vector3 antiGravityForce = -Physics.gravity;
        if (wrTimer < 0.33 * wallRunTime)
        {
            antiGravityForce *= wrTimer / wallRunTime;
            wallrunForce += (Physics.gravity + antiGravityForce);
        }

        //Forces
        rb.AddForce(wallrunForce + antiGravityForce, ForceMode.Acceleration);
        if (distance.magnitude > wallStickDistance) {distance = Vector3.zero;}
        rb.AddForce(distance * wallStickiness, ForceMode.Acceleration);
    }

    void Jump()
    {
        if (!canJump) {return;}
        
        float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
        switch (mode)
        {
            case Walking:
            case Sliding:
                rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);
                EnterFlying(true);
                break;
            default:
                break;
        }
    }

    void DoubleJump(Vector3 wishDir)
    {
        if (!canDJump) {return;} // Guard

        // Vertical
        float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);

        rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

        // Horizontal
        if (wishDir == Vector3.zero) {canDJump = false; return;}

        Vector3 horSpid = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 newSpid = wishDir.normalized;
        float newSpidMagnitude = dashSpeed;

        if (horSpid.magnitude > dashSpeed)
        {
            float dot = Vector3.Dot(wishDir.normalized, horSpid.normalized);
            newSpidMagnitude = (dot > 0) 
                ? dashSpeed + (horSpid.magnitude - dashSpeed) * dot 
                : Mathf.Clamp(dashSpeed * (1 + dot), dashSpeed * (dashSpeed / horSpid.magnitude), dashSpeed);
        }

        newSpid *= newSpidMagnitude;

        rb.AddForce(newSpid - horSpid, ForceMode.VelocityChange);

        canDJump = false;
    }
    #endregion

    #region MathGenius
    Vector3 RotateToPlane(Vector3 vect, Vector3 normal)
    {
        Vector3 rotDir = Vector3.ProjectOnPlane(normal, Vector3.up);
        Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
        rotDir = rotation * rotDir;
        float angle = -Vector3.Angle(Vector3.up, normal);
        rotation = Quaternion.AngleAxis(angle, rotDir);
        return rotation * vect;
    }

    float WallrunCameraAngle()
    {
        Vector3 rotDir = Vector3.ProjectOnPlane(groundNormal, Vector3.up);
        Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
        rotDir = rotation * rotDir;
        
        float angle = Vector3.SignedAngle(Vector3.up, groundNormal, 
            Quaternion.AngleAxis(90f, rotDir) * groundNormal);

        angle = (angle - 90) / 180;
        Vector3 normal = new Vector3(groundNormal.x, 0, groundNormal.z);

        return Vector3.Cross(transform.forward, normal).y * angle;
    }

    bool CanRunOnThisWall(Vector3 normal)
    {
        return (Vector3.Angle(normal, groundNormal) > 10 || wallBan == 0f);
    }

    Vector3 VectorToWall()
    {
        Vector3 position = transform.position + Vector3.up * col.height / 2f;
        RaycastHit hit;
        if (Physics.Raycast(position, -groundNormal, out hit, wallStickDistance) && Vector3.Angle(groundNormal, hit.normal) < 70)
        {
            groundNormal = hit.normal;
            return hit.point - position;
        }
        return Vector3.positiveInfinity;
    }

    Vector3 VectorToGround()
    {
        Vector3 position = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, wallStickDistance))
        {
            return hit.point - position;
        }
        return Vector3.positiveInfinity;
    }
    #endregion

    #region Getters
    public string getMode()
    {
        return mode.ToString();
    }

    public float getPlayerSpeed()
    {
        return Math.Abs(rb.velocity.magnitude);
    }
    #endregion

    public void GrapplingStart()
    {
        canDJump = true;
        if (mode == Wallruning)
        {
            EnterFlying();
        }
    }
}
