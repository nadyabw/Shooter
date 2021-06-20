using UnityEngine;

public abstract class BaseShootingUnit : DamageableObject
{
    #region Variables    

    [Header("Shooting")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;
    [SerializeField] protected float shootDelay = 1.0f;

    [Header("Health")]
    [SerializeField] protected float healthMax;
    [SerializeField] protected HealthBar healthBar;

    [Header("Animation")]
    [SerializeField] protected Animator animator;

    [Header("Movement")]
    [SerializeField] protected Transform bodyTransform;

    protected Transform cachedTransform;

    protected float currentHealth;

    #endregion

    #region Properties

    public bool IsDied { get; protected set; }

    #endregion

    #region Unity lifecycle

    protected virtual void Start()
    {
        cachedTransform = transform;
        currentHealth = healthMax;
    }

    #endregion

    #region Protected and Private methods

    protected void Shoot()
    {
        CreateBullet();
        PlayShootAnimation();
    }

    protected void CreateBullet()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bodyTransform.rotation);
    }

    protected virtual void Die()
    {
        IsDied = true;

        PlayDeathAnimation();

        GetComponent<Collider2D>().enabled = false;
        Destroy(healthBar.gameObject);
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
    }

    protected void PlayShootAnimation()
    {
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Shoot));
    }

    protected void PlayDeathAnimation()
    {
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Death));
    }

    protected void UpdateHealthBar()
    {
        healthBar.UpdateHealthState(currentHealth / healthMax);
    }

    #endregion

    public override void HandleDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
