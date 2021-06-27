using System.Collections;
using UnityEngine;

public class ExplosiveBarrel : DamageableObject, IExplosive
{
    #region Variables

    [Header("Base Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageRadius = 2f;
    [SerializeField] private float healthAmount = 5f;
    [SerializeField] private float chainExplosionDelay = 0.25f;
    [SerializeField] private LayerMask layerMask;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    #endregion

    #region Private methods

    private void PlayExplosionAnimation()
    {
        animator.SetTrigger(ObjectAnimationIdHelper.GetId(ObjectAnimationName.BarrelExplosion));
    }

    // Used in animation
    private void OnExplosionCompleted()
    {
        Destroy(gameObject);
    }

    private IEnumerator UpdateChainExplosionDelay(float delayedDamage)
    {
        yield return new WaitForSeconds(chainExplosionDelay);

        HandleDamage(delayedDamage);
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
            Explode();
        }
    }

    public void Explode()
    {
        PlayExplosionAnimation();
        GetComponent<Collider2D>().enabled = false;

        Collider2D[] objectsInRadius = Physics2D.OverlapCircleAll(transform.position, damageRadius, layerMask);

        foreach (Collider2D obj in objectsInRadius)
        {
            DamageableObject dObj = obj.GetComponent<DamageableObject>();
            if (dObj != null)
            {
                IExplosive iExpl = dObj.GetComponent<IExplosive>();
                if (iExpl != null)
                    iExpl.HandleChainExplosionDamage(damage);
                else
                    dObj.HandleDamage(damage);
            }
        }
    }

    public void HandleChainExplosionDamage(float delayedDamage)
    {
        StartCoroutine(UpdateChainExplosionDelay(delayedDamage));
    }
}

public interface IExplosive
{
    public void Explode();
    public void HandleChainExplosionDamage(float damage);
}
