using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    #region Variables

    [SerializeField] private float damage;

    #endregion

    #region Properties

    public float Damage => damage;

    #endregion
}