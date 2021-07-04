using Lean.Pool;
using UnityEngine;

public class HealthItem : BaseItem
{
    #region Variables

    [Header("Base settings")]

    [SerializeField] protected float healthAmount = 10f;

    #endregion

    protected override void HandleCollect()
    {
        Player pl = Player.Instance;
        if (pl.HasMaxHeatlh())
            return;

        pl.HandleHealthCollect(healthAmount);

        if (IsFromPool)
            LeanPool.Despawn(gameObject);
        else
            Destroy(gameObject);
    }
}
