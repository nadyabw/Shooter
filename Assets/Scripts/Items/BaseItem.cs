using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    #region Event handlers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            HandleCollect();
            Destroy(gameObject);
        }
    }

    #endregion

    protected abstract void HandleCollect();
}
