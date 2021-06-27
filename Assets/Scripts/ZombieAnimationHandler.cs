using System;
using UnityEngine;

public class ZombieAnimationHandler : MonoBehaviour
{
    public event Action OnAttackPerformed;

    // Used in animation
    private void PerformAttack()
    {
        OnAttackPerformed?.Invoke();
    }
}
