using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    // ������������� �� ����� ���� (������ �� ������� �����, ��� �������� �� ������ ����������)
    public bool IsFromPool { get; set; }

    #region Event handlers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollect();
    }

    #endregion

    protected abstract void HandleCollect();
}
