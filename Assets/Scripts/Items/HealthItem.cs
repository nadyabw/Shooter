using UnityEngine;

public class HealthItem : BaseItem
{
    #region Variables

    [Header("Base settings")]

    [SerializeField] protected float healthAmount = 10f;

    #endregion

    protected override void HandleCollect()
    {
        Player.Instance.HandleHealthCollect(healthAmount);
    }
}
