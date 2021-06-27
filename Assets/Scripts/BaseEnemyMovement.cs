using UnityEngine;

public class BaseEnemyMovement : MonoBehaviour
{
    #region Variables

    [Header("Base settings")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private float baseSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Transform cachedTransform;

    private Rigidbody2D rb;
    private Vector3 direction;
    private Vector3 targetPosition;
    private float currentSpeed;

    private bool isStopped;

    #endregion

    #region Unity lifecycle

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cachedTransform = transform;
    }

    private void Update()
    {
        if(!isStopped)
        {
            Move();
        }

        Rotate();
    }

    #endregion

    #region Private methods

    private void Move()
    {
        rb.velocity = direction * currentSpeed;

        animator.SetFloat(UnitAnimationIdHelper.GetId(UnitAnimationState.Move), direction.magnitude);
    }

    private void Rotate()
    {
        direction = (targetPosition - cachedTransform.position).normalized;
        bodyTransform.up = -(Vector2)direction;
    }

    #endregion

    public void SetTargetPosition(Vector3 pos)
    {
        targetPosition = pos;
    }

    public void StopMovement()
    {
        isStopped = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        currentSpeed = 0;

        animator.SetFloat(UnitAnimationIdHelper.GetId(UnitAnimationState.Move), 0);
    }

    public void StartMovement(float speedFactor = 1f)
    {
        isStopped = false;
        rb.isKinematic = false;
        currentSpeed = baseSpeed * speedFactor;
    }
}