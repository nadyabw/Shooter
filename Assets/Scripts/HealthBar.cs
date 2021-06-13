using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject healthProgress;

    private GameObject parentObject;
    private Vector3 offsetFromParent;

    #endregion

    #region Unity lifecycle

    private void Update()
    {
        UpdatePosition();
    }

    #endregion

    #region Public methods

    public void UpdateHealthState(float val)
    {
        val = Mathf.Clamp(val, 0f, 1f);

        healthProgress.transform.localScale = new Vector3(val, 1f, 1f);
    }


    public void SetParentAndOffset(GameObject parentObj, Vector3 offset)
    {
        parentObject = parentObj;
        offsetFromParent = offset;

        UpdatePosition();
    }

    #endregion

    #region Private methods

    private void UpdatePosition()
    {
        transform.position = parentObject.transform.position + offsetFromParent;
    }

    #endregion
}