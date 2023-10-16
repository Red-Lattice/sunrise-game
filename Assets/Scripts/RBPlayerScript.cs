using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBPlayerScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float jetpackForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float airDrag;
    [SerializeField] private float groundDistance;
    bool readyToJump;
    bool readyToDoubleJump;
    bool doubleJumped;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private bool grounded;
    [SerializeField] private float groundCheckErrorLength;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float radius;

    [SerializeField] private bool explosionActivate;

    [Header("SlopeHandling")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private bool onSlope;
    [SerializeField] private float angle;
    private RaycastHit slopeHit;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    [SerializeField] Rigidbody rb;

    private void Start()
    {
        readyToJump = true;
        readyToDoubleJump = false;
        rb.freezeRotation = true;
    }

    //private void explosion()
    //{
    //    if (explosionActivate)
    //    {
    //        explosionActivate = false;
    //        rb.AddForce(Vector3.up * 10F + Vector3.forward * 100F, ForceMode.Impulse);
    //    }
    //}

    private void Update()
    {
        onSlope = OnSlope();
        //explosion();
        // ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundCheckErrorLength);
        //grounded = groundedCheck.IsGrounded();
        //grounded = (rb.velocity.y == 0F);
        //grounded = Physics.CheckSphere(groundCheck.position, radius, whatIsGround);
        grounded = Physics.CheckCapsule( transform.position , transform.position - new Vector3(0,(2F * 0.25f + 0.2f), 0), radius, whatIsGround);

        MyInput();
        if (!onSlope)
        {
            SpeedControl();
        }
        else
        {
            AngledSpeedControl();
        }

        if (grounded)
        {
            rb.drag = groundDrag;
            readyToJump = true;
            doubleJumped = false;
            readyToDoubleJump = false;
        }
        else
        {
            rb.drag = airDrag;
        }

        if (rb.velocity.x == 0 && rb.velocity.y == 0 && rb.velocity.z == 0)
        {
            grounded = true;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
        }
        else
        {
            if (Input.GetKeyDown(jumpKey) && !grounded && !doubleJumped)
            {
                doubleJumped = true;
                DoubleJump();
            }
        }

        if (Input.GetKey(jumpKey) && !grounded && doubleJumped)
        {
            Jetpack();
        }
    }

    private void AirStrafing()
    {
        // NOTE: Improve this.
        
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            float upwardVelocity = rb.velocity.y;
            float flatVelocityMagnitude = flatVel.magnitude;
            Vector3 limitedVelocity = (flatVel + moveDirection).normalized * flatVelocityMagnitude;
            rb.velocity = limitedVelocity;
            rb.velocity += Vector3.up * upwardVelocity;
        }

        if ((flatVel.magnitude < moveSpeed))
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        /*if (onSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        }
        else
        {
            if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else
            {
                AirStrafing();
            }
        }*/
        if (grounded || OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        }
        else
        {
            AirStrafing();
        }
    }

    private void AngledSpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        // We don't want to limit the velocity if the player isn't grounded
        if ((flatVel.magnitude > moveSpeed))
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, limitedVel.y, limitedVel.z);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // We don't want to limit the velocity if the player isn't grounded
        if ((flatVel.magnitude > moveSpeed) && grounded)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0.01f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void DoubleJump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0.01f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void Jetpack()
    {
        rb.AddForce(transform.up * (jetpackForce / 100), ForceMode.Impulse);
    }

    public void updateGrounding(bool status)
    {
        grounded = status;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 2f, whatIsGround))
        {
            angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //angle < maxSlopeAngle &&
            return angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
