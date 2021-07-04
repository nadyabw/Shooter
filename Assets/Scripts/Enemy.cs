using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseShootingUnit
{
    private enum State { Idle, ReturnToStartPosition, Patrol, Pursuit, RagePursuit, Attack, Dead }

    #region Variables

    [Header("Vision")]
    [Range(30, 360)]
    [SerializeField] private float visionAngle;
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] private LayerMask allCollisionsMask;

    [Header("Base settings")]
    [SerializeField] private float pursuitRadius = 16f;
    [SerializeField] private float targetDetectionVisionRadius = 16f;
    [SerializeField] private float targetDetectionAnywayRadius = 12f;
    [SerializeField] private float attackRadius = 10f;
    [SerializeField] private float rageTime = 5f;

    [Header("Attack")]
    [Range(1, 5)]
    [SerializeField] private int targetUpdateDelayFactor;

    [Header("Movement")]
    [SerializeField] private BaseEnemyMovement enemyMovement;
    [SerializeField] private Transform patrolPointsTransform;

    private Player player;
    private Transform playerTransform;

    private Vector3 targetPosition;
    private Vector3 startPosition;

    private State currentState;
    private float currentRageTime;

    private bool isPatrolRole;
    private int currentPatrolPositionIndex;
    private List<Vector3> patrolPositions = new List<Vector3>();

    #endregion

    #region Unity lifecycle

    private void OnEnable()
    {
        Player.OnDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnDied -= HandlePlayerDeath;
    }

    protected override void Start()
    {
        base.Start();

        player = Player.Instance;
        playerTransform = player.transform;

        Init();
    }

    private void Update()
    {
        if ((currentState != State.Dead) && !player.IsDied)
            CheckState();

        UpdateCurrentState();
    }

    #endregion

    #region Private methods

    private void Init()
    {
        timeToNextAttack = attackDelay;

        for (int i = 0; i < patrolPointsTransform.childCount; i++)
        {
            patrolPositions.Add(patrolPointsTransform.GetChild(i).position);
        }
        patrolPointsTransform.gameObject.SetActive(false);

        if (patrolPositions.Count >= 2)
        {
            isPatrolRole = true;
            SetState(State.Patrol);
        }
        else
        {
            targetPosition = startPosition = cachedTransform.position;
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
            else if (currentState == State.Pursuit && !isPatrolRole)
            {
                SetState(State.ReturnToStartPosition);
            }
        }
    }

    private void SetState(State state)
    {
        switch (state)
        {
            case State.Idle:
                enemyMovement.StopMovement();
                break;
            case State.ReturnToStartPosition:
                enemyMovement.StartMovement(0.8f);
                targetPosition = startPosition;
                enemyMovement.SetTargetPosition(targetPosition);
                break;
            case State.Patrol:
                enemyMovement.StartMovement(0.5f);
                targetPosition = patrolPositions[currentPatrolPositionIndex];
                enemyMovement.SetTargetPosition(targetPosition);
                break;
            case State.Pursuit:
                enemyMovement.StartMovement();
                break;
            case State.RagePursuit:
                currentRageTime = rageTime;
                enemyMovement.StartMovement(1.1f);
                break;
            case State.Attack:
                enemyMovement.StopMovement();
                break;
            case State.Dead:
                enemyMovement.StopMovement();
                enemyMovement.enabled = false;
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
        }
    }

    protected override void UpdateAttack()
    {
        CheckShootingTargetPosition();
        enemyMovement.SetTargetPosition(targetPosition);

        base.UpdateAttack();
    }

    private void UpdatePursuit()
    {
        CheckTargetPosition();
        enemyMovement.SetTargetPosition(targetPosition);
    }

    private void UpdateRagePursuit()
    {
        CheckTargetPosition();
        enemyMovement.SetTargetPosition(targetPosition);

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
            enemyMovement.SetTargetPosition(targetPosition);
        }
    }

    private void UpdateReturnToStartPosition()
    {
        if (Vector3.Distance(cachedTransform.position, startPosition) <= distanceCompareDelta)
        {
            SetState(State.Idle);
        }
    }

    protected override void Attack()
    {
        // если игрок в радиусе атаки, но закрыт препятствием, то не стреляем, а ждём
        if(IsPlayerAccesibleToShoot(targetPosition))
            Shoot();
    }

    protected override void CreateBullet()
    {
        Vector3 dir = targetPosition - bulletSpawnPoint.position;
        Quaternion rot = Quaternion.FromToRotation(Vector3.down, dir);

        //Instantiate(bulletPrefab, bulletSpawnPoint.position, rot);
        LeanPool.Spawn(bulletPrefab, bulletSpawnPoint.position, rot);
    }

    protected override void Die()
    {
        base.Die();
        SetState(State.Dead);
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

    protected bool IsPlayerAccesibleToShoot(Vector3 targetPos)
    {
        Vector3 dir = targetPos - bulletSpawnPoint.position;

        RaycastHit2D rHit = Physics2D.Raycast(bulletSpawnPoint.position, dir, attackRadius, allCollisionsMask);
        if ((rHit.collider != null) && (rHit.collider.gameObject == player.gameObject))
        {
            return true;
        }

        return false;
    }

    private void CheckTargetPosition()
    {
        targetPosition += (playerTransform.position - targetPosition) / (targetUpdateDelayFactor * 10f);
    }

    private void CheckShootingTargetPosition()
    {
        //targetPosition += (playerTransform.position - targetPosition) / (targetUpdateDelayFactor * 10f);
        targetPosition = playerTransform.position;
        if (IsPlayerAccesibleToShoot(targetPosition))
            return;

        float playerBodyRadius = player.GetSize() / 2;

        Vector3 delta = bodyTransform.right * -playerBodyRadius;
        if (IsPlayerAccesibleToShoot(targetPosition + delta))
        {
            targetPosition += delta;
            return;
        }
        delta = bodyTransform.right * playerBodyRadius;
        if (IsPlayerAccesibleToShoot(targetPosition + delta))
        {
            targetPosition += delta;
            return;
        }
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
