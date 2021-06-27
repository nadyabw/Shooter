using UnityEngine;

public abstract class BaseUnit : DamageableObject
{
    #region Variables

    [Header("Bonuses")]
    [SerializeField] protected BaseItem itemPrefab;
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

    protected float timeToNextAttack;
    protected float currentHealth;

    #endregion

    #region Unity lifecycle

    protected virtual void Start()
    {
        cachedTransform = transform;
        currentHealth = healthMax;
    }

    #endregion

    #region Protected and private methods

    protected virtual void Die()
    {
        PlayDeathAnimation();

        GetComponent<Collider2D>().enabled = false;
        healthBar.gameObject.SetActive(false);
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
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
        int randNum = Random.Range(1, 101);
        if((randNum <= itemGenerationProbability) && (itemPrefab != null))
        {
            Instantiate(itemPrefab, cachedTransform.position, Quaternion.identity);
        }
    }

    protected abstract void Attack();

    #endregion

    public override void HandleDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            CheckItemGeneration();

            currentHealth = 0;
            Die();
        }
    }
}
