using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform bodyTransform;
    [SerializeField] private float speed = 15f;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Transform cachedTransform;

    #endregion

    #region Unity lifecycle

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cachedTransform = transform;
    }

    private void Update()
    {
        Move();
        Rotate();
    }

    #endregion

    #region Private methods

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 dir = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);

        rb.velocity = dir * speed;

        animator.SetFloat(UnitAnimationIdHelper.GetId(UnitAnimationState.Move), dir.magnitude);
    }

    private void Rotate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mousePos - cachedTransform.position;

        bodyTransform.up = -(Vector2) dir;
    }

    #endregion

    public void Stop()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }
}