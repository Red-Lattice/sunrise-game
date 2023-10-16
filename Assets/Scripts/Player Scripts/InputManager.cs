using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInput.OnFootActions OnFoot;
    [SerializeField] private PlayerMovement playerMovement;

    void Awake()
    {
        playerInput = new PlayerInput();
        OnFoot = playerInput.OnFoot;
    }

    private void OnEnable()
    {
        OnFoot.Enable();
    }

    private void OnDisable()
    {
        OnFoot.Disable();
    }

    void FixedUpdate()
    {
        //tells PlayerMovement to move using the value from our movement action.
        //playerMovement.ProcessMove(OnFoot.Movement.ReadValue<Vector2>());
    }
}
