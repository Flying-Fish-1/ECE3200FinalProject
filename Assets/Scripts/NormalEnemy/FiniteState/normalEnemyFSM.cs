using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum normalEnemyStateType
{
    Idle,
    Patrol,
    Chase,
    React,
    Attack,
    Hit,
    Dead
}

[Serializable]
public class Parameter
{
    public int health;
    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;
    public float attackArea;
    public Transform[] patrolPoints;
    public Transform[] chasePoints;
    public Transform target;
    public Transform attackPoint;
    public LayerMask targetLayer;
    public Animator animator;
    public bool getHit;
}

public class normalEnemyFSM : MonoBehaviour
{
    public Parameter parameter;

    private IState currentState;
    private Dictionary<normalEnemyStateType, IState> states = new Dictionary<normalEnemyStateType, IState>();
    
    void Start()
    {
        states.Add(normalEnemyStateType.Idle, new IdleState(this));
        states.Add(normalEnemyStateType.Patrol, new PatrolState(this));
        states.Add(normalEnemyStateType.Chase, new ChaseState(this));
        states.Add(normalEnemyStateType.React, new ReactState(this));
        states.Add(normalEnemyStateType.Attack, new AttackState(this));
        states.Add(normalEnemyStateType.Hit, new HitState(this));
        states.Add(normalEnemyStateType.Dead, new DeadState(this));

        TransitionState(normalEnemyStateType.Idle);

        parameter.animator = GetComponent<Animator>();
    }

    void Update()
    {
        currentState.OnUpdate();

        //if ())
        //{
        //    parameter.getHit = true;
        //}
    }

    public void TransitionState(normalEnemyStateType type)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        
        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }
}
