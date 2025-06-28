using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    /// <summary>
    /// Utility component that automatically destroys a GameObject after its ParticleSystem finishes playing.
    /// Used for cleanup of particle effects without manual timing management.
    /// </summary>
    public class ParticleSystemDestroy : MonoBehaviour
    {
        /// <summary>
        /// Called when the component starts. Schedules destruction after particle system duration.
        /// </summary>
        void Start()
        {
            var particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem == null)
            {
                Debug.LogError("ParticleSystemDestroy: No ParticleSystem component found on GameObject.", this);
                return;
            }

            Destroy(gameObject, particleSystem.duration);
        }
    }
}