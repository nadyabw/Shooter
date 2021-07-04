using Lean.Common.Examples;
using Lean.Pool;
using UnityEngine;

public abstract class BaseShootingUnit : BaseUnit
{
    #region Variables    

    [Header("Shooting")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;

    #endregion

    #region Protected and Private methods

    protected virtual void Shoot()
    {
        CreateBullet();
        PlayShootAnimation();
    }

    protected virtual void CreateBullet()
    {
        LeanPool.Spawn(bulletPrefab, bulletSpawnPoint.position, bodyTransform.rotation);
    }
    protected void PlayShootAnimation()
    {
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Shoot));
    }

    #endregion
}
