using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (parameter.getHit)
        {
            manager.TransitionState(normalEnemyStateType.Hit);
        }
        if (parameter.target != null && parameter.target.position.x >= parameter.chasePoints[0].position.x && parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(normalEnemyStateType.React);
        }
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < 0.5f)
        {
            manager.TransitionState(normalEnemyStateType.Idle);
        }
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

        if (parameter.getHit)
        {
            manager.TransitionState(normalEnemyStateType.Hit);
        }
        if (parameter.target != null)
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

    public void OnExit()
    {
        
    }
}

public class AttackState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public AttackState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("Attack");
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

    public void OnExit()
    {
        
    }
}

public class HitState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public HitState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("Hit");
        parameter.health -= 10;
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.health <= 0)
        {
            manager.TransitionState(normalEnemyStateType.Dead);
        }
        else
        {
            parameter.target = GameObject.FindWithTag("Player").transform;

            manager.TransitionState(normalEnemyStateType.Chase);
        }
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }
}

public class DeadState : IState
{
    private normalEnemyFSM manager;
    private Parameter parameter;

    public DeadState(normalEnemyFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    
    public void OnEnter()
    {
        parameter.animator.Play("Dead");
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}