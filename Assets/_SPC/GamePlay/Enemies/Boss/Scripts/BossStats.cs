using _SPC.GamePlay.Enemies.Destroyer.Scripts;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using DG.Tweening;

namespace _SPC.GamePlay.Enemies.Boss.Scripts
{
    [CreateAssetMenu(fileName = "BossStats", menuName = "Boss/Scriptable Objects/BossStats")]
    public class BossStats : ScriptableObject
    {
        [Header("Attacker Bullet Stats")]
        [SerializeField] [Range(5f,40f)] public float ProjectileSpeed;
        [SerializeField] [Range(0f,10f)] public float ProjectileBuffer;
        [SerializeField] [Range(0.1f,3f)] public float ProjectileSpawnRate;
        [SerializeField] public int bulletCount = 6;
        
        [Header("Destroyer Spawning Stats")]
        [SerializeField] public GameObject DestroyerPrefab;
        [SerializeField] public float minDistanceBetweenEnemies;
        [SerializeField] public int distanceBetweenEnemiesAccuracy;
        [SerializeField] public int numberOfEnemiesToSpawn = 3;
        [SerializeField] public float destroyerSpawnTime = 15f;

        [Header("Phase Stats")]
        [SerializeField] [Range(1f, 15f)] public float phase1SpecialAttackInterval = 7f;
        [SerializeField] [Range(1f, 15f)] public float phase2SpecialAttackInterval = 6f;
        [SerializeField] [Range(1f, 15f)] public float phase3SpecialAttackInterval = 5f;

        [Header("Rage Attack Stats")]
        [SerializeField] [Range(1f, 20f)] public float rageChargeDistance = 5f;
        [SerializeField] [Range(0.1f, 3f)] public float rageChargeTime = 0.5f;
        [SerializeField] [Range(0.1f, 3f)] public float rageHoldTime = 0.2f;
        [SerializeField] [Range(0.1f, 3f)] public float rageReturnTime = 0.8f;
        [SerializeField] public Ease rageChargeEase = Ease.OutQuad;
        [SerializeField] public Ease rageReturnEase = Ease.InOutQuad;
        [SerializeField] [Range(0.1f, 2f)] public float rageShakeTime = 0.5f;
        [SerializeField] [Range(0.1f, 2f)] public float rageShakeStrength = 0.3f;
        [SerializeField] [Range(1, 20)] public int rageShakeVibrato = 10;
        [SerializeField] [Range(0f, 180f)] public float rageShakeRandomness = 90f;
        [SerializeField] public Ease rageShakeEase = Ease.InOutQuad;

        [Header("Upgrading Stats")] 
        [SerializeField] public long scoreThresholdUpgrade = 1000;
        [SerializeField] [Range(1f,5f)] public float scoreThresholdMultiplier = 1.5f;
        [SerializeField] public float upgradePunchIntensity = 0.5f;
        [SerializeField] public float upgradePunchTime;

        [Header("Helath Stats")] 
        [SerializeField] public float Health;
    }
}
