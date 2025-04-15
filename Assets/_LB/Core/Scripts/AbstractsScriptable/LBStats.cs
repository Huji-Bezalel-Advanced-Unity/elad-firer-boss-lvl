using UnityEngine;

namespace _LB.Core.Scripts.Interfaces
{
    public abstract class LBStats: ScriptableObject
    {
        [Header("MovementStats")]
        float MoveSpeed { get; }

        #region MyRegion
        float ProjectileSpeed { get; }
        float ProjectileBuffer { get; }
        float ProjectileSpawnRate { get; }
        #endregion
        
    }
}