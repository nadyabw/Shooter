using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables

    [Header("Shooting")] [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootDelay = 0.25f;

    [Header("Health")] [SerializeField] private float healthMax;
    [SerializeField] private HealthBar healthBarPrefab;
    [SerializeField] private Transform healthBarPosition;

    [Header("Animation")] [SerializeField] private Animator animator;

    private static Player instance;

    private float timeToNextShoot;

    private HealthBar healthBar;
    private float currentHealth;

    #endregion

    #region Properties

    public static Player Instance => instance;
    public bool IsDied { get; private set; }

    #endregion

    #region Events

    public static event Action OnDied;

    #endregion

    #region Unity lifecycle

    private void Awake()
    {
        instance = this;

        currentHealth = healthMax;
        healthBar = Instantiate(healthBarPrefab);
        healthBar.SetParentAndOffset(gameObject, healthBarPosition.localPosition);
    }

    private void Update()
    {
        UpdateShoot();
    }

    #endregion

    #region Private methods

    private void UpdateShoot()
    {
        if (IsDied)
            return;

        timeToNextShoot -= Time.deltaTime;

        if (Input.GetButton("Fire1") && timeToNextShoot <= 0)
        {
            CreateBullet();
            timeToNextShoot = shootDelay;
            PlayShootAnimation();
        }
    }


    private void CreateBullet()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer dd = collision.gameObject.GetComponent<DamageDealer>();

        if (dd)
        {
            HandleDamage(dd.Damage);
        }
    }

    private void HandleDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.UpdateHealthState(currentHealth / healthMax);
    }

    private void Die()
    {
        IsDied = true;
        OnDied?.Invoke();

        PlayDeathAnimation();

        Destroy(GetComponent<Collider2D>());
        Destroy(healthBar.gameObject);
    }

    private void PlayShootAnimation()
    {
        animator.SetTrigger(AnimationIdHelper.GetId(AnimationState.Shoot));
    }

    private void PlayDeathAnimation()
    {
        animator.SetTrigger(AnimationIdHelper.GetId(AnimationState.Death));
    }

    public void AddHealth(float amount)
    {
        currentHealth += amount;

        if (currentHealth > healthMax)
        {
            currentHealth = healthMax;
        }

        UpdateHealthBar();
    }

    #endregion
}