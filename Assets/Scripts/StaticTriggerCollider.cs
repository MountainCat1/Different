using System;
using System.Collections.Generic;
using UnityEngine;

public class StaticTriggerCollider : TriggerCollider
{
    private HashSet<Vector2Int> collision;

    public override HashSet<Vector2Int> GetCollisions()
    {
        

#if (UNITY_EDITOR)
        return base.GetCollisions();
#else
        if (collision != null && collision.Count != 0)
        {
            Debug.LogWarning("Static collider used twice!");
            return collision;
        }

        collision = base.GetCollisions();
        return collision;
#endif
    }
}
