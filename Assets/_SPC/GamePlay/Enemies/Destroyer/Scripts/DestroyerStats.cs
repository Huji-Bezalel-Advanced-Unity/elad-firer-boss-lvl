using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Destroyer.Scripts
{
    [CreateAssetMenu(fileName = "DestroyerStats", menuName = "Boss/Scriptable Objects/DestroyerStats")]
    public class DestroyerStats : ScriptableObject
    {
        [Header("AttackerStats")]
        [SerializeField] [Range(5f,40f)] public float ProjectileSpeed;
        [SerializeField] [Range(0f,10f)] public float ProjectileBuffer;
        [SerializeField] [Range(0.1f,3f)] public float ProjectileSpawnRate;
        [SerializeField] [Range(0.01f,0.5f)] public float SmoothFactor;
        [SerializeField] [Range(0f,360f)] public float turnSpeed;
        [SerializeField] public float Health;
    }
}
