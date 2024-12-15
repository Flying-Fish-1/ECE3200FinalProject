using System. Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;
    public static Vector2 Movement;

    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool AttackWasPressed;
    public static bool HeavyAttackWasPressed;


    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _attackAction;
    private InputAction _heavyAttackAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        
        _moveAction = PlayerInput.actions ["Move"];
        _jumpAction = PlayerInput.actions ["Jump"];
        _runAction = PlayerInput.actions ["Run"];
        _attackAction = PlayerInput.actions ["Attack"];
        _heavyAttackAction = PlayerInput.actions ["HeavyAttack"];
    }

    private void Update()
    {
        Movement = _moveAction. ReadValue<Vector2>();
        
        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction. IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction. IsPressed();
        AttackWasPressed = _attackAction.IsPressed();
        HeavyAttackWasPressed = _heavyAttackAction.IsPressed();
    }
}