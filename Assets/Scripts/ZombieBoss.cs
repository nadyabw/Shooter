using System;
using UnityEngine;

public class ZombieBoss : Zombie
{
    #region Variables

    [Header("Life cycle")]
    [SerializeField] private int lifesNumber = 2;
    [SerializeField] private float ressurectionDelay = 1.5f;

    private float timeToRessurection;

    #endregion

    #region Events

    public static event Action OnDied;

    #endregion

    #region Private and Protected methods

    protected override void Die()
    {
        base.Die();

        lifesNumber--;

        // если ещё есть жизни то "воскресаем"
        if(lifesNumber > 0)
        {
            timeToRessurection = ressurectionDelay;
        }
        else
        {
            OnDied?.Invoke();
        }
    }

    protected override void UpdateDead()
    {
        if (timeToRessurection > 0)
        {
            timeToRessurection -= Time.deltaTime;
            if (timeToRessurection <= 0)
            {
                Ressurect();
            }
        }
    }

    private void Ressurect()
    {
        GetComponent<Collider2D>().enabled = true;
        currentHealth = healthMax;
        healthBar.gameObject.SetActive(true);
        UpdateHealthBar();

        zombieMovement.enabled = true;
        SetState(State.Idle);
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Ressurect));
    }

    #endregion
}
