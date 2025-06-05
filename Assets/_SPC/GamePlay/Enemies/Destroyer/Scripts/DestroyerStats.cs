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
        
        
    }
}
