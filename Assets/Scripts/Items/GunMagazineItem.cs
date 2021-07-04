using Lean.Pool;
using UnityEngine;

public class GunMagazineItem : BaseItem
{
    #region Variables

    [Header("Base settings")]

    [SerializeField] protected int ammoAmount = 50;

    #endregion

    protected override void HandleCollect()
    {
        Player pl = Player.Instance;
        if (pl.HasMaxAmmo())
            return;

        pl.HandleAmmoCollect(ammoAmount);

        if (IsFromPool)
            LeanPool.Despawn(gameObject);
        else
            Destroy(gameObject);
    }
}
