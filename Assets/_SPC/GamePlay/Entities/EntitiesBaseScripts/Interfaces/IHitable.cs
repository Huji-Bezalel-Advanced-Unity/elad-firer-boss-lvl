using _SPC.GamePlay.Weapons;
using UnityEngine;

namespace _SPC.GamePlay.Entities
{
    /// <summary>
    /// Interface for objects that can be hit by projectiles or other entities.
    /// </summary>
    public interface IHitable
    {
        /// <summary>
        /// Called when the object is hit by a projectile or other entity.
        /// </summary>
        /// <param name="projectileTransform">Position where the hit occurred.</param>
        /// <param name="shooterType">Type of weapon or entity that caused the hit.</param>
        void GotHit(Vector3 projectileTransform, WeaponType shooterType);
    }
    
}