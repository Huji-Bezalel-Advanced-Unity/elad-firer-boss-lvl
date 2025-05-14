using UnityEngine;

namespace _LB.Core.Scripts.AbstractsScriptable
{
    public abstract class LBStats: ScriptableObject
    {
        [Header("MovementStats")]
        [SerializeField] private float acceleration;
        [SerializeField] private float movementThreshold;
        
        [Header("AttackerStats")]
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float projectileBuffer;
        [SerializeField] private float projectileSpawnRate;


        public float Accelartion => acceleration;
        public float MovementThreshold => movementThreshold;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileBuffer => projectileBuffer;
        public float ProjectileSpawnRate => projectileSpawnRate;
        
    }
}