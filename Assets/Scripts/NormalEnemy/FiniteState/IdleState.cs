using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IdleState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;

    private float timer;

    public IdleState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("Idle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;

        if (parameter.getHit)
        {
            manager.TransitionState(normalEnemyStateType.Hit);
        }
        if (parameter.target != null && parameter.target.position.x >= parameter.chasePoints[0].position.x && parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(normalEnemyStateType.React);
        }
        if (timer >= parameter.idleTime)
        {
            manager.TransitionState(normalEnemyStateType.Patrol);
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        timer = 0;
    }
}

public class PatrolState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;

    private int patrolPosition;

    public PatrolState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("Walk");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position, parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.deltaTime);
        /*
        if (parameter.getHit)
        {
            manager.TransitionState(normalEnemyStateType.Hit);
        }
        */
        if (parameter.target != null && parameter.target.position.x >= parameter.chasePoints[0].position.x && parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(normalEnemyStateType.React);
        }
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < 0.5f)
        {
            manager.TransitionState(normalEnemyStateType.Idle);
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        patrolPosition++;

        if (patrolPosition >= parameter.patrolPoints.Length)
        {
            patrolPosition = 0;
        }
    }
}

public class ChaseState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;

    public ChaseState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("Walk");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.target);
        /*
        if (parameter.getHit)
        {
            manager.TransitionState(normalEnemyStateType.Hit);
        }
        */
        if (parameter.target != null && !Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.transform.position = Vector2.MoveTowards(manager.transform.position, parameter.target.position, parameter.chaseSpeed * Time.deltaTime);
        }
        else if (parameter.target == null || manager.transform.position.x <= parameter.chasePoints[0].position.x || manager.transform.position.x >= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(normalEnemyStateType.Idle);
        }
        else if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.TransitionState(normalEnemyStateType.Attack);
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        
    }
}

public class ReactState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public ReactState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("React");
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.getHit)
        {
            manager.TransitionState(normalEnemyStateType.Hit);
        }
        if (info.normalizedTime >= 0.95f)
        {
            manager.TransitionState(normalEnemyStateType.Chase);
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        
    }
}

public class AttackState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;
    private bool hasAttacked;
    private AnimatorStateInfo info;
    private float progress;

    public AttackState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("Attack");
        hasAttacked = false;
        parameter.damage = 10;
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        progress = info.normalizedTime % 1f;
        /*
        if (parameter.getHit)
        {
            manager.TransitionState(normalEnemyStateType.Hit);
        }
        */
        if (info.normalizedTime >= 0.95f)
        {
            manager.TransitionState(normalEnemyStateType.Chase);
        }
    }

    public void OnFixedUpdate()
    {

        if (progress >= 0.5f && progress <= 0.9f && !hasAttacked)
        {
            hasAttacked = true;
            parameter.attackHitBox.enabled = true;

            // 检测攻击范围内的玩家
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(
                parameter.attackPoint.position,
                parameter.attackArea,
                LayerMask.GetMask("Player")
            );

            foreach (Collider2D playerCollider in hitPlayers)
            {
                //Debug.Log("Enemy Attack Player");
                GameObject player = playerCollider.gameObject;
                // 触发敌人攻击事件
                CombatSystem.TriggerEnemyAttack(manager.gameObject, player, parameter.damage);
            }
        }

        if (progress >= 0.95f)
        {
            parameter.attackHitBox.enabled = false;
            hasAttacked = false;
            manager.TransitionState(normalEnemyStateType.Chase);
        }
    }

    public void OnExit()
    {
        parameter.attackHitBox.enabled = false;
        hasAttacked = false;
    }
}

public class HitState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;
    private AnimatorStateInfo info;
    private float progress;
    private Sequence hitSequence;
    private float hitTimer;

    public HitState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        //Debug.Log("Enemy is Hit");
        Debug.Log("Enemy's Health: " + parameter.health);
        parameter.animator.Play("Hit");
        // parameter.health -= 10;
        parameter.getHit = true;
        
        // 创建受击效果序列
        hitSequence = DOTween.Sequence();
        
        // 添加顿帧效果
        Time.timeScale = 1f;
        hitSequence.AppendCallback(() => Time.timeScale = 0.1f)
                  .AppendInterval(0.025f)
                  .AppendCallback(() => Time.timeScale = 1f);
        
        // 添加震屏效果
        //Camera.main.transform.DOShakePosition(0.1f, 1f, 10, 90, false);
        
        // 添加受击闪烁
        parameter.spriteRenderer.DOColor(Color.red, 0.1f)
                               .SetLoops(2, LoopType.Yoyo);
    }

    public void OnUpdate()
    {
        hitTimer += Time.deltaTime;

        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        progress = info.normalizedTime % 1f;

        if (parameter.health <= 0)
        {
            manager.TransitionState(normalEnemyStateType.Dead);
        }
        else if (info.normalizedTime >= 0.95f)
        {
            parameter.target = GameObject.FindWithTag("Player").transform;

            manager.TransitionState(normalEnemyStateType.Chase);
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnExit()
    {
        hitSequence?.Kill();
        parameter.spriteRenderer.color = Color.white;
        parameter.getHit = false;
    }
}

public class DeadState : MonoBehaviour, IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;
    private Sequence hitSequence;

    public DeadState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("Dead");

        // hitSequence = DOTween.Sequence();
        // // 添加顿帧效果
        // hitSequence.AppendCallback(() => Time.timeScale = 0.1f)
        //           .AppendInterval(0.025f)
        //           .AppendCallback(() => Time.timeScale = 1f);

        // parameter.spriteRenderer.DOColor(Color.red, 0.1f)
        //                        .SetLoops(2, LoopType.Yoyo);

        Destroy(parameter.EnemyObject, 1.5f);
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