using _SPC.GamePlay.Weapons;
using UnityEngine;

namespace _SPC.GamePlay.Entities
{
    public interface IHitable
    {
        public void GotHit(Vector3 projectileTransform, WeaponType shooterType);
    }
    
}