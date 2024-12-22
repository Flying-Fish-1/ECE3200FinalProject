using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Jump,
    LightAttack,
    HeavyAttack,
    SpecialAttack,
    Hit,
    Dead
}

[Serializable]
public class PlayerParameter//PlayerController : MonoBehaviour//, IDamageable
    {
        [Header("References")]
        public PlayerMovementStats MoveStats;
        public Collider2D _feetColl;
        public Collider2D _bodyColl;
        public Collider2D _LightAttackColl;
        public Collider2D _HeavyAttackColl;
        public Collider2D _SpecialAttackColl;

        public Rigidbody2D _rb;
        public Animator _animator;
        //public Attack attack;
        //public HeavyAttack heavyAttack;
        public SpriteRenderer spriteRenderer;
        public AudioSource music;
        public AudioClip light;
        public AudioClip heavy;
        public AudioClip counter;
        public AudioClip special;
        //public Rigidbody2D rb;
        public int maxHealth = 100;
        public int health = 100;
        public int currentDamage = 0;
        public int lightDamage = 5;
        public int heavyDamage = 10;
        public int specialDamage = 100;
        public int energy = 0;
        public int maxEnergy = 100;
        //public bool isDamageable = true;

        // Animation variables
        //public bool IsMoving;
        //public bool IsJumping;
        //public bool IsLightAttacking;
        //public bool IsHeavyAttacking;
        //public bool IsHit;
        //public bool IsDead;
        public bool isMoveable = true;
        // public bool isDead = true;

        // Movement variables
        public Vector2 _moveVelocity;
        public bool _isFacingRight;

        // Collision check variables
        public RaycastHit2D _groundHit;
        public RaycastHit2D _headHit;
        public bool _isGrounded;
        public bool _bumpedHead;
        public bool _isDamageable = true;

        // Jump variables
        public float VerticalVelocity; //{ get; private set; }
        public bool _isJumping;
        public bool _isFastFalling;
        public bool _isFalling;
        public float _fastFallTime;
        public float _fastFallReleaseSpeed;
        public int _numberOfJumpsUsed;

        // Apex variables
        public float _apexPoint;
        public float _timePassedThreshold;
        public bool _isPastApexThreshold;

        // Jump buffer variables
        public float _jumpBufferTimer;
        public bool _jumpReleasedDuringBuffer;

        // Coyote time variables
        public float _coyoteTimer;

        public Transform ui_healthbar;
        public Transform ui_energybar;
        // public GameObject deadMenu;
    }

public class PlayerController : MonoBehaviour
{
    

    public IState currentState;
    public Dictionary<PlayerState, IState> states = new Dictionary<PlayerState, IState>();

    public PlayerParameter parameter;

    void Start()
    {
        /*
        if (parameter == null)
        {
            Debug.LogError("PlayerParameter 未在 Inspector 中赋值！");
            return;
        }
        */

        states.Add(PlayerState.Idle, new PlayerIdleState(this));
        states.Add(PlayerState.Walk, new PlayerWalkState(this));
        states.Add(PlayerState.Run, new PlayerRunState(this));
        states.Add(PlayerState.Jump, new PlayerJumpState(this));
        states.Add(PlayerState.LightAttack, new PlayerLightAttackState(this));
        states.Add(PlayerState.HeavyAttack, new PlayerHeavyAttackState(this));
        states.Add(PlayerState.SpecialAttack, new PlayerSpecialAttackState(this));
        states.Add(PlayerState.Hit, new PlayerHitState(this));
        states.Add(PlayerState.Dead, new PlayerDeadState(this));

        parameter.spriteRenderer = GetComponent<SpriteRenderer>();
        //parameter._rb = GetComponent<Rigidbody2D>();
        //parameter.attack = GetComponent<Attack>();
        //parameter.heavyAttack = GetComponent<HeavyAttack>();
        parameter.health = 100;

        parameter._LightAttackColl.enabled = false;
        parameter._HeavyAttackColl.enabled = false;
        parameter._SpecialAttackColl.enabled = false;
        
        TransitionState(PlayerState.Idle);

        // Debug.Log(GameObject.Find("HUD/Energy/Bar"));

        parameter.ui_healthbar = GameObject.Find("HUD/Health/Bar").transform;
        parameter.ui_energybar = GameObject.Find("HUD/Energy/Bar").transform;

        RefreshHealthBar();
        RefreshEnergyBar();
    }
    
    void Update()
    {
        CountTimers();
        if (parameter.isMoveable) JumpChecks();

        currentState.OnUpdate();

        RefreshHealthBar();
        RefreshEnergyBar();
    }

    void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if (parameter._isGrounded)
        {
            Move(parameter.MoveStats.GroundAcceleration, parameter.MoveStats.GroundDeceleration, InputManager.Movement);
        }
        if (!parameter._isGrounded)
        {
            Move(parameter.MoveStats.AirAcceleration, parameter.MoveStats.AirDeceleration, InputManager.Movement);
        }

        currentState.OnFixedUpdate();
    }

    public void TransitionState(PlayerState nextState)
    {
        // if (parameter.isDead) return;
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentState = states[nextState];
        currentState.OnEnter();

        // RefreshEnergyBar();
    }

    private void OnEnable()
    {
        CombatSystem.OnEnemyAttackHit += OnEnemyAttackHit;
        //Debug.Log("PlayerController is Enabled");
    }

    private void OnDisable()
    {
        CombatSystem.OnEnemyAttackHit -= OnEnemyAttackHit;
    }

    private void OnEnemyAttackHit(GameObject attacker, GameObject target, int damage)
    {
        Debug.Log("Player is Hit by" + target.name);
        Debug.Log(parameter._isDamageable);
        if (target.CompareTag("Player") && parameter._isDamageable)
        {
            //Debug.Log("Player is Hit triggered");
            // 处理玩家受击逻辑
            parameter.health -= damage;
            Debug.Log("Player's Health: " + parameter.health);

            // 判断玩家是否死亡
            if (parameter.health <= 0)
            {
                TransitionState(PlayerState.Dead);
            }
            else
            {
                TransitionState(PlayerState.Hit);
            }

            RefreshHealthBar();
        }
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        // print("move ismoveable");
        // print(isMoveable);
        
        if (!parameter.isMoveable)
        {
            moveInput = Vector2.zero;
        }
        
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.RunIsHeld)
            {
                //IsMoving = true;
                targetVelocity = new Vector2(moveInput.x, 0f) * parameter.MoveStats.MaxRunSpeed;
            }
            else
            {
                //parameter.IsMoving = true;
                targetVelocity = new Vector2(moveInput.x, 0f) * parameter.MoveStats.MaxWalkSpeed;
            }

            parameter._moveVelocity = Vector2.Lerp(parameter._moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            parameter._rb.velocity = new Vector2(parameter._moveVelocity.x, parameter._rb.velocity.y);
        }

        else if (moveInput == Vector2.zero)
        {
            //parameter.IsMoving = false;
            parameter._moveVelocity = Vector2.Lerp(parameter._moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            parameter._rb.velocity = new Vector2(parameter._moveVelocity.x, parameter._rb.velocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (parameter._isFacingRight && moveInput.x > 0)
        {
            Turn(false);
        }
        else if (!parameter._isFacingRight && moveInput.x < 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            parameter._isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            parameter._isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion

    #region Jump

    private void JumpChecks()
    {
        // WHEN WE PRESSED JUMP BUTTON
        if (InputManager.JumpWasPressed)
        {
            parameter._jumpBufferTimer = parameter.MoveStats.JumpBufferTime;
            parameter._jumpReleasedDuringBuffer = false;
        }

        // WHEN WE RELEASE JUMP BUTTON
        if (InputManager.JumpWasReleased)
        {
            if (parameter._jumpBufferTimer > 0f)
            {
                parameter._jumpReleasedDuringBuffer = true;
            }

            if (parameter._isJumping && parameter.VerticalVelocity > 0f)
            {
                if (parameter._isPastApexThreshold)
                {
                    parameter._isPastApexThreshold = false;
                    parameter._isFastFalling = true;
                    parameter._fastFallTime = parameter.MoveStats.TimeForUpwardCancel;
                    parameter.VerticalVelocity = 0f;
                }
                else
                {
                    parameter._isFastFalling = true;
                    parameter._fastFallReleaseSpeed = parameter.VerticalVelocity;
                }
            }
        }

        // INITIATE WITH JUMP BUFFERING AND COYOTE TIME
        if (parameter._jumpBufferTimer > 0f && !parameter._isJumping && (parameter._isGrounded || parameter._coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (parameter._jumpReleasedDuringBuffer)
            {
                parameter._isFastFalling = true;
                parameter._fastFallReleaseSpeed = parameter.VerticalVelocity;
            }
        }

        // DOUBLE JUMP
        else if (parameter._jumpBufferTimer > 0f && parameter._isJumping && parameter._numberOfJumpsUsed < parameter.MoveStats.NumberOfJumpsAllowed)
        {
            parameter._isFastFalling = false;
            InitiateJump(1);
        }

        // AIR JUMP AFTER COYOTE TIME LAPSED
        else if (parameter._jumpBufferTimer > 0f && parameter._isFalling && parameter._numberOfJumpsUsed < parameter.MoveStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            parameter._isFastFalling = false;
        }

        // LANDED
        if ((parameter._isJumping || parameter._isFalling) && parameter._isGrounded && parameter.VerticalVelocity <= 0f)
        {
            parameter._isJumping = false;
            parameter._isFalling = false;
            parameter._isFastFalling = false;
            parameter._fastFallTime = 0f;
            parameter._isPastApexThreshold = false;
            parameter._numberOfJumpsUsed = 0;

            parameter.VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!parameter._isJumping)
        {
            parameter._isJumping = true;
        }

        parameter._jumpBufferTimer = 0f;
        parameter._numberOfJumpsUsed += numberOfJumpsUsed;
        parameter.VerticalVelocity = parameter.MoveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        // APPLY GRAVITY WHILE JUMPING
        if (parameter._isJumping)
        {
            // CHECK FOR HEAD BUMP
            if (parameter._bumpedHead)
            {
                parameter._isFastFalling = true;
            }
            
            // GRAVITY ON ASCENDING
            if (parameter.VerticalVelocity >= 0f)
            {
                // APEX CONTROLS
                parameter._apexPoint = Mathf.InverseLerp(parameter.MoveStats.InitialJumpVelocity, 0f, parameter.VerticalVelocity);

                if (parameter._apexPoint >= parameter.MoveStats.ApexThreshold)
                {
                    if (!parameter._isPastApexThreshold)
                    {
                        parameter._isPastApexThreshold = true;
                        parameter._timePassedThreshold = 0f;
                    }

                    if (parameter._isPastApexThreshold)
                    {
                        parameter._timePassedThreshold += Time.fixedDeltaTime;
                        if (parameter._timePassedThreshold < parameter.MoveStats.ApexHangTime)
                        {
                            parameter.VerticalVelocity = 0f;
                        }

                        else
                        {
                            parameter.VerticalVelocity = -0.01f;
                        }
                    }
                }

                // GRAVITY ON DESCENDING BUT NOT PASS APEX THRESHOLD
                else
                {
                    parameter.VerticalVelocity += parameter.MoveStats.Gravity * Time.fixedDeltaTime;
                    if (parameter._isPastApexThreshold)
                    {
                        parameter._isPastApexThreshold = false;
                    }
                }
            }

            // GRAVITY ON DESCENDING
            else if (!parameter._isFastFalling)
            {
                parameter.VerticalVelocity += parameter.MoveStats.Gravity * parameter.MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (parameter.VerticalVelocity < 0f)
            {
                if (!parameter._isFalling)
                {
                    parameter._isFalling = true;
                }
            }
        }

        // JUMP CUT
        if (parameter._isFastFalling)
        {
            if (parameter._fastFallTime >= parameter.MoveStats.TimeForUpwardCancel)
            {
                parameter.VerticalVelocity += parameter.MoveStats.Gravity * parameter.MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (parameter._fastFallTime < parameter.MoveStats.TimeForUpwardCancel)
            {
                parameter.VerticalVelocity = Mathf.Lerp(parameter._fastFallReleaseSpeed, 0f, parameter._fastFallTime / parameter.MoveStats.TimeForUpwardCancel);
            }
            
            parameter._fastFallTime += Time.fixedDeltaTime;
        }

        // NORMAL GRAVITY WHILE FALLING
        if (!parameter._isGrounded && !parameter._isJumping)
        {
            if (!parameter._isFalling)
            {
                parameter._isFalling = true;
            }

            parameter.VerticalVelocity += parameter.MoveStats.Gravity * Time.fixedDeltaTime;
        }

        // CLAMP FALL SPEED
        parameter.VerticalVelocity = Mathf.Clamp(parameter.VerticalVelocity, -parameter.MoveStats.MaxFallSpeed, 50f);

        parameter._rb.velocity = new Vector2(parameter._rb.velocity.x, parameter.VerticalVelocity); 

    }

    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(parameter._feetColl.bounds.center.x, parameter._feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(parameter._feetColl.bounds.size.x, parameter.MoveStats.GroundDetectionRayLength);

        parameter._groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, parameter.MoveStats.GroundDetectionRayLength, parameter.MoveStats.GroundLayer);
        if (parameter._groundHit.collider != null)
        {
            parameter._isGrounded = true;
        }
        else
        {
            parameter._isGrounded = false;
        }

        #region Debug Visualization
        
        if (parameter.MoveStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (parameter._isGrounded)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * parameter.MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * parameter.MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - parameter.MoveStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }

        #endregion

    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(parameter._feetColl.bounds.center.x, parameter._bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(parameter._feetColl.bounds.size.x * parameter.MoveStats.HeadWidth, parameter.MoveStats.HeadDetectionRayLength);

        parameter._headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, parameter.MoveStats.HeadDetectionRayLength, parameter.MoveStats.GroundLayer);
        if (parameter._headHit.collider != null)
        {
            parameter._bumpedHead = true;
        }
        else
        {
            parameter._bumpedHead = false;
        }

        #region Debug Visualization

        if (parameter.MoveStats.DebugShowIsHeadBumpBox)
        {
            float headWidth = parameter.MoveStats.HeadWidth;
            Color rayColor;
            if (parameter._bumpedHead)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * parameter.MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * parameter.MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + parameter.MoveStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }

        #endregion
    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }

    #endregion

    #region Timers

    private void CountTimers()
    {
        parameter._jumpBufferTimer -= Time.deltaTime;

        if (!parameter._isGrounded)
        {
            parameter._coyoteTimer -= Time.deltaTime;
        }
        else 
        {
            parameter._coyoteTimer = parameter.MoveStats.JumpCoyoteTime;
        }
    }

    #endregion

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(parameter._feetColl.bounds.center.x, parameter._feetColl.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if (parameter.MoveStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else
        {
            speed = -moveSpeed;
        }
        Vector2 velocity = new Vector2(speed, parameter.MoveStats.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * parameter.MoveStats.TimeTillJumpApex / parameter.MoveStats.ArcResolution;

        for (int i = 0; i < parameter.MoveStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < parameter.MoveStats.TimeTillJumpApex)
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, parameter.MoveStats.Gravity) * Mathf.Pow(simulationTime, 2);
            }

            else if (simulationTime < parameter.MoveStats.TimeTillJumpApex + parameter.MoveStats.ApexHangTime)
            {
                float apexTime = simulationTime - parameter.MoveStats.TimeTillJumpApex;
                displacement = velocity * parameter.MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, parameter.MoveStats.Gravity) * Mathf.Pow(parameter.MoveStats.TimeTillJumpApex, 2);
                displacement += new Vector2(speed, 0) * apexTime;
            }

            else
            {
                float descendTime = simulationTime - parameter.MoveStats.TimeTillJumpApex - parameter.MoveStats.ApexHangTime;
                displacement = velocity * parameter.MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, parameter.MoveStats.Gravity) * Mathf.Pow(parameter.MoveStats.TimeTillJumpApex, 2);
                displacement += new Vector2(speed, 0) * parameter.MoveStats.ApexHangTime;
                displacement += velocity * descendTime + 0.5f * new Vector2(0, parameter.MoveStats.Gravity) * Mathf.Pow(descendTime, 2);
            }

            drawPoint = startPosition + displacement;

            if (parameter.MoveStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), parameter.MoveStats.GroundLayer);
                if (hit.collider != null)
                {
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }

    void RefreshHealthBar()
    {
        float t_health_ratio = (float)parameter.health/(float)parameter.maxHealth;
        if (t_health_ratio < 0) t_health_ratio = 0;
        parameter.ui_healthbar.localScale = Vector3.Lerp(parameter.ui_healthbar.localScale, new Vector3(t_health_ratio, 1, 1), Time.deltaTime * 8f);
    }

    void RefreshEnergyBar()
    {
        float t_energy_ratio = (float)parameter.energy/(float)parameter.maxEnergy;
        if (t_energy_ratio < 0) t_energy_ratio = 0;
        parameter.ui_energybar.localScale = Vector3.Lerp(parameter.ui_energybar.localScale, new Vector3(t_energy_ratio, 1, 1), Time.deltaTime * 8f);
    }
        
}
