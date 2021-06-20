using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject healthProgress;

    #endregion

    #region Public methods

    public void UpdateHealthState(float val)
    {
        val = Mathf.Clamp(val, 0f, 1f);

        healthProgress.transform.localScale = new Vector3(val, 1f, 1f);
    }

    #endregion
}