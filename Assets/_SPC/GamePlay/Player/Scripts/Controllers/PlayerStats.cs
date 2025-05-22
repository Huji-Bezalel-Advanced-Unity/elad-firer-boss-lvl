
using UnityEngine;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    [CreateAssetMenu(menuName = "Player/ScriptableObjects/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        
        [Header("MovementStats")]
        public float Acceleration;
        public float MovementThreshold;
        
        [Header("AttackerStats")]
        public float ProjectileSpeed;
        public float ProjectileBuffer;
        public float ProjectileSpawnRate;
        
        
        [Header("DataStats")]
        public float Health;
        
        [Header("AnimationStats")]
        public float RotationSpeed;
        
        
        
    }
}