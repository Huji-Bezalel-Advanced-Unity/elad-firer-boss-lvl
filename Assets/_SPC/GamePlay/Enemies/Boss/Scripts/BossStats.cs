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
        
        
        [Header("Destroyer Spawning Stats")]
        [SerializeField] public GameObject DestroyerPrefab;
        [SerializeField] public float minDistanceBetweenEnemies;
        [SerializeField] public int distanceBetweenEnemiesAccuracy;
    }
}
