using System.Collections.Generic;
using UnityEngine;

public class Zombie : DamageableObject
{
    private enum State {Idle, ReturnToStartPosition, Patrol, Pursuit, RagePursuit, Attack, Dead }

    #region Variables

    [Header("Base settings")]
    [SerializeField] private float pursuitRadius = 14f;
    [SerializeField] private float targetDetectionRadius = 10f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float rageTime = 5f;

    [Header("Health")]
    [SerializeField] private float healthMax;
    [SerializeField] private HealthBar healthBar;

    [Header("Attack")]
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private float damageAmount = 2f;
    [Range(1, 5)]
    [SerializeField] private int targetUpdateDelayFactor;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private ZombieMovement zombieMovement;
    [SerializeField] private Transform patrolPointsTransform;

    private Player player;
    private Transform playerTransform;

    private Transform cachedTransform;

    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float timeToNextAttack;

    private State currentState;
    private float currentHealth;
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

    private void Start()
    {
        cachedTransform = transform;

        player = Player.Instance;
        playerTransform = player.transform;

        Init();
    }

    private void Update()
    {
        if ((currentState == State.Dead) || player.IsDied)
            return;

        CheckState();
        UpdateCurrentState();
    }

    #endregion

    #region Private methods

    private void Init()
    {
        currentHealth = healthMax;

        for (int i = 0; i < patrolPointsTransform.childCount; i++)
        {
            patrolPositions.Add(patrolPointsTransform.GetChild(i).transform.position);
        }
        patrolPointsTransform.gameObject.SetActive(false);

        if(patrolPositions.Count >= 2)
        {
            isPatrolRole = true;
            SetState(State.Patrol);
        }
        else
        {
            startPosition = cachedTransform.position;
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
        else if ((distance < targetDetectionRadius) && (currentState != State.RagePursuit))
        {
            SetState(State.Pursuit);
        }
        else if ((distance > pursuitRadius) && (currentState != State.RagePursuit))
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

    private void SetState(State state)
    {
        switch (state)
        {
            case State.Idle:
                zombieMovement.StopMovement();
                break;
            case State.ReturnToStartPosition:
                zombieMovement.StartMovement(0.8f);
                zombieMovement.SetTargetPosition(startPosition);
                break;
            case State.Patrol:
                zombieMovement.StartMovement(0.5f);
                zombieMovement.SetTargetPosition(patrolPositions[currentPatrolPositionIndex]);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer dd = collision.gameObject.GetComponent<DamageDealer>();
        if (dd != null)
        {
            HandleDamage(dd.Damage);
        }
    }

    private void UpdateAttack()
    {
        timeToNextAttack -= Time.deltaTime;
        if(timeToNextAttack <= 0)
        {
            PlayAttackAnimation();
            player.HandleDamage(damageAmount);
            timeToNextAttack = attackDelay;
        }
    }

    private void UpdatePursuit()
    {
        // следим за позицией игрока с некоторым запаздыванием
        targetPosition += (playerTransform.position - targetPosition) / (targetUpdateDelayFactor * 10f);
        zombieMovement.SetTargetPosition(targetPosition);
    }

    private void UpdateRagePursuit()
    {
        targetPosition = playerTransform.position;
        zombieMovement.SetTargetPosition(targetPosition);

        currentRageTime -= Time.deltaTime;
        if (currentRageTime <= 0)
            SetState(State.Pursuit);
    }

    private void UpdatePatrol()
    {
        if (Vector3.Distance(cachedTransform.position, patrolPositions[currentPatrolPositionIndex]) <= 0.1f)
        {
            currentPatrolPositionIndex++;
            if (currentPatrolPositionIndex == patrolPositions.Count)
                currentPatrolPositionIndex = 0;

            zombieMovement.SetTargetPosition(patrolPositions[currentPatrolPositionIndex]);
        }
    }

    private void UpdateReturnToStartPosition()
    {
        if (Vector3.Distance(cachedTransform.position, startPosition) <= 0.1f)
        {
            SetState(State.Idle);
        }
    }

    private void HandlePlayerDeath()
    {
        SetState(State.Idle);
    }

    private void Die()
    {
        SetState(State.Dead);

        PlayDeathAnimation();

        GetComponent<Collider2D>().enabled = false;
        Destroy(healthBar.gameObject);

        zombieMovement.StopMovement();
        zombieMovement.enabled = false;
    }

    protected void PlayAttackAnimation()
    {
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Shoot));
    }

    protected void PlayDeathAnimation()
    {
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Death));
    }

    private void UpdateHealthBar()
    {
        healthBar.UpdateHealthState(currentHealth / healthMax);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, targetDetectionRadius);

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);

        Gizmos.color = Color.magenta;

        // в PlayMode рисуем маршрут по точкам в мировых координатах
        if(UnityEditor.EditorApplication.isPlaying)
        {
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
            for (int i = 0; i < patrolPointsTransform.childCount; i++)
            {
                if ((i + 1) < patrolPointsTransform.childCount)
                    Gizmos.DrawLine(patrolPointsTransform.GetChild(i).transform.position, patrolPointsTransform.GetChild(i + 1).transform.position);
                else
                    Gizmos.DrawLine(patrolPointsTransform.GetChild(i).transform.position, patrolPointsTransform.GetChild(0).transform.position);
            }
        }
    }

    #endregion

    public override void HandleDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthBar();
        SetState(State.RagePursuit);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
