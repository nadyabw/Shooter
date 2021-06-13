using UnityEngine;
using System.Collections.Generic;

public enum AnimationState { Idle, Move, Shoot, Death }

public static class AnimationIdHelper
{
    private static Dictionary<AnimationState, int> animationIdMap;

    private static void Init()
    {
        animationIdMap = new Dictionary<AnimationState, int>();

        animationIdMap.Add(AnimationState.Idle, Animator.StringToHash("Idle"));
        animationIdMap.Add(AnimationState.Move, Animator.StringToHash("MoveSpeed"));
        animationIdMap.Add(AnimationState.Shoot, Animator.StringToHash("Shoot"));
        animationIdMap.Add(AnimationState.Death, Animator.StringToHash("Death"));
    }

    public static int GetId(AnimationState state)
    {
        if(animationIdMap == null)
        {
            Init();
        }

        return animationIdMap[state];
    }
}
