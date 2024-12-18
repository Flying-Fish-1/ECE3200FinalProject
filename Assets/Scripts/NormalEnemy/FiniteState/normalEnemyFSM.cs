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
    public int damage = 10;
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
    public Rigidbody2D rb;
    public bool getHit;
}

public class normalEnemyFSM : MonoBehaviour//, IDamageable
{
    public Parameter parameter;

    private IState currentState;
    private Dictionary<normalEnemyStateType, IState> states = new Dictionary<normalEnemyStateType, IState>();
    //private bool isDamageable = true;

    
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

    }

    void FixedUpdate()
    {
        currentState.OnFixedUpdate();
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

    private void OnEnable()
    {
        CombatSystem.OnPlayerAttackHit += OnPlayerAttackHit;
    }

    private void OnDisable()
    {
        CombatSystem.OnPlayerAttackHit -= OnPlayerAttackHit;
    }

    private void OnPlayerAttackHit(GameObject attacker, GameObject target, int damage)
    {
        if (target == gameObject)
        {
            // 处理敌人受击逻辑
            parameter.health -= damage;
            parameter.getHit = true;

            // 判断敌人是否死亡
            if (parameter.health <= 0)
            {
                TransitionState(normalEnemyStateType.Dead);
            }
            else
            {
                TransitionState(normalEnemyStateType.Hit);
            }
        }
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
            //Debug.Log("Player is viewed");
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

    /*
    public void OnHit(int damage, Vector2 knockback)
    {
        print("damageable");
        print(isDamageable);
        if (isDamageable)
        {
            isDamageable = false;
            // print(parameter.health);
            parameter.health -= damage;
            parameter.rb.AddForce(knockback);
            // print(parameter.health);
            // print(damage);
            if (parameter.health <= 0)
            {
                Defeated();
            }
            else
            {
                Hit();
            }

            Invoke("resetIsDamageable", 0.5f);
        }
    }

    public void Defeated()
    {
        TransitionState(normalEnemyStateType.Dead);

        // Invoke("RemoveEnemy", 2f);
    }

    public void Hit()
    {
        TransitionState(normalEnemyStateType.Hit);
    }

    public void RemoveEnemy()
    {
        Destroy(gameObject);
    }

    public void resetIsDamageable()
    {
        isDamageable = true;
    }
    */
}

