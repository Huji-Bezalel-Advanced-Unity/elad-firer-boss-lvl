using UnityEngine;

namespace _SPC.GamePlay.Entities.Player
{
    /// <summary>
    /// ScriptableObject containing all player statistics and configuration values.
    /// </summary>
    [CreateAssetMenu(menuName = "Player/ScriptableObjects/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Movement Stats")]
        [Tooltip("Acceleration force applied to the player's movement.")]
        public float Acceleration;
        
        [Tooltip("Minimum input magnitude required to trigger movement.")]
        public float MovementThreshold;
        
        [Header("Attacker Stats")]
        [Tooltip("Speed of projectiles fired by the player.")]
        [SerializeField] [Range(5f, 40f)] public float ProjectileSpeed;
        
        [Tooltip("Buffer distance for projectile spawning from the player.")]
        [SerializeField] [Range(0f, 10f)] public float ProjectileBuffer;
        
        [Tooltip("Rate at which projectiles can be spawned (time between shots).")]
        [SerializeField] [Range(0.1f, 3f)] public float ProjectileSpawnRate;
        
        [Tooltip("Number of projectiles fired in a single attack.")]
        [Range(1, 10)] public int NumberOfShots = 1;
        
        [Tooltip("Spacing between multiple projectiles when firing multiple shots.")]
        [Range(0f, 2f)] public float ShotSpacing = 0.5f;
        
        [Header("Data Stats")]
        [Tooltip("Maximum health points for the player.")]
        public float Health;
        
        [Header("Upgrade Stats")]
        [Tooltip("Score threshold required to trigger an upgrade opportunity.")]
        public int ScoreThresholdUpgrade = 2000;
        
        [Tooltip("Multiplier applied to the score threshold after each upgrade.")]
        [Range(1f, 5f)] public float ScoreThresholdMultiplier = 1.5f;
        
        [Header("Animation Stats")]
        [Tooltip("Speed at which the player rotates towards targets.")]
        public float RotationSpeed;
    }
}