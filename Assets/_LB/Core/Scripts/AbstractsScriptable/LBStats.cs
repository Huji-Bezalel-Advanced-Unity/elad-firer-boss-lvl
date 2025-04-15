using UnityEngine;

namespace _LB.Core.Scripts.AbstractsScriptable
{
    public abstract class LBStats: ScriptableObject
    {
        [Header("MovementStats")]
        [SerializeField] private float moveSpeed;
        
        [Header("AttackerStats")]
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float projectileBuffer;
        [SerializeField] private float projectileSpawnRate;


        public float MoveSpeed => moveSpeed;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileBuffer => projectileBuffer;
        public float ProjectileSpawnRate => projectileSpawnRate;
        
    }
}