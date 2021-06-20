using System;
using UnityEngine;

public class Player : BaseShootingUnit
{
    #region Variables

    [Header("Movement")]
    [SerializeField] private PlayerMovement playerMovement;

    private static Player instance;
    private float timeToNextShoot;

    #endregion

    #region Properties

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
            Shoot();
            timeToNextShoot = shootDelay;
        }
    }

    protected override void Die()
    {
        base.Die();

        playerMovement.Stop();
        playerMovement.enabled = false;
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