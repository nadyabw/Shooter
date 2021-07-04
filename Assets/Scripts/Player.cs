using System;
using UnityEngine;

public class Player : BaseShootingUnit
{
    #region Variables

    [Header("Shooting")]
    [SerializeField] private int ammoMax = 100;

    [Header("Movement")]
    [SerializeField] private PlayerMovement playerMovement;

    private static Player instance;

    private int currentAmmo;

    #endregion

    #region Properties

    public bool IsDied { get; private set; }
    public static Player Instance => instance;

    #endregion

    #region Events

    public static event Action OnDied;
    public static event Action<float> OnHealthChanged;
    public static event Action<float> OnAmmoChanged;

    #endregion

    #region Unity lifecycle

    private void Awake()
    {
        instance = this;
        currentAmmo = ammoMax;
    }

    private void Update()
    {
        UpdateAttack();
    }

    #endregion

    #region Private and protected methods

    protected override void Attack()
    {
        if (IsDied)
            return;

        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    protected override void Shoot()
    {
        if(currentAmmo > 0)
        {
            currentAmmo--;
            base.Shoot();
            OnAmmoChanged?.Invoke((float)currentAmmo / ammoMax);
        }
    }

    protected override void Die()
    {
        base.Die();

        playerMovement.Stop();
        playerMovement.enabled = false;

        IsDied = true;
        OnDied?.Invoke();
    }

    #endregion

    #region Public methods

    public override void HandleDamage(float damageAmount)
    {
        base.HandleDamage(damageAmount);

        OnHealthChanged?.Invoke(currentHealth / healthMax);
    }

    public bool HasMaxAmmo()
    {
        return Mathf.Approximately(ammoMax, currentAmmo);
    }

    public void HandleAmmoCollect(int amount)
    {
        currentAmmo += amount;

        if (currentAmmo > ammoMax)
        {
            currentAmmo = ammoMax;
        }
        OnAmmoChanged?.Invoke((float)currentAmmo / ammoMax);
    }

    public void HandleHealthCollect(float amount)
    {
        currentHealth += amount;

        if (currentHealth > healthMax)
        {
            currentHealth = healthMax;
        }
        OnHealthChanged?.Invoke(currentHealth / healthMax);
    }

    #endregion
}