using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private float speed = 15f;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Player player;
    private bool isStopped = false;

    #endregion

    #region Unity lifecycle

    private void Start()
    {
        player = Player.Instance;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (player.IsDied)
        {
            if (!isStopped)
            {
                StopMovement();
            }

            return;
        }

        Move();
        Rotate();
    }

    #endregion

    #region Private methods

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(horizontal, vertical);

        rb.velocity = dir * speed;

        animator.SetFloat(AnimationIdHelper.GetId(AnimationState.Move), dir.magnitude);
    }

    private void Rotate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = (mousePos - transform.position);

        transform.up = -(Vector2) dir;
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        isStopped = true;
    }

    #endregion
}