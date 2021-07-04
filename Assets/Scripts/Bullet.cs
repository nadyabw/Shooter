using Lean.Pool;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    #region Variables

    [SerializeField] private float speed = 75f;

    private Rigidbody2D rb;

    // чтобы не возвращать объект в пул несколько раз и всЄ работало корректно, т.к.
    // OnBecameInvisible и OnTriggerEnter2D могут вызыватьс€ дл€ одной и той же пули
    // одновременно, - при попадании в преп€тствие например. ≈сли не отслеживать это
    // то пул со временем становитьс€ пустым (даже если задано capacity 10)
    private bool isActive;

    #endregion

    #region Unity lifecycle

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    #endregion

    #region Private methods

    private void OnBecameInvisible()
    {
        if (isActive)
            KillSelf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
            KillSelf();
    }

    private void KillSelf()
    {
        isActive = false;

        LeanPool.Despawn(gameObject);
    }

    public void OnSpawn()
    {
        isActive = true;
        rb.velocity = -transform.up * speed;
    }

    public void OnDespawn()
    {
    }

    #endregion
}