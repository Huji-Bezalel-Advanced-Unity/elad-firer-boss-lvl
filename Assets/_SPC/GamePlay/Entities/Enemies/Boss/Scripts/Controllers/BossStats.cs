using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// ScriptableObject containing all configurable stats for the boss, including attack, phase, rage, laser, and upgrade parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "BossStats", menuName = "Boss/Scriptable Objects/BossStats")]
    public class BossStats : ScriptableObject
    {
        [Header("Attacker Bullet Stats")]
        [Tooltip("Speed of boss projectiles.")]
        [SerializeField] [Range(5f,40f)] public float ProjectileSpeed;
        [Tooltip("Buffer distance for projectile spawn.")]
        [SerializeField] [Range(0f,10f)] public float ProjectileBuffer;
        [Tooltip("Time between projectile spawns.")]
        [SerializeField] [Range(0.1f,3f)] public float ProjectileSpawnRate;
        [Tooltip("Number of bullets per attack.")]
        [SerializeField] public int bulletCount;
        
        [Header("Destroyer Spawning Stats")]
        [Tooltip("Prefab for spawned destroyer enemies.")]
        [SerializeField] public GameObject DestroyerPrefab;
        [Tooltip("Minimum distance between spawned enemies.")]
        [SerializeField] public float minDistanceBetweenEnemies;
        [Tooltip("Accuracy for distance calculation between enemies.")]
        [SerializeField] public int distanceBetweenEnemiesAccuracy;
        [Tooltip("Number of enemies to spawn per attack.")]
        [SerializeField] public int numberOfEnemiesToSpawn;
        [Tooltip("Time between destroyer spawns.")]
        [SerializeField] public float destroyerSpawnTime;

        [Header("Phase Stats")]
        [Tooltip("Interval between special attacks in phase 1.")]
        [SerializeField] [Range(1f, 15f)] public float phase1SpecialAttackInterval;
        [Tooltip("Interval between special attacks in phase 2.")]
        [SerializeField] [Range(1f, 15f)] public float phase2SpecialAttackInterval;
        [Tooltip("Interval between special attacks in phase 3.")]
        [SerializeField] [Range(1f, 15f)] public float phase3SpecialAttackInterval;

        [Header("Rage Attack Stats")]
        [Tooltip("Time to charge rage attack.")]
        [SerializeField] [Range(0.1f, 3f)] public float rageChargeTime;
        [Tooltip("Time to hold rage attack.")]
        [SerializeField] [Range(0.1f, 3f)] public float rageHoldTime;
        [Tooltip("Time to return from rage attack.")]
        [SerializeField] [Range(0.1f, 3f)] public float rageReturnTime;
        [Tooltip("Ease for rage charge animation.")]
        [SerializeField] public Ease rageChargeEase = Ease.OutQuad;
        [Tooltip("Ease for rage return animation.")]
        [SerializeField] public Ease rageReturnEase = Ease.InOutQuad;
        [Tooltip("Shake time for rage effect.")]
        [SerializeField] [Range(0.1f, 2f)] public float rageShakeTime;
        [Tooltip("Shake strength for rage effect.")]
        [SerializeField] [Range(0.1f, 2f)] public float rageShakeStrength;
        [Tooltip("Shake vibrato for rage effect.")]
        [SerializeField] [Range(1, 20)] public int rageShakeVibrato;
        [Tooltip("Shake randomness for rage effect.")]
        [SerializeField] [Range(0f, 180f)] public float rageShakeRandomness;
        [Tooltip("Ease for rage shake animation.")]
        [SerializeField] public Ease rageShakeEase = Ease.InOutQuad;

        [Header("Laser Attack Stats")]
        [Tooltip("Time to stretch the laser to its full length.")]
        [SerializeField] [Range(0.5f, 3f)] public float laserStretchTime;
        [Tooltip("Speed at which the laser moves across the arena.")]
        [SerializeField] [Range(1f, 50f)] public float laserMoveSpeed;

        [Header("Upgrading Stats")]
        [Tooltip("Score threshold for boss upgrade.")]
        [SerializeField] public long scoreThresholdUpgrade;
        [Tooltip("Multiplier for score threshold on upgrade.")]
        [SerializeField] [Range(1f,5f)] public float scoreThresholdMultiplier;
        [Tooltip("Punch intensity for upgrade animation.")]
        [SerializeField] public float upgradePunchIntensity;
        [Tooltip("Punch time for upgrade animation.")]
        [SerializeField] public float upgradePunchTime;

        [Header("Health Stats")]
        [Tooltip("Boss health value.")]
        [SerializeField] public float Health;
    }
}
