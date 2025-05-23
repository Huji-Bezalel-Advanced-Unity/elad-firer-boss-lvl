
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
        [SerializeField] [Range(5f,40f)] public float ProjectileSpeed;
        [SerializeField] [Range(0f,10f)] public float ProjectileBuffer;
        [SerializeField] [Range(0.1f,3f)] public float ProjectileSpawnRate;
        
        
        [Header("DataStats")]
        public float Health;
        
        [Header("AnimationStats")]
        public float RotationSpeed;
        
        
        
    }
}