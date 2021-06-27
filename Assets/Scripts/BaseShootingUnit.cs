using UnityEngine;

public abstract class BaseShootingUnit : BaseUnit
{
    #region Variables    

    [Header("Shooting")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;

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

/*    protected override void Die()
    {
        base.Die();
    }*/

    protected void PlayShootAnimation()
    {
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Shoot));
    }

    #endregion
}
