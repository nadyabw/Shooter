using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    #region Variables

    [SerializeField] private Image progressImage;
    [SerializeField] private float valueChangeTime = 1.5f;

    #endregion

    private float targetValue = 1f;
    private float currentValueChangeTime;

    #region Unity lifecycle

    private void Update()
    {
        if (currentValueChangeTime < valueChangeTime)
        {
            currentValueChangeTime += Time.deltaTime;
            float currentValue = progressImage.fillAmount;
            currentValue = Mathf.Lerp(currentValue, targetValue, currentValueChangeTime / valueChangeTime);
            progressImage.fillAmount = currentValue;
        }
    }

    #endregion

    #region Public methods

    public void UpdateState(float value)
    {
        value = Mathf.Clamp(value, 0f, 1f);

        targetValue = value;
        currentValueChangeTime = 0f;
    }

    #endregion
}