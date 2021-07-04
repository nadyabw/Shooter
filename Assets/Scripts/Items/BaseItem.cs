using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    // сгенерировано во время игры (выпало из убитого врага, или положено на уровне изначально)
    public bool IsFromPool { get; set; }

    #region Event handlers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollect();
    }

    #endregion

    protected abstract void HandleCollect();
}
