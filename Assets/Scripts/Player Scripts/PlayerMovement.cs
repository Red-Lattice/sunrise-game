using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Initializing Objects")]
    // Note: Keep the amount of these reduced
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerLook playerLookScript;
    [SerializeField] private WorldPropertiesScriptableObject worldProperties;
    
    [Header("Changeable Parameters")]
    // Note: Make as many of these as possible
    [SerializeField] private float playerSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float sprintAcceleration;
    [SerializeField] private float maxSprintVelocity;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float jetpackAcceleration;
    
    // Other values
    private float sprintVelocity;
    private float staticAcceleration;
    private float staticDecceleration;
    private bool wallRunning;
    private bool jetpackActive;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    private Vector3 playerVelocity;
    private bool isGrounded;

    void Start()
    {
        staticAcceleration = acceleration;
        staticDecceleration = decceleration;
    }

    // Update is called once per frame
    void Update()
    {
        rotatePlayer();

        // Checks all of the keys.
        // In separate methods to improve code readability.

        //We have the global position,
        checkWKey();
        checkAKey();
        checkSKey();
        checkDKey();

        applyGravity();
        checkJump();

        zeroOutSmallValues();
        
        moveDirection.z += sprintVelocity;

        playerController.Move(transform.TransformDirection(moveDirection) * playerSpeed * Time.deltaTime);

        moveDirection.z -= sprintVelocity;
    }

    private void rotatePlayer()
    {
        playerTransform.rotation = Quaternion.Euler(0, playerLookScript.getYRot(), 0);
    }

    private void checkWKey()
    {
        sprintCheck();
        if (Input.GetKey(KeyCode.W))
        {
            if (moveDirection.z < 1.0F)
            {
                moveDirection.z += (acceleration / 100.0F);
            }
            return;
        }
        if (moveDirection.z > 0.0F)
        {
            moveDirection.z -= (decceleration / 100.0F);
        }
    }

    private void checkSKey()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (moveDirection.z > -1.0F)
            {
                moveDirection.z -= (acceleration / 100.0F);
            }
            return;
        }
        if (moveDirection.z < 0.0F)
        {
            moveDirection.z += (decceleration / 100.0F);
        }
    }

    private void checkAKey()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (moveDirection.x > -1.0F)
            {
                moveDirection.x -= (acceleration / 100.0F);
            }
            return;
        }
        if (moveDirection.x < 0.0F)
        {
            moveDirection.x += (decceleration / 100.0F);
        }
    }

    private void checkDKey()
    {
        if (Input.GetKey(KeyCode.D))
        {
            if (moveDirection.x < 1.0F)
            {
                moveDirection.x += (acceleration / 100.0F);
            }
            return;
        }
        if (moveDirection.x > 0.0F)
        {
            moveDirection.x -= (decceleration / 100.0F);
        }
    }

    private void sprintCheck()
    {
        if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.S))
        {
            if (sprintVelocity < maxSprintVelocity)
            {
                sprintVelocity += (sprintAcceleration / 100);
            }
        }
        else
        {
            if (sprintVelocity > 0.0F)
            {
                sprintVelocity -= (sprintAcceleration / 100);
            }
        }
        if (sprintVelocity < 0.0F)
        {
            // We don't want negative sprint lol
            sprintVelocity = 0.0F;
        }
    }

    // This prevents tiny values from being static and the player sliding or whatever
    private void zeroOutSmallValues()
    {
        if (0.0F < Mathf.Abs(moveDirection.x) && Mathf.Abs(moveDirection.x) < 0.001F)
        {
            moveDirection.x = 0.0F;
        }
        if (0.0F < Mathf.Abs(moveDirection.z) && Mathf.Abs(moveDirection.z) < 0.001F)
        {
            moveDirection.z = 0.0F;
        }
    }

    private void applyGravity()
    {
        //if (!playerController.isGrounded)
        //{
            moveDirection.y -= (worldProperties.getGravity() / 100) * 0.25F;
            if (!playerController.isGrounded)
            {
                acceleration = staticAcceleration / 3;
                decceleration = 0.0F;
            }
            else
            {
                acceleration = staticAcceleration;
                decceleration = staticDecceleration;
                moveDirection.y = 0.0F;
            }
        //}
        //else
        //{
        //    acceleration = staticAcceleration;
        //    decceleration = staticDecceleration;
        //    moveDirection.y = 0.0F;
        //}
    }

    private void checkJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerController.isGrounded)
            {
                moveDirection.y += (jumpVelocity);
                return;
            }
            /*else
            {
                jetPack(true);
            }*/
        }
        else
        {
            /*if (Input.GetKey(KeyCode.Space))
            {
                if (playerController.isGrounded)
                {
                    return;
                }
                else
                {
                    if (jetpackActive)
                    {
                        jetPack(false);
                    }
                }
            }*/
        }
        /*if (Input.GetKey(KeyCode.Space))
        {
            if (playerController.isGrounded)
            {
                moveDirection.y += jumpVelocity;
                return;
            }
            else
            {
                jetPack();
            }
        }*/
    }

    private void jetPack(bool initial)
    {
        if (initial)
        {
            moveDirection.y += jumpVelocity / 2F;
            jetpackActive = true;
            return;
        }
        else
        {
            moveDirection.y += (jetpackAcceleration / 100F);
            return;
        }
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            moveDirection.y += jumpVelocity / 0.5F;
            return;
        }
        else
        {
            moveDirection.y += (jetpackAcceleration / 100F);
            return;
        }*/
    }

    //Recieves inputs from InputManager.cs and applies them to character controller
    //public void ProcessMove(Vector2 input)
    //{
        //Vector3 moveDirection = Vector3.zero;
        //moveDirection.x = input.x;
        //moveDirection.z = input.y;
        
        

        //playerController.Move(transform.TransformDirection(moveDirection) * playerSpeed * Time.deltaTime * acceleration);
    //}

    // SETTERS


    // GETTERS
}
