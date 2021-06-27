using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform healthProgressTransform;
    [SerializeField] private float valueChangeTime = 1.5f;

    #endregion

    private float targetValue = 1f;
    private float currentValueChangeTime;

    #region Unity lifecycle

    private void Update()
    {
        // плавно изменяем хэлсбар к заданному значению
        if (currentValueChangeTime < valueChangeTime)
        {
            currentValueChangeTime += Time.deltaTime;
            float currentValue = healthProgressTransform.localScale.x;
            currentValue = Mathf.Lerp(currentValue, targetValue, currentValueChangeTime / valueChangeTime);
            healthProgressTransform.localScale = new Vector3(currentValue, 1f, 1f);
        }
    }

    #endregion

    #region Public methods

    public void UpdateHealthState(float value)
    {
        value = Mathf.Clamp(value, 0f, 1f);

        targetValue = value;
        currentValueChangeTime = 0f;
    }

    #endregion
}