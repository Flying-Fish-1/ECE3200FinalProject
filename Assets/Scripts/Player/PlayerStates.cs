using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerIdleState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerIdleState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter._animator.Play("Idle");
    }

    public void OnUpdate()
    {
        
        if (InputManager.JumpWasPressed)
        {
            manager.TransitionState(PlayerState.Jump);
        }
        if (InputManager.Movement != Vector2.zero && !InputManager.RunIsHeld)
        {
            manager.TransitionState(PlayerState.Walk);
        }
        if (InputManager.RunIsHeld && InputManager.Movement.x != 0)
        {
            manager.TransitionState(PlayerState.Run);
        }
        if (InputManager.AttackWasPressed)
        {
            manager.TransitionState(PlayerState.LightAttack);
        }
        if (InputManager.HeavyAttackWasPressed)
        {
            manager.TransitionState(PlayerState.HeavyAttack);
        }

        
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        
    }
}

public class PlayerWalkState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerWalkState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter._animator.Play("Run");
        //Debug.Log("Walk");
    }

    public void OnUpdate()
    {
        if (InputManager.Movement == Vector2.zero)
        {
            manager.TransitionState(PlayerState.Idle);
        }
        if (InputManager.RunIsHeld)
        {
            manager.TransitionState(PlayerState.Run);
        }
        if (InputManager.JumpWasPressed)
        {
            manager.TransitionState(PlayerState.Jump);
        }
        if (InputManager.AttackWasPressed)
        {
            manager.TransitionState(PlayerState.LightAttack);
        }
        if (InputManager.HeavyAttackWasPressed)
        {
            manager.TransitionState(PlayerState.HeavyAttack);
        }
        

    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        
    }
}


public class PlayerRunState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerRunState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter._animator.Play("Run");
    }

    public void OnUpdate()
    {
        if (InputManager.Movement == Vector2.zero)
        {
            manager.TransitionState(PlayerState.Idle);
        }
        if (!InputManager.RunIsHeld)
        {
            manager.TransitionState(PlayerState.Walk);
        }
        if (InputManager.JumpWasPressed)
        {
            manager.TransitionState(PlayerState.Jump);
        }
        if (InputManager.AttackWasPressed)
        {
            manager.TransitionState(PlayerState.LightAttack);
        }
        if (InputManager.HeavyAttackWasPressed)
        {
            manager.TransitionState(PlayerState.HeavyAttack);
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        
    }
}

public class PlayerJumpState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerJumpState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter._animator.Play("Jump");
        //Debug.Log("Jump");
    }

    public void OnUpdate()
    {
        AnimatorStateInfo info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        float progress = info.normalizedTime;

        if (InputManager.Movement == Vector2.zero && !parameter._isJumping && progress >= 0.95f)
        {
            Debug.Log("To Idle");
            manager.TransitionState(PlayerState.Idle);
        }
        if (InputManager.RunIsHeld && !parameter._isJumping)
        {
            manager.TransitionState(PlayerState.Run);
            Debug.Log("To Run");
        }
        if (InputManager.Movement.x != 0 && !parameter._isJumping && !InputManager.RunIsHeld)
        {
            manager.TransitionState(PlayerState.Walk);
            Debug.Log("To Walk");
        }
        if (InputManager.AttackWasPressed)
        {
            manager.TransitionState(PlayerState.LightAttack);
            Debug.Log("To Light Attack");
        }
        if (InputManager.JumpWasPressed && parameter._numberOfJumpsUsed == 1 && parameter._isJumping)
        {
            manager.TransitionState(PlayerState.Jump);
            Debug.Log("To Jump");
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        Debug.Log("Exit Jump");
    }
}

public class PlayerLightAttackState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerLightAttackState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter._animator.Play("LightAttack");
        Debug.Log("Light Attack");
    }

    public void OnUpdate()
    {
        AnimatorStateInfo info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        float progress = info.normalizedTime;

        if (progress >= 0.95f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
    }

    public void OnFixedUpdate()
    {
        AnimatorStateInfo info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        float progress = info.normalizedTime;

        if (0.3f < progress && progress < 0.6f)
        {
            
        }
    }

    public void OnExit()
    {
        
    }
}

public class PlayerHeavyAttackState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerHeavyAttackState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter._animator.Play("LightAttack02");
    }

    public void OnUpdate()
    {
        AnimatorStateInfo info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        float progress = info.normalizedTime;
        //Debug.Log(progress);
        if (progress >= 0.95f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
    }

    public void OnFixedUpdate()
    {
        AnimatorStateInfo info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        float progress = info.normalizedTime;

        if (0.3f < progress && progress < 0.6f)
        {
            
        }
    }

    public void OnExit()
    {
        
    }
}

public class PlayerSpecialAttackState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerSpecialAttackState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        
    }
}

public class PlayerHitState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerHitState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        
    }
}

public class PlayerDeadState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;

    public PlayerDeadState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        
    }
}


