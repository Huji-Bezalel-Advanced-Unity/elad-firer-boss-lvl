using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.Core.Scripts.Interfaces
{
    public interface IHitable
    {
        public void GotHit(Vector3 projectileTransform, WeaponType shooterType);
    }
    
}