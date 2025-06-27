using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    [CreateAssetMenu(fileName = "BossStats", menuName = "Boss/Scriptable Objects/BossStats")]
    public class BossStats : ScriptableObject
    {
        [Header("Attacker Bullet Stats")]
        [SerializeField] [Range(5f,40f)] public float ProjectileSpeed;
        [SerializeField] [Range(0f,10f)] public float ProjectileBuffer;
        [SerializeField] [Range(0.1f,3f)] public float ProjectileSpawnRate;
        [SerializeField] public int bulletCount;
        
        [Header("Destroyer Spawning Stats")]
        [SerializeField] public GameObject DestroyerPrefab;
        [SerializeField] public float minDistanceBetweenEnemies;
        [SerializeField] public int distanceBetweenEnemiesAccuracy;
        [SerializeField] public int numberOfEnemiesToSpawn;
        [SerializeField] public float destroyerSpawnTime;

        [Header("Phase Stats")]
        [SerializeField] [Range(1f, 15f)] public float phase1SpecialAttackInterval;
        [SerializeField] [Range(1f, 15f)] public float phase2SpecialAttackInterval;
        [SerializeField] [Range(1f, 15f)] public float phase3SpecialAttackInterval;

        [Header("Rage Attack Stats")]
        [SerializeField] [Range(0.1f, 3f)] public float rageChargeTime;
        [SerializeField] [Range(0.1f, 3f)] public float rageHoldTime;
        [SerializeField] [Range(0.1f, 3f)] public float rageReturnTime;
        [SerializeField] public Ease rageChargeEase = Ease.OutQuad;
        [SerializeField] public Ease rageReturnEase = Ease.InOutQuad;
        [SerializeField] [Range(0.1f, 2f)] public float rageShakeTime;
        [SerializeField] [Range(0.1f, 2f)] public float rageShakeStrength;
        [SerializeField] [Range(1, 20)] public int rageShakeVibrato;
        [SerializeField] [Range(0f, 180f)] public float rageShakeRandomness;
        [SerializeField] public Ease rageShakeEase = Ease.InOutQuad;

        [Header("Laser Attack Stats")]
        [SerializeField] [Range(0.5f, 3f)] public float laserStretchTime;

        [SerializeField] [Range(1f, 50f)] public float laserMoveSpeed;

        [Header("Upgrading Stats")] 
        [SerializeField] public long scoreThresholdUpgrade;
        [SerializeField] [Range(1f,5f)] public float scoreThresholdMultiplier;
        [SerializeField] public float upgradePunchIntensity;
        [SerializeField] public float upgradePunchTime;

        [Header("Helath Stats")] 
        [SerializeField] public float Health;

    }
}
