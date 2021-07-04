using UnityEngine;
using System.Collections.Generic;

public enum UnitAnimationState { Idle, Move, Shoot, SBAttack1, SBAttack2, SBAttack3, SBAttack4, Death, Ressurect }

public static class UnitAnimationIdHelper
{
    private static Dictionary<UnitAnimationState, int> animationIdMap;

    private static void Init()
    {
        animationIdMap = new Dictionary<UnitAnimationState, int>();

        animationIdMap.Add(UnitAnimationState.Idle, Animator.StringToHash("Idle"));
        animationIdMap.Add(UnitAnimationState.Move, Animator.StringToHash("MoveSpeed"));
        animationIdMap.Add(UnitAnimationState.Shoot, Animator.StringToHash("Shoot"));
        animationIdMap.Add(UnitAnimationState.Death, Animator.StringToHash("Death"));
        animationIdMap.Add(UnitAnimationState.Ressurect, Animator.StringToHash("Ressurect"));
        animationIdMap.Add(UnitAnimationState.SBAttack1, Animator.StringToHash("SBAttack1"));
        animationIdMap.Add(UnitAnimationState.SBAttack2, Animator.StringToHash("SBAttack2"));
        animationIdMap.Add(UnitAnimationState.SBAttack3, Animator.StringToHash("SBAttack3"));
        animationIdMap.Add(UnitAnimationState.SBAttack4, Animator.StringToHash("SBAttack4"));
    }

    public static int GetId(UnitAnimationState state)
    {
        if(animationIdMap == null)
        {
            Init();
        }

        if(animationIdMap.ContainsKey(state))
        {
            return animationIdMap[state];
        }

        return -1;
    }
}
