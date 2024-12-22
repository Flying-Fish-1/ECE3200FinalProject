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
    public static bool SpecialAttackWasPressed;
    public static bool PauseWasPressed;


    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _attackAction;
    private InputAction _heavyAttackAction;
    private InputAction _specialAttackAction;
    private InputAction _pauseAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        
        _moveAction = PlayerInput.actions ["Move"];
        _jumpAction = PlayerInput.actions ["Jump"];
        _runAction = PlayerInput.actions ["Run"];
        _attackAction = PlayerInput.actions ["Attack"];
        _heavyAttackAction = PlayerInput.actions ["HeavyAttack"];
        _specialAttackAction = PlayerInput.actions ["SpecialAttack"];
        _pauseAction = PlayerInput.actions ["Pause"];
    }

    private void Update()
    {
        if (!GameManager.isPaused)
        {
            Movement = _moveAction. ReadValue<Vector2>();
            
            JumpWasPressed = _jumpAction.WasPressedThisFrame();
            JumpIsHeld = _jumpAction. IsPressed();
            JumpWasReleased = _jumpAction.WasReleasedThisFrame();

            RunIsHeld = _runAction. IsPressed();
            AttackWasPressed = _attackAction.IsPressed();
            HeavyAttackWasPressed = _heavyAttackAction.IsPressed();
            SpecialAttackWasPressed = _specialAttackAction.IsPressed();
            // PauseWasPressed = _pauseAction.IsPressed();
        }
        PauseWasPressed = _pauseAction.IsPressed();
    }
}