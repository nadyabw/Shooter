using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables

    [SerializeField] private float speed = 75f;

    #endregion

    #region Unity lifecycle

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = -transform.up * speed;
    }

    #endregion

    #region Private methods

    private void OnBecameInvisible()
    {
        KillSelf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KillSelf();
    }

    private void KillSelf()
    {
        Destroy(gameObject);
    }

    #endregion
}