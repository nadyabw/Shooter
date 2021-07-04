using Pathfinding;
using UnityEngine;

public class BaseEnemyMovement : MonoBehaviour
{
    #region Variables

    [Header("Base settings")]
    [SerializeField] protected Transform bodyTransform;
    [SerializeField] protected float baseSpeed = 5f;

    [Header("Animation")]
    [SerializeField] protected Animator animator;

    protected Transform cachedTransform;
    protected Transform targetTransform;

    protected Rigidbody2D rb;

    protected AIPath aiPath;
    protected AIDestinationSetter aiDestinationSetter;

    #endregion

    #region Unity lifecycle

    private void Start()
    {
        cachedTransform = transform;
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();

        // создаем пустой GameObject чтобы подсунуть его Transform в aiDestinationSetter
        // затем просто ситуативно двигаем этот пустой GameObject в точку назначения
        CreateTargetTransform();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiDestinationSetter.target = targetTransform;
    }

    protected virtual void Update()
    {
        animator.SetFloat(UnitAnimationIdHelper.GetId(UnitAnimationState.Move), aiPath.velocity.magnitude);
        Rotate();
    }

    #endregion

    #region Private methods

    protected virtual void Rotate()
    {
        Vector3 dir = (targetTransform.position - cachedTransform.position).normalized;
        float angle = Vector3.Angle(dir, aiPath.velocity);
        if(angle > 45)
        {
            bodyTransform.up = -aiPath.velocity;
        }
        else
        {
            bodyTransform.up = -(Vector2)dir;
        }
    }

    private void CreateTargetTransform()
    {
        Positions positions = FindObjectOfType<Positions>();
        GameObject go = new GameObject();
        go.transform.parent = positions.transform;
        targetTransform = go.transform;
    }

    #endregion

    public void SetTargetPosition(Vector3 pos)
    {
        targetTransform.position = pos;
    }

    public void StopMovement()
    {
        aiPath.maxSpeed = 0;
        aiPath.enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        animator.SetFloat(UnitAnimationIdHelper.GetId(UnitAnimationState.Move), 0);
    }

    public void StartMovement(float speedFactor = 1f)
    {
        rb.isKinematic = false;
        aiPath.enabled = true;
        aiPath.maxSpeed = baseSpeed * speedFactor;
    }
}