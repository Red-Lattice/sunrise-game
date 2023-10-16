using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    /*********************************************************************************************
    ** This code was partially stolen from this video: https://www.youtube.com/watch?v=gNt9wBOrQO4
    *********************************************************************************************/
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    private float maxWallRunTime;
    private float wallRunTimer = -1F;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    [SerializeField] private float wallCheckDistance;
    private float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    [SerializeField] private bool wallLeft;
    [SerializeField] private bool wallRight;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerController playerController;

    private void Update()
    {
        CheckForWall();
        StateMachine();
        if (playerController.getWallrunningActivity())
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        //**************************start point, direction, store hit info, distance
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsGround);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsGround);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0)
        {
            if (!playerController.getWallrunningActivity())
            {
                StartWallRun();
            }
        }
        // State 3 - none
        else
        {
            if (playerController.getWallrunningActivity())
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        playerController.setWallrunningActivity(true);
    }

    private void WallRunningMovement()
    {
        // Disable gravity
        playerController.setGravityActivity(false);

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        playerController.updateForwardVector(wallForward);
        playerController.updateNormalVector(wallNormal);
    }

    private void StopWallRun()
    {
        playerController.setWallrunningActivity(false);
        playerController.setGravityActivity(true);
    }
    
}
