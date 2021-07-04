using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieBoss : Zombie
{
    #region Variables

    [Header("Boss type")]
    [SerializeField] private bool isSuperBoss = false;

    [Header("Life cycle")]
    [SerializeField] private int lifesNumber = 2;
    [SerializeField] private float ressurectionDelay = 1.5f;

    private float timeToRessurection;
    private bool isActivated;

    #endregion

    #region Events

    public static event Action OnDied;
    public static event Action<float> OnHealthChanged;
    public static event Action OnActivated;

    #endregion

    #region Unity lifecycle

    protected override void Update()
    {
        base.Update();

        CheckActivation();
    }

    #endregion

    #region Private and Protected methods

    protected override void PlayAttackAnimation()
    {
        if(!isSuperBoss)
        {
            base.PlayAttackAnimation();
        }
        else
        {
            int randNum = Random.Range(1, 5);
            switch (randNum)
            {
                case 1:
                    animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.SBAttack1));
                    break;
                case 2:
                    animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.SBAttack2));
                    break;
                case 3:
                    animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.SBAttack3));
                    break;
                case 4:
                    animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.SBAttack4));
                    break;
            }
        }
    }

    private void CheckActivation()
    {
        if(!isActivated && ((currentState == State.Pursuit) || (currentState == State.RagePursuit)))
        {
            isActivated = true;
            OnActivated?.Invoke();
        }
    }

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
        bodyCollider.enabled = true;
        currentHealth = healthMax;
        OnHealthChanged?.Invoke(currentHealth / healthMax);

        zombieMovement.enabled = true;
        SetState(State.Idle);
        animator.SetTrigger(UnitAnimationIdHelper.GetId(UnitAnimationState.Ressurect));
    }

    #endregion

    public override void HandleDamage(float damageAmount)
    {
        base.HandleDamage(damageAmount);

        OnHealthChanged?.Invoke(currentHealth / healthMax);
    }
}
