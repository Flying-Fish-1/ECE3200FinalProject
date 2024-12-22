using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

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
        parameter._animator.Play("idle");
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
        if (InputManager.SpecialAttackWasPressed && parameter.energy >= parameter.maxEnergy/2)
        {
            manager.TransitionState(PlayerState.SpecialAttack);
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
        if (InputManager.SpecialAttackWasPressed && parameter.energy >= parameter.maxEnergy/2)
        {
            manager.TransitionState(PlayerState.SpecialAttack);
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
        if (InputManager.SpecialAttackWasPressed && parameter.energy >= parameter.maxEnergy/2)
        {
            manager.TransitionState(PlayerState.SpecialAttack);
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

        if (progress >= 1f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
    }

    public void OnFixedUpdate()
    {
        // 攻击判定窗口
        if (progress >= 0.3f && progress <= 0.5f)
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
                    if (parameter.energy < parameter.maxEnergy) parameter.energy += 5;
                }
            }
        }

        else if(progress >= 0.8f && progress < 0.95f)
        {
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
                    if (parameter.energy < parameter.maxEnergy) parameter.energy += 5;
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
            hasAttacked = false;
            enemiesHit.Clear();
        }

        // 动画结束，切换回待机状态
        if (progress >= 0.95f)
        {
            manager.TransitionState(PlayerState.Idle);
        }
        
    }

    public void OnExit()
    {
        parameter._LightAttackColl.enabled = false;
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
        parameter.isMoveable = false;

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
        if (progress >= 0.5f && progress <= 0.9f)
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
                    CombatSystem.TriggerPlayerAttack(manager.gameObject, enemy, parameter.heavyDamage * ((parameter.energy * 3)/parameter.maxEnergy + 1));
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
        parameter._HeavyAttackColl.enabled = false;
        parameter.isMoveable = true;
        parameter.currentDamage = 0;
        enemiesHit.Clear();
    }
}

public class PlayerSpecialAttackState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;
    AnimatorStateInfo info;
    float progress;
    private bool hasAttacked;
    private HashSet<GameObject> enemiesHit;
    

    public PlayerSpecialAttackState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
        enemiesHit = new HashSet<GameObject>();
    }

    public void OnEnter()
    {
        parameter.isMoveable = false;
        parameter._isDamageable = false;
        parameter.energy -= parameter.maxEnergy/2;

        parameter.currentDamage = parameter.specialDamage;
        parameter._animator.Play("ThrowSword");
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
        if (progress >= 0.3f && progress <= 0.9f)
        {
            // 只在第一次进入攻击窗口时启用攻击碰撞器
            if (!hasAttacked)
            {
                hasAttacked = true;
                parameter._SpecialAttackColl.enabled = true;
            }
            // parameter._SpecialAttackColl.enabled = true;

            // 检测攻击范围内的敌人
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
                parameter._SpecialAttackColl.bounds.center,
                parameter._SpecialAttackColl.bounds.size,
                0f,
                LayerMask.GetMask("Enemy")
            );

            // 检测攻击范围内的敌人攻击
            Collider2D[] hitAttack = Physics2D.OverlapBoxAll(
                parameter._SpecialAttackColl.bounds.center,
                parameter._SpecialAttackColl.bounds.size,
                0f,
                LayerMask.GetMask("EnemyAttack")
            );
            // Debug.Log(hitAttack);

            foreach (Collider2D attackCollider in hitAttack)
            {
                // Debug.Log("success");
                GameObject enemy = attackCollider.transform.parent.gameObject;
                // Debug.Log(enemy);
                if (!enemiesHit.Contains(enemy))
                {
                    enemiesHit.Add(enemy);
                    // 触发攻击事件
                    CombatSystem.TriggerPlayerAttack(manager.gameObject, enemy, parameter.specialDamage * 2);
                }
                // enemiesHit.Add(enemy);
                // CombatSystem.TriggerPlayerAttack(manager.gameObject, enemy, parameter.specialDamage);
            }

            // Debug.Log(3);
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                GameObject enemy = enemyCollider.gameObject;
                if (!enemiesHit.Contains(enemy))
                {
                    enemiesHit.Add(enemy);
                    // 触发攻击事件
                    CombatSystem.TriggerPlayerAttack(manager.gameObject, enemy, parameter.specialDamage);
                }
                // enemiesHit.Add(enemy);
                // CombatSystem.TriggerPlayerAttack(manager.gameObject, enemy, parameter.specialDamage);
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
        parameter._HeavyAttackColl.enabled = false;
        parameter.isMoveable = true;
        parameter._isDamageable = true;
        parameter.currentDamage = 0;
        enemiesHit.Clear();
    }
}

public class PlayerHitState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;
    AnimatorStateInfo info;
    float progress;
    private Sequence hitSequence;

    public PlayerHitState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.isMoveable = false;
        parameter._animator.Play("hurt");

        // 创建动画序列
        hitSequence = DOTween.Sequence();
        
        // 添加顿帧效果
        Time.timeScale = 1f;
        hitSequence.AppendCallback(() => Time.timeScale = 0.1f)
                  .AppendInterval(0.025f)
                  .AppendCallback(() => Time.timeScale = 1f);
                  
        // 添加震屏效果
        //Camera.main.DOShakePosition(0.2f, 0.5f, 10, 90, false);
        
        // 添加受击闪烁
        parameter.spriteRenderer.DOColor(Color.red, 0.1f)
                               .SetLoops(2, LoopType.Yoyo);
    }

    public void OnUpdate()
    {
        info = parameter._animator.GetCurrentAnimatorStateInfo(0);
        progress = info.normalizedTime;

        if (progress >= 0.95f)
        {
            parameter.isMoveable = true;
            manager.TransitionState(PlayerState.Idle);
            
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        parameter.isMoveable = true;
        hitSequence?.Kill();
        parameter.spriteRenderer.color = Color.white;
    }
}

public class PlayerDeadState : IState
{
    private PlayerController manager;
    private PlayerParameter parameter;
    // private GameObject dead_menu;

    public PlayerDeadState(PlayerController manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter()
    {
        Debug.Log("Player is dead");
        parameter.isMoveable = false;
        Scene currentScene = SceneManager.GetActiveScene(); // Get the current scene
        SceneManager.LoadScene(currentScene.name); // Reload the scene by its name
        Time.timeScale = 1f;
        // dead_menu = GameObject.Find("GameManager/Dead Menu");
        // dead_menu.SetActive(true);
        // parameter.isDead = true;
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


