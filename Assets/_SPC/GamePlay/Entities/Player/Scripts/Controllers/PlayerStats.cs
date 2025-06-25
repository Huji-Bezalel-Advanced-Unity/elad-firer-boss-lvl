using UnityEngine;

namespace _SPC.GamePlay.Entities.Player
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
        [Range(1, 10)] public int NumberOfShots = 1;
        [Range(0f, 2f)] public float ShotSpacing = 0.5f;
        
        
        [Header("DataStats")]
        public float Health;
        
        [Header("Upgrade Stats")]
        public int ScoreThresholdUpgrade = 2000;
        [Range(1f, 5f)] public float ScoreThresholdMultiplier = 1.5f;
        
        [Header("AnimationStats")]
        public float RotationSpeed;
        
        
        
    }
}