using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Destroyer
{
    /// <summary>
    /// ScriptableObject containing all statistics and configuration for destroyer enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "DestroyerStats", menuName = "Boss/Scriptable Objects/DestroyerStats")]
    public class DestroyerStats : ScriptableObject
    {
        [Header("AttackerStats")]
        [Tooltip("Speed of projectiles fired by the destroyer.")]
        [SerializeField] [Range(5f,40f)] public float ProjectileSpeed;
        
        [Tooltip("Buffer distance for projectile collision detection.")]
        [SerializeField] [Range(0f,10f)] public float ProjectileBuffer;
        
        [Tooltip("Time between projectile spawns.")]
        [SerializeField] [Range(0.1f,3f)] public float ProjectileSpawnRate;
        
        [Tooltip("Smoothing factor for projectile movement.")]
        [SerializeField] [Range(0.01f,0.5f)] public float SmoothFactor;
        
        [Tooltip("Speed at which the destroyer bullets turn towards targets.")]
        [SerializeField] [Range(0f,360f)] public float turnSpeed;
        
        [Tooltip("Maximum health of the destroyer.")]
        [SerializeField] public float Health;

        [Header("Movement Stats")]
        [Tooltip("Movement speed of the destroyer.")]
        [SerializeField] [Range(1f, 20f)] public float MovementSpeed;

        [Header("Wander Settings")]
        [Tooltip("Minimum delay between wander movements.")]
        [field:SerializeField] public float minWanderDelay { get; set; } = 1f;
        
        [Tooltip("Maximum delay between wander movements.")]
        [field:SerializeField] public float maxWanderDelay { get; set; } = 3f;
    }
}
