using UnityEngine;
using System.Collections.Generic;

public enum ObjectAnimationName { BarrelExplosion }

public static class ObjectAnimationIdHelper
{
    private static Dictionary<ObjectAnimationName, int> animationIdMap;

    private static void Init()
    {
        animationIdMap = new Dictionary<ObjectAnimationName, int>();

        animationIdMap.Add(ObjectAnimationName.BarrelExplosion, Animator.StringToHash("Explosion"));
    }

    public static int GetId(ObjectAnimationName animName)
    {
        if (animationIdMap == null)
        {
            Init();
        }

        if (animationIdMap.ContainsKey(animName))
        {
            return animationIdMap[animName];
        }

        return -1;
    }
}
