using _SPC.GamePlay.Enemies.Destroyer.Scripts;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

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

        [Header("Upgrading Stats")] 
        [SerializeField] public int scoreThresholdUpgrade = 1000;
        [SerializeField] [Range(1f,5f)] public float scoreThresholdMultiplier = 1.5f;
        [SerializeField] public float upgradePunchIntensity = 0.5f;
        [SerializeField] public float upgradePunchTime;
    }
}
