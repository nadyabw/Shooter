using Lean.Pool;
using UnityEngine;

public abstract class BaseUnit : DamageableObject
{
    #region Variables

    [Header("Bonuses")]
    [SerializeField] protected BaseItem[] itemPrefabs;
    [Range(0, 100)]
    [SerializeField] protected int itemGenerationProbability;

    [Header("Attack")]
    [SerializeField] protected float attackDelay = 1.0f;

    [Header("Health")]
    [SerializeField] protected float healthMax;
    [SerializeField] protected HealthBar healthBar;

    [Header("Animation")]
    [SerializeField] protected Animator animator;

    [Header("Movement")]
    [SerializeField] protected Transform bodyTransform;
    [SerializeField] protected float distanceCompareDelta = 0.1f;

    protected Transform cachedTransform;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D bodyCollider;

    protected float timeToNextAttack;
    protected float currentHealth;

    #endregion

    #region Unity lifecycle

    protected virtual void Start()
    {
        cachedTransform = transform;
        currentHealth = healthMax;
        bodyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    #endregion

    #region Protected and private methods

    protected virtual void Die()
    {
        PlayDeathAnimation();

        bodyCollider.enabled = false;
        spriteRenderer.sortingOrder = 0;
        if(healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    protected void UpdateHealthBar()
    {
        healthBar.UpdateHealthState(currentHealth / healthMax);
    }

    protected void PlayDeathAnimation()
    {
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Death));
    }

    protected virtual void UpdateAttack()
    {
        timeToNextAttack -= Time.deltaTime;
        if (timeToNextAttack <= 0)
        {
            Attack();
            timeToNextAttack = attackDelay;
        }
    }

    private void CheckItemGeneration()
    {
        if ((itemPrefabs == null) || (itemPrefabs.Length == 0))
            return;

        int randNum = Random.Range(1, 101);
        if(randNum <= itemGenerationProbability)
        {
            int randIdx = Random.Range(0, itemPrefabs.Length);

            BaseItem bItem = LeanPool.Spawn(itemPrefabs[randIdx], cachedTransform.position, Quaternion.identity);
            bItem.IsFromPool = true;
        }
    }

    protected abstract void Attack();

    #endregion

    public float GetSize()
    {
        return bodyCollider.bounds.size.x;
    }

    public bool HasMaxHeatlh()
    {
        return Mathf.Approximately(healthMax, currentHealth);
    }

    public override void HandleDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if(healthBar != null)
        {
            UpdateHealthBar();
        }

        if (currentHealth <= 0)
        {
            CheckItemGeneration();

            currentHealth = 0;
            Die();
        }
    }
}
