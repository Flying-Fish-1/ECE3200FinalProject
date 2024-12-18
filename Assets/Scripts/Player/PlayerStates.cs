using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            //Debug.Log("To Idle");
            manager.TransitionState(PlayerState.Idle);
        }
        if (InputManager.RunIsHeld && !parameter._isJumping)
        {
            manager.TransitionState(PlayerState.Run);
            //Debug.Log("To Run");
        }
        if (InputManager.Movement.x != 0 && !parameter._isJumping && !InputManager.RunIsHeld)
        {
            manager.TransitionState(PlayerState.Walk);
            //Debug.Log("To Walk");
        }
        if (InputManager.AttackWasPressed)
        {
            manager.TransitionState(PlayerState.LightAttack);
            //Debug.Log("To Light Attack");
        }
        if (InputManager.JumpWasPressed && parameter._numberOfJumpsUsed == 1 && parameter._isJumping)
        {
            manager.TransitionState(PlayerState.Jump);
            //Debug.Log("To Jump");
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        //Debug.Log("Exit Jump");
    }
}

public class PlayerLightAttackState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;
    private bool hasAttacked;
    AnimatorStateInfo info;
    float progress;

    // 用于记录已被击中的敌人
    private HashSet<GameObject> enemiesHit;

    public PlayerLightAttackState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
        enemiesHit = new HashSet<GameObject>();
    }
    

    public void OnEnter()
    {
        parameter.currentDamage = parameter.lightDamage; 
        parameter._animator.Play("LightAttack");
        //Debug.Log("Light Attack");
        hasAttacked = false;
        enemiesHit.Clear(); // 进入状态时清空已击中敌人列表
    }

    public void OnUpdate()
    {
        info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        progress = info.normalizedTime;

        if (progress >= 0.95f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
    }

    public void OnFixedUpdate()
    {
        // 攻击判定窗口
        if (progress >= 0.3f && progress <= 0.6f)
        {
            // 只在第一次进入攻击窗口时启用攻击碰撞器
            if (!hasAttacked)
            {
                hasAttacked = true;
                parameter._LightAttackColl.enabled = true;
            }

            // 检测攻击范围内的敌人
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
                parameter._LightAttackColl.bounds.center,
                parameter._LightAttackColl.bounds.size,
                0f,
                LayerMask.GetMask("Enemy")
            );

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                GameObject enemy = enemyCollider.gameObject;
                if (!enemiesHit.Contains(enemy))
                {
                    enemiesHit.Add(enemy);
                    // 触发攻击事件
                    CombatSystem.TriggerPlayerAttack(manager.gameObject, enemy, parameter.lightDamage);
                }
            }
        }
        else
        {
            // 超出攻击窗口，禁用攻击碰撞器
            if (parameter._LightAttackColl.enabled)
            {
                parameter._LightAttackColl.enabled = false;
            }
        }

        // 动画结束，切换回待机状态
        if (progress >= 0.95f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
        
    }

    public void OnExit()
    {
        parameter.currentDamage = 0;
        enemiesHit.Clear();
    }
}

public class PlayerHeavyAttackState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;
    private bool hasAttacked;
    AnimatorStateInfo info;
    float progress;

    // 用于记录已被击中的敌人
    private HashSet<GameObject> enemiesHit;

    public PlayerHeavyAttackState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
        enemiesHit = new HashSet<GameObject>();
    }

    public void OnEnter()
    {
        parameter.currentDamage = parameter.heavyDamage;
        parameter._animator.Play("LightAttack02");
        enemiesHit.Clear(); // 进入状态时清空已击中敌人列表
    }

    public void OnUpdate()
    {
        info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        progress = info.normalizedTime;
        //Debug.Log(progress);

        if (progress >= 0.95f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
    }

    public void OnFixedUpdate()
    {
        // 攻击判定窗口
        if (progress >= 0.3f && progress <= 0.6f)
        {
            // 只在第一次进入攻击窗口时启用攻击碰撞器
            if (!hasAttacked)
            {
                hasAttacked = true;
                parameter._HeavyAttackColl.enabled = true;
            }

            // 检测攻击范围内的敌人
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
                parameter._HeavyAttackColl.bounds.center,
                parameter._HeavyAttackColl.bounds.size,
                0f,
                LayerMask.GetMask("Enemy")
            );

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                GameObject enemy = enemyCollider.gameObject;
                if (!enemiesHit.Contains(enemy))
                {
                    enemiesHit.Add(enemy);
                    // 触发攻击事件
                    CombatSystem.TriggerPlayerAttack(manager.gameObject, enemy, parameter.heavyDamage);
                }
            }
        }
        else
        {
            // 超出攻击窗口，禁用攻击碰撞器
            if (parameter._HeavyAttackColl.enabled)
            {
                parameter._HeavyAttackColl.enabled = false;
            }
        }

        // 动画结束，切换回待机状态
        if (progress >= 0.95f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
    }

    public void OnExit()
    {
        parameter.currentDamage = 0;
        enemiesHit.Clear();
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
    AnimatorStateInfo info;
    float progress;

    public PlayerHitState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        Debug.Log("Player is hit");
    }

    public void OnUpdate()
    {
        info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        progress = info.normalizedTime;

        if (progress >= 0.95f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
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
        Debug.Log("Player is dead");
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


