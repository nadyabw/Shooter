using UnityEngine;

public class Enemy : BaseShootingUnit
{
    #region Variables

    [Header("Base settings")]
    [Range(1, 5)]
    [SerializeField] private int targetUpdateDelayFactor;

    private float timeToNextShoot;

    private Player player;
    private Transform playerTransform;

    private Vector3 targetPosition;

    #endregion

    #region Unity lifecycle

    protected override void Start()
    {
        base.Start();

        player = Player.Instance;
        playerTransform = player.transform;

        timeToNextShoot = shootDelay;
    }

    private void Update()
    {
        if (IsDied || player.IsDied)
            return;

        CheckTargetPosition();
        UpdateShootAim();

        UpdateShoot();
    }

    #endregion

    #region Private methods

    private void UpdateShootAim()
    {
        Vector3 dir = (targetPosition - cachedTransform.position);
        bodyTransform.up = -(Vector2)dir;
    }

    private void UpdateShoot()
    {
        timeToNextShoot -= Time.deltaTime;

        if (timeToNextShoot <= 0)
        {
            Shoot();
            timeToNextShoot = shootDelay;
        }
    }

/*    protected override void Die()
    {
        base.Die();
    }*/

    private void CheckTargetPosition()
    {
        // следим за позицией игрока с некоторым запаздыванием
        targetPosition += (playerTransform.position - targetPosition) / (targetUpdateDelayFactor * 10f);
    }

    #endregion
}