
using UnityEngine;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    [CreateAssetMenu(menuName = "Player/ScriptableObjects/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        
        [Header("MovementStats")]
        [SerializeField] private float acceleration;
        [SerializeField] private float movementThreshold;
        
        [Header("AttackerStats")]
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float projectileBuffer;
        [SerializeField] private float projectileSpawnRate;
        
        
        [Header("DataStats")]
        [SerializeField] private float health;
        
        [Header("AnimationStats")]
        [SerializeField] private float rotationSpeed;
        
        
        public float RotationSpeed => rotationSpeed;
        public float Accelartion => acceleration;
        public float MovementThreshold => movementThreshold;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileBuffer => projectileBuffer;
        public float ProjectileSpawnRate => projectileSpawnRate;
        public float Health => health;
        
    }
}