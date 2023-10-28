using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LepPlayerMovement : MonoBehaviour
{
    /**
    * This code is a modified version of some code originally written by youtube user Leprawel
    */

    //Ground
    [Header("Ground")]
    private float groundSpeed = 6f;
    [SerializeField] private float playerSpeed = 6f;
    [SerializeField] private float grAccel = 20f;
    [SerializeField] private float frictionConstant;
    [SerializeField] private float coefficientOfFriction;

    //Air
    [Header("Air")]
    [SerializeField] private float airSpeed = 3f;
    [SerializeField] private float airAccel = 20f;
    [SerializeField] private float airDash = 100f;
    [SerializeField] private float gravityDisableTimer = 0f;

    //Jump
    [Header("Jump")]
    [SerializeField] private float jumpUpSpeed = 9.2f;
    [SerializeField] private float dashSpeed = 6f;

    //Wall
    [Header("Wall Stuff")]
    [SerializeField] private float wallSpeed = 10f;
    [SerializeField] private float wallClimbSpeed = 4f;
    [SerializeField] private float wallAccel = 20f;
    [SerializeField] private float wallRunTime = 3f;
    [SerializeField] private float wallStickiness = 20f;
    [SerializeField] private float wallStickDistance = 1f;
    [SerializeField] private float wallFloorBarrier = 40f;
    [SerializeField] private float wallBanTime = 4f;
    Vector3 bannedGroundNormal;

    //Cooldowns
    bool canJump = true;
    bool canDJump = true;
    bool canAirDash = false;
    float wallBan = 0f;
    float wrTimer = 0f;
    float wallStickTimer = 0f;

    //States
    bool jump;
    bool crouched;
    [Header("Debug (read only)")]
    [SerializeField] bool grounded;

    Collider ground;

    Vector3 groundNormal = Vector3.up;
    

    enum Mode
    {
        Walking,
        Flying,
        Wallruning
    }
    [SerializeField] Mode mode = Mode.Flying;
    [Header("Initialization")]
    [SerializeField] private CapsuleCollider col;
    [SerializeField] private CameraController camCon;
    [SerializeField] private Transform camTranform;
    [SerializeField] private Rigidbody rb;
    Vector3 dir = Vector3.zero;

    void OnGUI()
    {
        GUILayout.Label("Velocity: " + new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude);
        GUILayout.Label("Upward Velocity: " + rb.velocity.y);
    }

    void Update()
    {
        if (gravityDisableTimer > 0f)
        {
            gravityDisableTimer -= Time.deltaTime * 5;
        }
        else
        {
            rb.useGravity = !(mode == Mode.Walking);
        }

        col.material.dynamicFriction = 0f;
        dir = Direction();

        crouched = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    void AirDash()
    {
        if (gravityDisableTimer < 0.05)
        {
            rb.useGravity = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canAirDash)
        {
            //Vector3 lookDir = new Vector3(camTranform.rotation.x, camTranform.roation.y, camTransform.rotation.z);
            rb.AddForce(camTranform.forward * airDash, ForceMode.Force);
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            canDJump = true;
            canAirDash = false;
            rb.useGravity = false;
            gravityDisableTimer = 1f;
        }
    }

    void FixedUpdate()
    {
        if (crouched)
        {
            col.height = Mathf.Max(0.6f, col.height - Time.deltaTime * 10f);
            groundSpeed = 0.3f * playerSpeed;
        }
        else
        {
            col.height = Mathf.Min(1.8f, col.height + Time.deltaTime * 10f);
            groundSpeed = playerSpeed;
        }

        if (wallStickTimer == 0f && wallBan > 0f)
        {
            bannedGroundNormal = groundNormal;
        }
        else
        {
            bannedGroundNormal = Vector3.zero;
        }

        wallStickTimer = Mathf.Max(wallStickTimer - Time.deltaTime, 0f);
        wallBan = Mathf.Max(wallBan - Time.deltaTime, 0f);

        switch (mode)
        {
            case Mode.Wallruning:
                camCon.SetTilt(WallrunCameraAngle());
                Wallrun(dir, wallSpeed, wallClimbSpeed, wallAccel);
                if (ground.tag != "InfiniteWallrun") wrTimer = Mathf.Max(wrTimer - Time.deltaTime, 0f);
                break;

            case Mode.Walking:
                camCon.SetTilt(0);
                Walk(dir, groundSpeed, grAccel);
                break;

            case Mode.Flying:
                camCon.SetTilt(0);
                AirMove(dir, airSpeed, airAccel);
                break;
        }

        jump = false;
    }



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
        if (collision.contactCount > 0)
        {
            float angle;

            foreach (ContactPoint contact in collision.contacts)
            {
                angle = Vector3.Angle(contact.normal, Vector3.up);
                if (angle < wallFloorBarrier)
                {
                    EnterWalking();
                    grounded = true;
                    groundNormal = contact.normal;
                    ground = contact.otherCollider;
                    return;
                }
            }

            if (VectorToGround().magnitude > 0.2f)
            {
                grounded = false;
            }

            if (grounded == false)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (contact.otherCollider.tag != "NoWallrun" && contact.otherCollider.tag != "Player" && mode != Mode.Walking)
                    {
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
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.contactCount == 0)
        {
            EnterFlying();
        }
    }
    #endregion



    #region Entering States
    void EnterWalking()
    {
        if (mode != Mode.Walking && canJump)
        {
            if (mode == Mode.Flying && crouched)
            {
                rb.AddForce(-rb.velocity.normalized, ForceMode.VelocityChange);
            }
            if (rb.velocity.y < -1.2f)
            {
                camCon.Punch(new Vector2(0, -3f));
            }
            //StartCoroutine(bHopCoroutine(bhopLeniency));
            mode = Mode.Walking;
        }
    }

    void EnterFlying(bool wishFly = false)
    {
        grounded = false;
        canAirDash = true;
        if (mode == Mode.Wallruning && VectorToWall().magnitude < wallStickDistance && !wishFly)
        {
            return;
        }
        else if (mode != Mode.Flying)
        {

            wallBan = wallBanTime;
            canDJump = true;
            mode = Mode.Flying;
        }
    }

    void EnterWallrun()
    {
        if (mode != Mode.Wallruning)
        {
            if (VectorToGround().magnitude > 0.2f && CanRunOnThisWall(bannedGroundNormal) && wallStickTimer == 0f)
            {
                wrTimer = wallRunTime;
                canDJump = true;
                mode = Mode.Wallruning;
            }
            else
            {
                EnterFlying(true);
            }
        }
    }
    #endregion



    #region Movement Types
    void Walk(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        //wishDir is just the direction vector.
        if (jump && canJump)
        {
            Jump();
        }
        else
        {
            //if (crouched) acceleration = 0.5f;
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

        if (mode != Mode.Wallruning)
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

    void AirMove(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        if (jump && !crouched)
        {
            DoubleJump(wishDir);
        }

        if (crouched && rb.velocity.y > -10 && Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
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
        }
        else if (wrTimer == 0f || crouched)
        {
            rb.AddForce(groundNormal * 3f, ForceMode.VelocityChange);
            EnterFlying(true);
        }
        else
        {
            //Horizontal
            Vector3 distance = VectorToWall();
            wishDir = RotateToPlane(wishDir, -distance.normalized);
            wishDir *= maxSpeed;
            wishDir.y = Mathf.Clamp(wishDir.y, -climbSpeed, climbSpeed);
            Vector3 wallrunForce = wishDir - rb.velocity;
            if (wallrunForce.magnitude > 0.2f) wallrunForce = wallrunForce.normalized * acceleration;

            //Vertical
            if (rb.velocity.y < 0f && wishDir.y > 0f) wallrunForce.y = 2f * acceleration;

            //Anti-gravity force
            Vector3 antiGravityForce = -Physics.gravity;
            if (wrTimer < 0.33 * wallRunTime)
            {
                antiGravityForce *= wrTimer / wallRunTime;
                wallrunForce += (Physics.gravity + antiGravityForce);
            }

            //Forces
            rb.AddForce(wallrunForce, ForceMode.Acceleration);
            rb.AddForce(antiGravityForce, ForceMode.Acceleration);
            if (distance.magnitude > wallStickDistance) distance = Vector3.zero;
            rb.AddForce(distance * wallStickiness, ForceMode.Acceleration);
        }
        if (!grounded)
        {
            wallStickTimer = 0.2f;
            EnterFlying();
        }
    }

    void Jump()
    {
        if (mode == Mode.Walking && canJump)
        {
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);
            StartCoroutine(jumpCooldownCoroutine(0.2f));
            EnterFlying(true);
        }
    }

    void DoubleJump(Vector3 wishDir)
    {
        if (canDJump)
        {
            //Vertical
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);

            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

            //Horizontal
            if (wishDir != Vector3.zero)
            {
                Vector3 horSpid = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                Vector3 newSpid = wishDir.normalized;
                float newSpidMagnitude = dashSpeed;

                if (horSpid.magnitude > dashSpeed)
                {
                    float dot = Vector3.Dot(wishDir.normalized, horSpid.normalized);
                    if (dot > 0)
                    {
                        newSpidMagnitude = dashSpeed + (horSpid.magnitude - dashSpeed) * dot;
                    }
                    else
                    {
                        newSpidMagnitude = Mathf.Clamp(dashSpeed * (1 + dot), dashSpeed * (dashSpeed / horSpid.magnitude), dashSpeed);
                    }
                }

                newSpid *= newSpidMagnitude;

                rb.AddForce(newSpid - horSpid, ForceMode.VelocityChange);
            }

            canDJump = false;
        }
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
        vect = rotation * vect;
        return vect;
    }

    float WallrunCameraAngle()
    {
        Vector3 rotDir = Vector3.ProjectOnPlane(groundNormal, Vector3.up);
        Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
        rotDir = rotation * rotDir;
        float angle = Vector3.SignedAngle(Vector3.up, groundNormal, Quaternion.AngleAxis(90f, rotDir) * groundNormal);
        angle -= 90;
        angle /= 180;
        Vector3 playerDir = transform.forward;
        Vector3 normal = new Vector3(groundNormal.x, 0, groundNormal.z);

        return Vector3.Cross(playerDir, normal).y * angle;
    }

    bool CanRunOnThisWall(Vector3 normal)
    {
        return (Vector3.Angle(normal, groundNormal) > 10 || wallBan == 0f);
    }

    Vector3 VectorToWall()
    {
        Vector3 direction;
        Vector3 position = transform.position + Vector3.up * col.height / 2f;
        RaycastHit hit;
        if (Physics.Raycast(position, -groundNormal, out hit, wallStickDistance) && Vector3.Angle(groundNormal, hit.normal) < 70)
        {
            groundNormal = hit.normal;
            direction = hit.point - position;
            return direction;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }

    Vector3 VectorToGround()
    {
        Vector3 position = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, wallStickDistance))
        {
            return hit.point - position;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }
    #endregion



    #region Coroutines
    IEnumerator jumpCooldownCoroutine(float time)
    {
        canJump = false;
        yield return new WaitForSeconds(time);
        canJump = true;
    }
    #endregion

    public string getMode()
    {
        return mode.ToString();
    }

    public float getPlayerSpeed()
    {
        return Math.Abs(rb.velocity.x + rb.velocity.z);
    }
}
