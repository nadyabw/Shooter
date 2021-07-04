using System.Collections.Generic;
using UnityEngine;

public class Zombie : BaseUnit
{
    protected enum State {Idle, ReturnToStartPosition, Patrol, Pursuit, RagePursuit, Attack, Dead }

    #region Variables

    [Header("Vision")]
    [Range(30, 360)]
    [SerializeField] private float visionAngle;
    [SerializeField] private LayerMask obstaclesMask;

    [Header("Base settings")]
    [SerializeField] private float pursuitRadius = 14f;
    [SerializeField] private float targetDetectionVisionRadius = 10f;
    [SerializeField] private float targetDetectionAnywayRadius = 6f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float rageTime = 5f;

    [Header("Attack")]
    [SerializeField] private float damageAmount = 2f;
    [Range(1, 5)]
    [SerializeField] private int targetUpdateDelayFactor;

    [Header("Animation")]
    [SerializeField] private ZombieAnimationHandler animationHandler;

    [Header("Movement")]
    [SerializeField] protected BaseEnemyMovement zombieMovement;
    [SerializeField] private Transform patrolPointsTransform;

    private Player player;
    private Transform playerTransform;

    private Vector3 targetPosition;
    private Vector3 startPosition;

    protected State currentState;
    private float currentRageTime;

    private bool isPatrolRole;
    private int currentPatrolPositionIndex;
    private List<Vector3> patrolPositions = new List<Vector3>();

    #endregion

    #region Unity lifecycle

    private void OnEnable()
    {
        Player.OnDied += HandlePlayerDeath;

        // отслеживаем смерть босса
        ZombieBoss.OnDied += HandleBossDeath;

        animationHandler.OnAttackPerformed += DoDamage;
    }

    private void OnDisable()
    {
        Player.OnDied -= HandlePlayerDeath;
        ZombieBoss.OnDied -= HandleBossDeath;
        animationHandler.OnAttackPerformed -= DoDamage;
    }

    protected override void Start()
    {
        base.Start();

        player = Player.Instance;
        playerTransform = player.transform;

        Init();
    }

    protected virtual void Update()
    {
        if ((currentState != State.Dead) && !player.IsDied)
            CheckState();

        UpdateCurrentState();
    }

    #endregion

    #region Private methods

    private void Init()
    {
        for (int i = 0; i < patrolPointsTransform.childCount; i++)
        {
            patrolPositions.Add(patrolPointsTransform.GetChild(i).position);
        }
        patrolPointsTransform.gameObject.SetActive(false);

        if(patrolPositions.Count >= 2)
        {
            isPatrolRole = true;
            SetState(State.Patrol);
        }
        else
        {
            targetPosition = startPosition = cachedTransform.position;
            zombieMovement.SetTargetPosition(targetPosition);
            SetState(State.Idle);
        }
    }

    private void CheckState()
    {
        float distance = Vector3.Distance(playerTransform.position, cachedTransform.position);

        if (distance < attackRadius)
        {
            SetState(State.Attack);
        }
        else if ((currentState != State.RagePursuit) && (distance < targetDetectionVisionRadius) && (IsPlayerVisible()))
        {
            SetState(State.Pursuit);
        }
        else if ((currentState != State.RagePursuit) && (distance < targetDetectionAnywayRadius))
        {
            SetState(State.Pursuit);
        }
        else if ((currentState != State.RagePursuit) && (distance > pursuitRadius))
        {
            if (currentState == State.Pursuit && isPatrolRole)
            {
                SetState(State.Patrol);
            }
            else if(currentState == State.Pursuit && !isPatrolRole)
            {
                SetState(State.ReturnToStartPosition);
            }
        }
    }

    protected void SetState(State state)
    {
        switch (state)
        {
            case State.Idle:
                zombieMovement.StopMovement();
                break;
            case State.ReturnToStartPosition:
                zombieMovement.StartMovement(0.8f);
                targetPosition = startPosition;
                zombieMovement.SetTargetPosition(targetPosition);
                break;
            case State.Patrol:
                zombieMovement.StartMovement(0.5f);
                targetPosition = patrolPositions[currentPatrolPositionIndex];
                zombieMovement.SetTargetPosition(targetPosition);
                break;
            case State.Pursuit:
                zombieMovement.StartMovement();
                break;
            case State.RagePursuit:
                currentRageTime = rageTime;
                zombieMovement.StartMovement(1.1f);
                break;
            case State.Attack:
                zombieMovement.StopMovement();
                break;
            case State.Dead:
                zombieMovement.StopMovement();
                zombieMovement.enabled = false;
                break;
        }

        currentState = state;
    }

    private void UpdateCurrentState()
    {
        switch (currentState)
        {
            case State.ReturnToStartPosition:
                UpdateReturnToStartPosition();
                break;
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Pursuit:
                UpdatePursuit();
                break;
            case State.RagePursuit:
                UpdateRagePursuit();
                break;
            case State.Attack:
                UpdateAttack();
                break;
            case State.Dead:
                UpdateDead();
                break;
        }
    }

    protected override void Attack()
    {
        PlayAttackAnimation();
    }

    private void DoDamage()
    {
        player.HandleDamage(damageAmount);
    }

    private void UpdatePursuit()
    {
        CheckTargetPosition();
        zombieMovement.SetTargetPosition(targetPosition);
    }

    private void UpdateRagePursuit()
    {
        CheckTargetPosition();
        zombieMovement.SetTargetPosition(targetPosition);

        currentRageTime -= Time.deltaTime;
        if (currentRageTime <= 0)
            SetState(State.Pursuit);
    }

    private void UpdatePatrol()
    {
        if (Vector3.Distance(cachedTransform.position, patrolPositions[currentPatrolPositionIndex]) <= distanceCompareDelta)
        {
            currentPatrolPositionIndex++;
            if (currentPatrolPositionIndex == patrolPositions.Count)
                currentPatrolPositionIndex = 0;

            targetPosition = patrolPositions[currentPatrolPositionIndex];
            zombieMovement.SetTargetPosition(targetPosition);
        }
    }

    private void UpdateReturnToStartPosition()
    {
        if (Vector3.Distance(cachedTransform.position, startPosition) <= distanceCompareDelta)
        {
            SetState(State.Idle);
        }
    }

    protected virtual void UpdateDead()
    {
    }

    private void HandlePlayerDeath()
    {
        if ((currentState == State.Dead) || (currentState == State.Idle))
            return;

        if (isPatrolRole)
            SetState(State.Patrol);
        else
            SetState(State.ReturnToStartPosition);
    }

    // если босса убили, то все ещё живые зомби тоже умирают )
    private void HandleBossDeath()
    {
        if (currentState == State.Dead)
            return;

        Die();
    }

    protected bool IsPlayerVisible()
    {
        Vector3 bodyDir = -bodyTransform.up;
        Vector3 dirToPlayer = playerTransform.position - cachedTransform.position;
        float angleToPlayer = Vector3.Angle(dirToPlayer, bodyDir);

        if (angleToPlayer > (visionAngle / 2))
            return false;

        RaycastHit2D rHit = Physics2D.Raycast(cachedTransform.position, dirToPlayer, targetDetectionVisionRadius, obstaclesMask);
        if (rHit.collider != null)
        {
            return false;
        }

        return true;
    }

    /*protected bool IsTargetAccesible(Vector3 targetPos)
    {
        Vector3 dir = targetPos - cachedTransform.position;
        float distance = dir.magnitude;

        RaycastHit2D rHit = Physics2D.Raycast(cachedTransform.position, dir, distance, obstaclesMask);
        if (rHit.collider != null)
        {
            return false;
        }

        return true;
    }*/

    private void CheckTargetPosition()
    {
        targetPosition += (playerTransform.position - targetPosition) / (targetUpdateDelayFactor * 10f);
    }

    protected override void Die()
    {
        base.Die();
        SetState(State.Dead);
    }

    protected virtual void PlayAttackAnimation()
    {
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Shoot));
    }

    private void OnDrawGizmos()
    {
        if (currentState == State.Dead)
            return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, targetDetectionVisionRadius);

        Gizmos.color = new Color(1f, 0.6f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, targetDetectionAnywayRadius);

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);

        // VISION ANGLE
        Gizmos.color = Color.red;
        Vector3 dir = -bodyTransform.up;
        Quaternion rotLeft = Quaternion.AngleAxis(-visionAngle / 2, bodyTransform.forward);
        Quaternion rotRight = Quaternion.AngleAxis(visionAngle / 2, bodyTransform.forward);
        Vector3 rayLeft = rotLeft * dir;
        Vector3 rayRight = rotRight * dir;

        Gizmos.DrawRay(transform.position, rayLeft * targetDetectionVisionRadius);
        Gizmos.DrawRay(transform.position, dir * targetDetectionVisionRadius);
        Gizmos.DrawRay(transform.position, rayRight * targetDetectionVisionRadius);
        // VISION ANGLE

#if UNITY_EDITOR

        // в PlayMode рисуем маршрут по точкам в мировых координатах
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < patrolPositions.Count; i++)
            {
                if ((i + 1) < patrolPositions.Count)
                    Gizmos.DrawLine(patrolPositions[i], patrolPositions[i + 1]);
                else
                    Gizmos.DrawLine(patrolPositions[i], patrolPositions[0]);
            }
        }
        // в режиме редактирования рисуем маршрут по дочерним элементам patrolPoints
        else
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < patrolPointsTransform.childCount; i++)
            {
                if ((i + 1) < patrolPointsTransform.childCount)
                    Gizmos.DrawLine(patrolPointsTransform.GetChild(i).position, patrolPointsTransform.GetChild(i + 1).position);
                else
                    Gizmos.DrawLine(patrolPointsTransform.GetChild(i).position, patrolPointsTransform.GetChild(0).position);
            }
        }

#endif

    }

    #endregion

    public override void HandleDamage(float damageAmount)
    {
        SetState(State.RagePursuit);

        base.HandleDamage(damageAmount);
    }
}
