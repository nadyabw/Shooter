using UnityEngine;

public abstract class DamageableObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer dd = collision.gameObject.GetComponent<DamageDealer>();
        if (dd != null)
        {
            HandleDamage(dd.Damage);
        }
    }

    public abstract void HandleDamage(float damageAmount);
}
