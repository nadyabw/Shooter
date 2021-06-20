using UnityEngine;

public class ExplosiveBarrel : DamageableObject
{
    #region Variables

    [Header("Base Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageRadius = 2f;
    [SerializeField] private float healthAmount = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    #endregion

    #region Private methods

    private void PlayExplosionAnimation()
    {
        animator.SetTrigger(ObjectAnimationIdHelper.GetId(ObjectAnimationName.BarrelExplosion));
    }

    private void OnExplosionCompleted()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }

    #endregion

    public override void HandleDamage(float damageAmount)
    {
        healthAmount -= damageAmount;
        if (healthAmount <= 0)
        {
            PlayExplosionAnimation();
            GetComponent<Collider2D>().enabled = false;

            LayerMask lMask = LayerMask.GetMask(LayerNames.Enemy, LayerNames.Player, LayerNames.ExplosiveObject);
            Collider2D[] objectsInRadius = Physics2D.OverlapCircleAll(transform.position, damageRadius, lMask);

            foreach (Collider2D obj in objectsInRadius)
            {
                DamageableObject dObj = obj.gameObject.GetComponent<DamageableObject>();
                if (dObj != null)
                {
                    dObj.HandleDamage(damage);
                }
            }
        }
    }
}
