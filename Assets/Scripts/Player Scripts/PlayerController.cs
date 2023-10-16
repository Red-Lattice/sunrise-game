using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float runningSpeed = 11.5f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float initialBoost;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit;
    [SerializeField] private float jetpackSpeed;
    [SerializeField] private float fuelLimit;
    [SerializeField] private float wallrunSpeed;
    [SerializeField] private float wallMomentumDecay;

    [Header("Initialization")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private WallRunning wallrunScript;

    [Header("Debugging")]
    [SerializeField] Vector3 moveDirection = Vector3.zero;
    [SerializeField] private bool doubleJumped;
    [SerializeField] private bool gravityActive;
    [SerializeField] private bool wallrunningActive;
    [SerializeField] Vector3 wallForwardVector = Vector3.zero;
    [SerializeField] Vector3 wallNormalVector = Vector3.zero;
    [SerializeField] Vector3 wallMomentum = Vector3.zero;
    private bool jetpackUnusable;
    private float jetpackFuel;
    float rotationX = 0;

    private bool canMove = true;

    void Start()
    {
        gravityActive = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (wallMomentum.x > 0.1F || wallMomentum.y > 0.1F || wallMomentum.z > 0.1F)
        {
            if (wallMomentum.x > 0)
            {
                wallMomentum.x = (wallMomentum.x > 0.1) ? (wallMomentum.x - (wallMomentumDecay * Time.deltaTime)) : 0F;
            }
            if (wallMomentum.x < 0)
            {
                wallMomentum.x = (wallMomentum.x < 0.1) ? (wallMomentum.x + (wallMomentumDecay * Time.deltaTime)) : 0F;
            }
            if (wallMomentum.y > 0)
            {
                wallMomentum.y = (wallMomentum.y > 0.1) ? (wallMomentum.y - (wallMomentumDecay * Time.deltaTime)) : 0F;
            }
            if (wallMomentum.y < 0)
            {
                wallMomentum.y = (wallMomentum.y < 0.1) ? (wallMomentum.y + (wallMomentumDecay * Time.deltaTime)) : 0F;
            }
            if (wallMomentum.z > 0)
            {
                wallMomentum.y = (wallMomentum.y > 0.1) ? (wallMomentum.z - (wallMomentumDecay * Time.deltaTime)) : 0F;
            }
            if (wallMomentum.z < 0)
            {
                wallMomentum.z = (wallMomentum.z < 0.1) ? (wallMomentum.z + (wallMomentumDecay * Time.deltaTime)) : 0F;
            }
            //wallMomentum = wallMomentum * 0.75F;
            //if (wallMomentum.x < 0.1F && wallMomentum.y < 0.1F && wallMomentum.z < 0.1F)
            //{
            //    wallMomentum = Vector3.zero;
            //}
        }
        if (!wallrunningActive)
        {
            normalMovementStuff();
        }

        manageFuel();
    }
    void LateUpdate()
    {
        if (wallrunningActive)
        {
            WallRunningStuff();
        }
    }

    private void doubleJumpCheck(float miscParam)
    {
        if (!doubleJumped)
        {
            moveDirection.y = -miscParam;
            moveDirection.y += (gravity * Time.deltaTime) + miscParam + initialBoost;
            jetpackUnusable = true;
            doubleJumped = true;
        }
        else
        {
            if (jetpackFuel > 0f && jetpackFuel < (fuelLimit + 0.1f))
            {
                jetpackFuel -= Time.deltaTime;
                moveDirection.y = (gravity * Time.deltaTime) + miscParam + (jetpackSpeed * Time.deltaTime);
                jetpackUnusable = true;
            }
        }
        jetpackUnusable = true;
    }

    private void manageFuel()
    {
        if (!jetpackUnusable && jetpackFuel < 3f)
        {
            jetpackFuel += Time.deltaTime;
        }
        // Checksum to prevent illegal fuel amount
        if (jetpackFuel > (fuelLimit + 0.1f))
        {
            jetpackFuel = fuelLimit - 1f;
        }
    }

    private void normalMovementStuff()
    {
        jetpackUnusable = false;
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S);
        // Forward and backward
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        // Left and right
        float curSpeedY = canMove ? (walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        // movementDirectionY saves the old moveDirection.y since it gets overwritten in the next line.
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        //Resets doubleJumped
        doubleJumped = characterController.isGrounded ? false : doubleJumped;

        if (Input.GetKeyDown(KeyCode.Space) && canMove)
        {
            if (characterController.isGrounded)
            {
                // Normal jump
                moveDirection.y = jumpSpeed;
            }
            else
            {
                // Triggers the jetpack jump
                doubleJumpCheck(movementDirectionY);
                movementDirectionY = 0F;
            }
            jetpackUnusable = true;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        if (!jetpackUnusable && Input.GetKey(KeyCode.Space) && doubleJumped) 
        {
            doubleJumpCheck(movementDirectionY);
        }
        
        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (gravityActive)
        {
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
        } 
        else
        {
            moveDirection.y = 0F;
        }

        // Move the controller
        characterController.Move((moveDirection + wallMomentum) * Time.deltaTime);

        playerAndCameraRotation();
    }

    // Things to keep in mind:
    // You'll only ever do a proper jump off a wall
    // Component along vector in direction of wall?
    private void WallRunningStuff()
    {
        jetpackUnusable = false;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //STEP 1: Get vectors representing the direction the player moves (with the magnitudes)
        //STEP 2: Combine into one vector
        //STEP 3: find the projection of that vector onto the wall vector
        //Step 4: Move player in that direction

        // Forward and backward
        float curSpeedX = (wallrunSpeed) * Input.GetAxis("Vertical");
        // Left and right
        float curSpeedY = (wallrunSpeed) * Input.GetAxis("Horizontal");
        // movementDirectionY saves the old moveDirection.y since it gets overwritten in the next line.
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        Vector3 projectedVector = Vector3.Project(moveDirection, wallForwardVector);

        doubleJumped = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Wall jump
            moveDirection.y = jumpSpeed;
            jetpackUnusable = true;
            wallMomentum = wallNormalVector * jumpSpeed;
            characterController.Move((moveDirection + wallMomentum) * Time.deltaTime);
            setWallrunningActivity(false);
            setGravityActivity(true);
            return;
        }

        // Move the controller
        characterController.Move(projectedVector * Time.deltaTime);

        playerAndCameraRotation();
    }

    public float getWallrunSpeed()
    {
        return wallrunSpeed;
    }

    public void setWallrunningActivity(bool state)
    {
        wallrunningActive = state;
    }

    public bool getWallrunningActivity()
    {
        return wallrunningActive;
    }

    public void setGravityActivity(bool gravityStatus)
    {
        gravityActive = gravityStatus;
    }

    public void updateForwardVector(Vector3 recievedVector)
    {
        wallForwardVector = recievedVector;
    }

    public void updateNormalVector(Vector3 recievedVector)
    {
        wallNormalVector = recievedVector;
    }

    private void playerAndCameraRotation()
    {
        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}