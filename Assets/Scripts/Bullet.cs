using Lean.Pool;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    #region Variables

    [SerializeField] private float speed = 75f;

    private Rigidbody2D rb;

    // ����� �� ���������� ������ � ��� ��������� ��� � �� �������� ���������, �.�.
    // OnBecameInvisible � OnTriggerEnter2D ����� ���������� ��� ����� � ��� �� ����
    // ������������, - ��� ��������� � ����������� ��������. ���� �� ����������� ���
    // �� ��� �� �������� ����������� ������ (���� ���� ������ capacity 10)
    private bool isActive;

    #endregion

    #region Unity lifecycle

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    #endregion

    #region Private methods

    private void OnBecameInvisible()
    {
        if (isActive)
            KillSelf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
            KillSelf();
    }

    private void KillSelf()
    {
        isActive = false;

        LeanPool.Despawn(gameObject);
    }

    public void OnSpawn()
    {
        isActive = true;
        rb.velocity = -transform.up * speed;
    }

    public void OnDespawn()
    {
    }

    #endregion
}