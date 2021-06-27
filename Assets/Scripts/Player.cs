using System;
using UnityEngine;

public class Player : BaseShootingUnit
{
    #region Variables

    [Header("Movement")]
    [SerializeField] private PlayerMovement playerMovement;

    private static Player instance;

    #endregion

    #region Properties

    public bool IsDied { get; private set; }
    public static Player Instance => instance;

    #endregion

    #region Events

    public static event Action OnDied;

    #endregion

    #region Unity lifecycle

    private void Awake()
    {
        instance = this;
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

    public void HandleHealthCollect(float amount)
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